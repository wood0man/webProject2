using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using webProject2.Models;

namespace webProject2.Data
{
    public class webProject2Context : DbContext
    {
        public webProject2Context (DbContextOptions<webProject2Context> options)
            : base(options)
        {
        }

        public DbSet<webProject2.Models.items> items { get; set; } = default!;

        public DbSet<webProject2.Models.orders>? orders { get; set; }

        public DbSet<webProject2.Models.users>? users { get; set; }
    }
}
