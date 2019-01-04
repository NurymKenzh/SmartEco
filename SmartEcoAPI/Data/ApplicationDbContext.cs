using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<CompanyKK>()
            //    .HasIndex(c => c.NameRU);

            //modelBuilder.Entity<Person>()
            //.HasIndex(p => new { p.FirstName, p.LastName });
        }
    }
}
