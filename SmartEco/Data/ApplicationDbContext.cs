using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartEco.Data;
using SmartEco.Models;

namespace SmartEco.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        //public DbSet<SmartEco.Models.Pollutant> Pollutant { get; set; }
        //public DbSet<SmartEco.Models.PollutionSource> PollutionSource { get; set; }
        //public DbSet<SmartEco.Models.PollutionSourceData> PollutionSourceData { get; set; }
        //public DbSet<SmartEco.Models.KazHydrometAirPost> KazHydrometAirPost { get; set; }
        //public DbSet<SmartEco.Models.KazHydrometSoilPost> KazHydrometSoilPost { get; set; }
        //public DbSet<SmartEco.Models.KATO> KATO { get; set; }
        //public DbSet<SmartEco.Models.PollutionEnvironment> PollutionEnvironment { get; set; }
        //public DbSet<SmartEco.Models.DataProvider> DataProvider { get; set; }
        //public DbSet<SmartEco.Models.EcomonMonitoringPoint> EcomonMonitoringPoint { get; set; }
        //public DbSet<SmartEco.Models.Log> Log { get; set; }
    }
}
