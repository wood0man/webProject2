using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
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

        public IActionResult adminPage()
        {

            return View();
        }


        public IActionResult customerPage() {

            if (HttpContext.Session.GetString("role") == "admin"){
                return View("login"); }


            return View(); 
        }
        public IActionResult index() {
            return View();
        }

        public IActionResult Details()
        {
            return View();
        }
        public IActionResult login() {
            return View();
        }
        [HttpPost]
        public IActionResult login(string name,string password) {

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

            return View();
        }
        [HttpPost]

        public IActionResult mypurchase(int userid) {

            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=mono;Integrated Security=True");

            string sql = "select userid from userid where Id= '"+HttpContext.Session.GetString("userid")+"'";

            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            
            orders order = new orders();
            SqlDataReader reader = comm.ExecuteReader();
            
            if (reader.Read())
            {

                order.itemid = (int)reader["itemid"];
                order.userid = (int)reader["userid"];
                order.quantity = (int)reader["quantity"];
                order.buyDate = (DateTime)reader["buyDate"];
            }
            conn.Close();

            return View(order);

        }
    }
}