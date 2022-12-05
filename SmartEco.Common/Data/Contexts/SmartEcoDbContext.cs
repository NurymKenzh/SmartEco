using Microsoft.EntityFrameworkCore;
using SmartEco.Common.Data.Entities;
using SmartEco.Common.Enums;

namespace SmartEco.Common.Data.Contexts
{
    public class SmartEcoDbContext : DbContext
    {
        public SmartEcoDbContext(DbContextOptions<SmartEcoDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Person>()
                .Property(e => e.Role)
                .HasConversion(role => role.ToString(), role => (Role)Enum.Parse(typeof(Role), role));
        }

        public DbSet<Person> Person { get; set; }
    }
}
