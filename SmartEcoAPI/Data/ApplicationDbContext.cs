using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartEcoAPI.Models;

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

        public DbSet<SmartEcoAPI.Models.MeasuredParameter> MeasuredParameter { get; set; }

        public DbSet<SmartEcoAPI.Models.EcomonMonitoringPoint> EcomonMonitoringPoint { get; set; }

        public DbSet<SmartEcoAPI.Models.MeasuredData> MeasuredData { get; set; }

        public DbSet<SmartEcoAPI.Models.Log> Log { get; set; }

        public DbSet<SmartEcoAPI.Models.Pollutant> Pollutant { get; set; }

        public DbSet<SmartEcoAPI.Models.PollutionSource> PollutionSource { get; set; }

        public DbSet<SmartEcoAPI.Models.PollutionSourceData> PollutionSourceData { get; set; }

        public DbSet<SmartEcoAPI.Models.KazHydrometAirPost> KazHydrometAirPost { get; set; }
    }
}
