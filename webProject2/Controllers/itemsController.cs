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
    public class itemsController : Controller
    {
        private readonly webProject2Context _context;

        public itemsController(webProject2Context context)
        {
            _context = context;
        }

        // GET: items
        public async Task<IActionResult> Index()
        {
              return _context.items != null ? 
                          View(await _context.items.ToListAsync()) :
                          Problem("Entity set 'webProject2Context.items'  is null.");
        }

        // GET: items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.items == null)
            {
                return NotFound();
            }

            var items = await _context.items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (items == null)
            {
                return NotFound();
            }

            return View(items);
        }

        // GET: items/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,name,description,price,quantity,discount,category,image")] items items)
        {
            if (ModelState.IsValid)
            {
                _context.Add(items);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(items);
        }

        // GET: items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.items == null)
            {
                return NotFound();
            }

            var items = await _context.items.FindAsync(id);
            if (items == null)
            {
                return NotFound();
            }
            return View(items);
        }

        // POST: items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,description,price,quantity,discount,category,image")] items items)
        {
            if (id != items.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(items);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!itemsExists(items.Id))
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
            return View(items);
        }

        // GET: items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.items == null)
            {
                return NotFound();
            }

            var items = await _context.items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (items == null)
            {
                return NotFound();
            }

            return View(items);
        }

        // POST: items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.items == null)
            {
                return Problem("Entity set 'webProject2Context.items'  is null.");
            }
            var items = await _context.items.FindAsync(id);
            if (items != null)
            {
                _context.items.Remove(items);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool itemsExists(int id)
        {
          return (_context.items?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public IActionResult search() {
            items item = new items();

            ViewData["full"] = "blanching";
            return View(item);
            
        }
        [HttpPost]
        public IActionResult search(string title) {
            List<items> list=new List<items>();
            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=mono;Integrated Security=True");
            string sql = "SELECT * FROM items where name like '%" + title + "%'";
            SqlCommand comm =   new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            items items = new items();
            while (reader.Read())
            {

                list.Add(new items {

                    name = (string)reader["name"],
                description = (string)reader["description"],
                price = (int)reader["price"],
                category = (string)reader["category"],
                discount = (string)reader["discount"],
                quantity = (int)reader["quantity"],
                image = (string)reader["image"]




            });
            
        }
            conn.Close();
            reader.Close();
            ViewData["full"]=null;
            return View(list);



        }

        

    }
}
