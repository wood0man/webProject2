﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
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
                return View();

            }


            

            return View();
        
        }
    }
}