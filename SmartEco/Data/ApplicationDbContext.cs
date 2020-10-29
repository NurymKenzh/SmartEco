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
        public DbSet<SmartEco.Models.Pollutant> Pollutant { get; set; }
        public DbSet<SmartEco.Models.PollutionSource> PollutionSource { get; set; }
        public DbSet<SmartEco.Models.PollutionSourceData> PollutionSourceData { get; set; }
        public DbSet<SmartEco.Models.KazHydrometAirPost> KazHydrometAirPost { get; set; }
        public DbSet<SmartEco.Models.KazHydrometSoilPost> KazHydrometSoilPost { get; set; }
        public DbSet<SmartEco.Models.KATO> KATO { get; set; }
        public DbSet<SmartEco.Models.PollutionEnvironment> PollutionEnvironment { get; set; }
        public DbSet<SmartEco.Models.DataProvider> DataProvider { get; set; }
        public DbSet<SmartEco.Models.MonitoringPost> MonitoringPost { get; set; }
        public DbSet<SmartEco.Models.Project> Project { get; set; }
        public DbSet<SmartEco.Models.TerritoryType> TerritoryType { get; set; }
        public DbSet<SmartEco.Models.Target> Target { get; set; }
        public DbSet<SmartEco.Models.Event> Event { get; set; }
        public DbSet<SmartEco.Models.TargetTerritory> TargetTerritory { get; set; }
        public DbSet<SmartEco.Models.TargetValue> TargetValue { get; set; }
        public DbSet<SmartEco.Models.AActivity> AActivity { get; set; }
        public DbSet<SmartEco.Models.LEDScreen> LEDScreen { get; set; }
        public DbSet<SmartEco.Models.Executor> Executor { get; set; }
        public DbSet<SmartEco.Models.PlantationsType> PlantationsType { get; set; }
        public DbSet<SmartEco.Models.PlantationsStateType> PlantationsStateType { get; set; }
        public DbSet<SmartEco.Models.AuthorizedAuthority> AuthorizedAuthority { get; set; }
        public DbSet<SmartEco.Models.SpeciallyProtectedNaturalTerritory> SpeciallyProtectedNaturalTerritory { get; set; }
        public DbSet<SmartEco.Models.GreemPlantsPassport> GreemPlantsPassport { get; set; }
        public DbSet<SmartEco.Models.TreesByObjectTableOfTheBreedStateList> TreesByObjectTableOfTheBreedStateList { get; set; }
        public DbSet<SmartEco.Models.TreesByFacilityManagementMeasuresList> TreesByFacilityManagementMeasuresList { get; set; }
        public DbSet<SmartEco.Models.PlantationsState> PlantationsState { get; set; }
        public DbSet<SmartEco.Models.SpeciesDiversity> SpeciesDiversity { get; set; }
        public DbSet<SmartEco.Models.Ecopost> Ecopost { get; set; }
        //public DbSet<SmartEco.Models.EcomonMonitoringPoint> EcomonMonitoringPoint { get; set; }
        //public DbSet<SmartEco.Models.Log> Log { get; set; }
    }
}
