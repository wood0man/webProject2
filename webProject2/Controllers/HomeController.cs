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
                return View("login"); 
            }

            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("webProject2Context");
            SqlConnection conn = new SqlConnection(conStr);
            string sql = "select * from items where discount='yes' ";
            SqlCommand comm=new SqlCommand  (sql, conn) ;
            conn.Open();
            List<items>list=new List<items>();
            SqlDataReader reader = comm.ExecuteReader();
            while (reader.Read()) {
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
            ViewData["name"] = HttpContext.Session.GetString("name");
            conn.Close();
            return View(list); 
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
            ViewData = null;   
        }
        [HttpPost]
        public IActionResult login(string name,string password, bool autologin) {

            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("webProject2Context");
            SqlConnection conn = new SqlConnection(conStr);
            string sql = "SELECT * FROM users where name= '" + name + "' and password = '"+password+"'";
            SqlCommand comm=new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();

            if (reader.Read())
            {

                string id = Convert.ToString((int)reader["Id"]);
                string name1 = (string)reader["name"];
                string role = (string)reader["role"];
                HttpContext.Session.SetString("userid", id);
                HttpContext.Session.SetString("name", name);
                HttpContext.Session.SetString("role", role);
                reader.Close();
                conn.Close();

                ViewData["name"] = HttpContext.Session.GetString("name");

                if (autologin)
                {
                    var cookieOptions = new CookieOptions
                    { Expires = DateTime.Now.AddDays(30) };
                    HttpContext.Response.Cookies.Append("name", name, cookieOptions);
                    HttpContext.Response.Cookies.Append("password", password, cookieOptions);
                }
                if (role == "customer")
                {


                    return RedirectToAction("customerPage");
                }
                else
                {
                    return View("adminPage");
                }



            }


            else
            {
                ViewData["wrongLoginInfo"] = "Wrong password or username";
                conn.Close();
                return View();
            }
            
        
        }


        public IActionResult itemslist() {

            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("webProject2Context");
            SqlConnection conn = new SqlConnection(conStr);
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
            conn.Close();
            return View(list);
        }

        public IActionResult register() {

            return View();
        }

        [HttpPost]
        public IActionResult register(string name, string password,string password2) {

            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("webProject2Context");
            SqlConnection conn = new SqlConnection(conStr);
            string sql = "SELECT * FROM users where name= '"+name+"'and password= '"+password+"'";

            SqlCommand comm= new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            if (reader.Read())
            {
                ViewData["userDoesExists"] = "Windows.alert(\"The user does already exists. choose another name\")";
                return View();
            }
            else if (password != password2) {
                ViewData["PassMessage"] = "windows.alert(\"the passwords don't match\")";
            }
            else
            {
                reader.Close();
                sql = "insert into users (name,password,registerDate,role) values ('" + name + "','" + password + "',CURRENT_TIMESTAMP,'customer')";
                comm = new SqlCommand(sql, conn);
                comm.ExecuteNonQuery();
                conn.Close();
                return View("login");
            }

            conn.Close();
            return View();


        }


        public IActionResult mypurchase() {
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("webProject2Context");
            SqlConnection conn = new SqlConnection(conStr);
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
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("webProject2Context");
            SqlConnection conn = new SqlConnection(conStr);
            
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


        public IActionResult logout()
        {
            HttpContext.Session.Remove("userid");
            HttpContext.Session.Remove("role");
            HttpContext.Session.Remove("name");
            HttpContext.Response.Cookies.Delete("name");
            HttpContext.Response.Cookies.Delete("role");
            HttpContext.Response.Cookies.Delete("userid");
            return RedirectToAction("login", "Home");
        }

        public IActionResult dashboard()
        {

            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("webProject2Context");
            SqlConnection conn = new SqlConnection(conStr);
            string sql = "select count(quantity) from items where category= 'Fantasy' ";
            SqlCommand command = new SqlCommand(sql, conn);
            conn.Open();

            ViewData["c1"] = (int)command.ExecuteScalar();
            sql = "select count(quantity) from items where category= 'Mystery' ";
            command = new SqlCommand(sql, conn);
            ViewData["c2"] = (int)command.ExecuteScalar();
            sql = "select count(quantity) from items where category= 'Adventure' ";
            command = new SqlCommand(sql, conn);
            ViewData["c3"] = command.ExecuteScalar();
            conn.Close();
            return View();
        }
    

        
}
}