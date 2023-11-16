using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=mono;Integrated Security=True");
            string sql = "Select * from orders  ";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new orders
                {
                    Id = (int)reader["Id"],
                    userid = (int)reader["userid"],
                    itemid = (int)reader["itemid"],
                    buyDate = (DateTime)reader["buyDate"],
                    quantity = (int)reader["quantity"]
                });

            }
            ViewData["state"] = "blanching";
            return View(list);
        }




        [HttpPost]
        public IActionResult invoicelist(string order) {
            List<orders> list = new List<orders>();
            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=mono;Integrated Security=True");
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
            return View(list);
        }

        public IActionResult buy() {
            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=mono;Integrated Security=True");
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
            return View(list);
        }

        [HttpPost]

        public IActionResult buy(int itemid,int quantity) {
            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=mono;Integrated Security=True");

            string name=null;
            
            
            int userid = Convert.ToInt16(HttpContext.Session.GetString("userid"));


            string sql = "SELECT * FROM items";

            SqlCommand comm=new SqlCommand(sql, conn);
            List<items> list = new List<items>();
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
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
            return View(list);
        
        
        }

        
    } 
}

