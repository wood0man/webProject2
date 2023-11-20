using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Net.Mail;
using System.Xml.Linq;
using webProject2.Models;

namespace webProject2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> adminPage()
        {
            List<categories> list = new List<categories>();

            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=mono;Integrated Security=True");
            string sql = "select distinct(category)from items";
            SqlCommand comm = new SqlCommand(sql, conn) ;
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            while (reader.Read()) {
                list.Add(new categories { category = (string)reader["category"] });
            }

           
            return View(list);
            
        }



        public IActionResult customerPage() {

            if (HttpContext.Session.GetString("role") == "admin"){
                return View("login"); }


            return View(); 
        }
       

        public IActionResult Details()
        {
            return View();
        }
        public IActionResult login() {
            if (!HttpContext.Request.Cookies.ContainsKey("name"))
                return View();
            else
            {
                string name = HttpContext.Request.Cookies["name"].ToString();
                string password = HttpContext.Request.Cookies["password"].ToString();
                ViewData["name"] = name;
                ViewData["password"] = password;

                return View();
            }

        }
        [HttpPost]
        public IActionResult login(string name,string password, Boolean autologin) {

            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=mono;Integrated Security=True");
            string sql = "SELECT * FROM users where name= '" + name + "' and password = '"+password+"'";
            SqlCommand comm=new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            
            if(reader.Read())
            {

                string id = Convert.ToString((int)reader["Id"]);
                string name1 = (string)reader["name"];
                string role = (string)reader["role"];
                HttpContext.Session.SetString("userid", id);
                HttpContext.Session.SetString("name", name);
                HttpContext.Session.SetString("role", role);
                reader.Close();
                conn.Close();
                // dunno why this code is here but I'll leave it lol
                ViewData["name"]=HttpContext.Session.GetString("name");
                if (autologin == true)
                {
                    var cookieOptions = new CookieOptions
                    { Expires = DateTime.Now.AddDays(30) };
                    HttpContext.Response.Cookies.Append("name", name, cookieOptions);
                    HttpContext.Response.Cookies.Append("password", password, cookieOptions);
                }

                if (role == "customer")
                {


                    return View("customerPage");
                }
                else if (role == "admin")
                {
                    return View("adminPage");
                }


                else
                    ViewData["wrongLoginInfo"] = "Wrong password or username";
                return View("login");

            }


           

            return View();
        
        }


        public IActionResult itemslist() {

            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=mono;Integrated Security=True");
            List<items> list=new List<items>();
            string sql = "select * from items ";
            SqlCommand command = new SqlCommand(sql, conn);
            SqlDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                list.Add(new items
                {
                    Id = (int)reader["Id"],
                    name = (string)reader["name"],
                    description = (string)reader["description"],
                    price = (int)reader["price"],
                    quantity = (int)reader["quantity"],
                    discount = (string)reader["discount"],
                    category = (string)reader["category"],
                    image = (string)reader["image"]
                }) ;
            }
            
            return View(list);
        }

        public IActionResult register() {

            return View();
        }

        [HttpPost]
        public IActionResult register(string name, string password) {

            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=mono;Integrated Security=True");

            string sql = "insert into users (name,password,registerDate,role) values ('"+name+"','"+password+ "',CURRENT_TIMESTAMP,'customer')";

            SqlCommand comm= new SqlCommand(sql, conn);
            conn.Open();
            comm.ExecuteNonQuery();
            conn.Close();
            return View("login");





        }


        public IActionResult mypurchase() {
            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=mono;Integrated Security=True");

            string sql = "select * from orders where userid= (select Id from userid where userid= '" + HttpContext.Session.GetString("userid") + "' ) ";

            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();


            SqlDataReader reader = comm.ExecuteReader();
            List<orders> list = new List<orders>();
            while (reader.Read())
            {
                list.Add(new orders
                {

                    itemid = (int)reader["itemid"],
                    userid = (int)reader["userid"],
                    quantity = (int)reader["quantity"],
                    buyDate = (DateTime)reader["buyDate"]


                });
            }
            conn.Close();

            return View(list);
        }

        public IActionResult ourproducts() {
            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=mono;Integrated Security=True");
            List<items> list = new List<items>();
            string sql = "select * from items ";
            SqlCommand command = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new items
                {
                    Id = (int)reader["Id"],
                    name = (string)reader["name"],
                    description = (string)reader["description"],
                    price = (int)reader["price"],
                    quantity = (int)reader["quantity"],
                    discount = (string)reader["discount"],
                    category = (string)reader["category"],
                    image = (string)reader["image"]
                });
            }
            conn.Close();
            reader.Close();
            return View(list);
        }

        public IActionResult email()
        {

            return View();
        }


        [HttpPost, ActionName("email")]
        [ValidateAntiForgeryToken]
        public IActionResult email(string address, string subject, string body)
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            var mail = new MailMessage();
            mail.From = new MailAddress("wood03man@gmail.com");
            mail.To.Add(address); // receiver email address
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            mail.Body = body;
            SmtpServer.Port = 587;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = new System.Net.NetworkCredential("wood03man@gmail.com","jilcatxqpjhhietk");
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
            ViewData["Message"] = "Email sent.";
            return View();

        }

       


    }
}