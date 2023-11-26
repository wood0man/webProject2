using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using webProject2.Data;
using webProject2.Models;

namespace webProject2.Controllers
{
    public class ordersController : Controller
    {
        private readonly webProject2Context _context;

        public ordersController(webProject2Context context)
        {
            _context = context;
        }

        // GET: orders
        public async Task<IActionResult> Index()
        {
            return _context.orders != null ?
                        View(await _context.orders.ToListAsync()) :
                        Problem("Entity set 'webProject2Context.orders'  is null.");
        }

        // GET: orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.orders == null)
            {
                return NotFound();
            }

            var orders = await _context.orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // GET: orders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,userid,itemid,buyDate,quantity")] orders orders)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orders);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(orders);
        }

        // GET: orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.orders == null)
            {
                return NotFound();
            }

            var orders = await _context.orders.FindAsync(id);
            if (orders == null)
            {
                return NotFound();
            }
            return View(orders);
        }

        // POST: orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,userid,itemid,buyDate,quantity")] orders orders)
        {
            if (id != orders.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orders);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ordersExists(orders.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(orders);
        }

        // GET: orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.orders == null)
            {
                return NotFound();
            }

            var orders = await _context.orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // POST: orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.orders == null)
            {
                return Problem("Entity set 'webProject2Context.orders'  is null.");
            }
            var orders = await _context.orders.FindAsync(id);
            if (orders != null)
            {
                _context.orders.Remove(orders);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ordersExists(int id)
        {
            return (_context.orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        public IActionResult invoicelist()
        {
            List<orders> list = new List<orders>();
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("webProject2Context");
            SqlConnection conn = new SqlConnection(conStr);
            string sql = "Select distinct(userid) from orders  ";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new orders
                {
                    userid = (int)reader["userid"]
                });

            }
            ViewData["state"] = "blanching";
            conn.Close();
            return View(list);
        }




        [HttpPost]
        public IActionResult invoicelist(string order) {
            List<orders> list = new List<orders>();
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("webProject2Context");
            SqlConnection conn = new SqlConnection(conStr);
            string sql = "Select * from orders where userid ='" + order + "' ";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new orders { Id = (int)reader["Id"],
                    userid = (int)reader["userid"],
                    itemid = (int)reader["itemid"],
                    buyDate = (DateTime)reader["buyDate"],
                    quantity = (int)reader["quantity"]
                });

            }
            ViewData["state"] = null;
            conn.Close();
            return View(list);
        }

        public IActionResult buy() {
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("webProject2Context");
            SqlConnection conn = new SqlConnection(conStr);
            string sql = "select * from items where quantity>0 ";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            List<items> list = new List<items>();
            SqlDataReader reader = comm.ExecuteReader();
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
                });


            }
            conn.Close();
            return View(list);
        }

        [HttpPost]

        public IActionResult buy(int itemid,int quantity) {
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("webProject2Context");
            SqlConnection conn = new SqlConnection(conStr);

            string name =null;
            int userid = Convert.ToInt16(HttpContext.Session.GetString("userid"));

            string sql = "SELECT * FROM items where quantity >0";

            SqlCommand comm=new SqlCommand(sql, conn);
            List<items> list = new List<items>();
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            while (reader.Read())
            {

                if ((int)reader["quantity"] - quantity <= 0) {
                    ViewData["buyMessage"] = "Out of stock. sorry";
                }

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
            reader.Close();
             sql = "select * from items where Id='"+itemid+"'";
             comm = new SqlCommand(sql, conn);
            conn.Close();
            conn.Open();
             reader= comm.ExecuteReader();
            if (reader.Read()) {
                
                name= (string)reader["name"];
                
            }
            conn.Close();
            
            reader.Close();
            sql = "insert into cart (name,quantity, userid,itemid) values('"+name+"','"+quantity+"','"+userid+"', '"+itemid+"') ";
                comm = new SqlCommand(sql, conn);
                conn.Open();

            comm.ExecuteNonQuery();
            conn.Close();


            sql = " update items set quantity = quantity - '" + quantity + "' where Id='" + itemid + "'";
            comm= new SqlCommand(sql, conn);
            conn.Open();
            comm.ExecuteNonQuery();

            ViewData["buyMessage"] = "Added to cart";
            conn.Close();
            return View(list);
        
        
        }


        public IActionResult checkout() {
            double amount = 0;
            List<cart>list= new List<cart>();
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("webProject2Context");
            SqlConnection conn = new SqlConnection(conStr);
            string sql = "SELECT * FROM cart where userid= '"+HttpContext.Session.GetString("userid")+"' ";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader= comm.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new cart {
                    userid = (int)reader["userid"],
                    itemid = (int)reader["itemid"],
                    name = (string)reader["name"],
                    quantity = (int)reader["quantity"]
                });

                
                 sql = "SELECT * FROM items where Id= '" + reader["itemid"] + "' ";
                SqlConnection conn2 = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=mono;Integrated Security=True");

                SqlCommand comm2 = new SqlCommand(sql, conn2);
                conn2.Open();
                SqlDataReader reader2 = comm2.ExecuteReader();
                if (reader2.Read()) {

                    if ((string)reader2["discount"] == "yes")
                    {
                        double pp = (int)reader2["price"];
                        double discountedprice = pp * 0.1;
                        amount += (int)reader2["price"] - discountedprice;
                    }
                    else
                    {
                        amount += (int)reader2["price"];     
                    }
                }
                reader2.Close();
                conn2.Close();
            }
            ViewData["amount"] =amount;

            conn.Close();
            return View(list);
        }
        [HttpPost]
        public IActionResult checkout(int userid) {
            userid = Convert.ToInt16(HttpContext.Session.GetString("userid"));
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("webProject2Context");
            SqlConnection conn = new SqlConnection(conStr);

            string sql = "SELECT * FROM cart where userid= '" + HttpContext.Session.GetString("userid") + "' ";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            var builder2 = WebApplication.CreateBuilder();
            string conStr2 = builder.Configuration.GetConnectionString("webProject2Context");
            SqlConnection conn2 = new SqlConnection(conStr);
            conn2.Open();
            while (reader.Read()) {
                sql = "insert into orders (userid,itemid,buyDate,quantity) values ('"+userid+"' , '" + (int)reader["itemid"] +"', GETDATE() ,'" + (int)reader["quantity"] +"')";
                comm = new SqlCommand(sql, conn2);
                comm.ExecuteNonQuery();
            }


            sql = "DELETE FROM cart where userid = '"+userid+"' ";
            conn.Close();
            conn.Open();
             comm = new SqlCommand(sql, conn);
            comm.ExecuteNonQuery();

            conn2.Close();
            conn.Close();

           

            return RedirectToAction("customerPage", "Home");


        }
    } 
}

