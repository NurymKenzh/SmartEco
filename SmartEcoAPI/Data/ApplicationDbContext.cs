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

        public DbSet<SmartEcoAPI.Models.KazHydrometSoilPost> KazHydrometSoilPost { get; set; }

        public DbSet<SmartEcoAPI.Models.KATO> KATO { get; set; }

        public DbSet<SmartEcoAPI.Models.PollutionEnvironment> PollutionEnvironment { get; set; }

        public DbSet<SmartEcoAPI.Models.DataProvider> DataProvider { get; set; }

        public DbSet<SmartEcoAPI.Models.MonitoringPost> MonitoringPost { get; set; }

        public DbSet<SmartEcoAPI.Models.Layer> Layer { get; set; }

        public DbSet<SmartEcoAPI.Models.Person> Person { get; set; }

        public DbSet<SmartEcoAPI.Models.MonitoringPostMeasuredParameters> MonitoringPostMeasuredParameters { get; set; }

        public DbSet<SmartEcoAPI.Models.MeasuredParameterUnit> MeasuredParameterUnit { get; set; }

        public DbSet<SmartEcoAPI.Models.Project> Project { get; set; }

        public DbSet<SmartEcoAPI.Models.TerritoryType> TerritoryType { get; set; }

        public DbSet<SmartEcoAPI.Models.Target> Target { get; set; }

        public DbSet<SmartEcoAPI.Models.Event> Event { get; set; }

        public DbSet<SmartEcoAPI.Models.TargetTerritory> TargetTerritory { get; set; }

        public DbSet<SmartEcoAPI.Models.TargetValue> TargetValue { get; set; }

        public DbSet<SmartEcoAPI.Models.AActivity> AActivity { get; set; }

        public DbSet<SmartEcoAPI.Models.LEDScreen> LEDScreen { get; set; }

        public DbSet<SmartEcoAPI.Models.Executor> Executor { get; set; }

        public DbSet<SmartEcoAPI.Models.AActivityExecutor> AActivityExecutor { get; set; }

        public DbSet<SmartEcoAPI.Models.PlantationsType> PlantationsType { get; set; }

        public DbSet<SmartEcoAPI.Models.PlantationsStateType> PlantationsStateType { get; set; }

        public DbSet<SmartEcoAPI.Models.AuthorizedAuthority> AuthorizedAuthority { get; set; }

        public DbSet<SmartEcoAPI.Models.SpeciallyProtectedNaturalTerritory> SpeciallyProtectedNaturalTerritory { get; set; }

        public DbSet<SmartEcoAPI.Models.GreemPlantsPassport> GreemPlantsPassport { get; set; }

        public DbSet<SmartEcoAPI.Models.PlantationsState> PlantationsState { get; set; }

        public DbSet<SmartEcoAPI.Models.SpeciesDiversity> SpeciesDiversity { get; set; }
    }
}
