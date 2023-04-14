using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartEcoAPI.Akimato.Models;

namespace SmartEcoAPI.Akimato.Data
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

        public DbSet<SmartEcoAPI.Akimato.Models.MeasuredParameter> MeasuredParameter { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.EcomonMonitoringPoint> EcomonMonitoringPoint { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.MeasuredData> MeasuredData { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.Log> Log { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.Pollutant> Pollutant { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.PollutionSource> PollutionSource { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.PollutionSourceData> PollutionSourceData { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.KazHydrometAirPost> KazHydrometAirPost { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.KazHydrometSoilPost> KazHydrometSoilPost { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.KATO> KATO { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.PollutionEnvironment> PollutionEnvironment { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.DataProvider> DataProvider { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.MonitoringPost> MonitoringPost { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.Layer> Layer { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.Person> Person { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.MonitoringPostMeasuredParameters> MonitoringPostMeasuredParameters { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.MeasuredParameterUnit> MeasuredParameterUnit { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.Project> Project { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.TerritoryType> TerritoryType { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.Target> Target { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.Event> Event { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.TargetTerritory> TargetTerritory { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.TargetValue> TargetValue { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.AActivity> AActivity { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.LEDScreen> LEDScreen { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.Executor> Executor { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.AActivityExecutor> AActivityExecutor { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.PlantationsType> PlantationsType { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.PlantationsStateType> PlantationsStateType { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.AuthorizedAuthority> AuthorizedAuthority { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.SpeciallyProtectedNaturalTerritory> SpeciallyProtectedNaturalTerritory { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.GreemPlantsPassport> GreemPlantsPassport { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.TreesByObjectTableOfTheBreedStateList> TreesByObjectTableOfTheBreedStateList { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.TreesByFacilityManagementMeasuresList> TreesByFacilityManagementMeasuresList { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.PlantationsState> PlantationsState { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.SpeciesDiversity> SpeciesDiversity { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.Ecopost> Ecopost { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.ReceptionRecyclingPoint> ReceptionRecyclingPoint { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.Question> Question { get; set; }

        public DbSet<SmartEcoAPI.Akimato.Models.Answer> Answer { get; set; }
    }
}
