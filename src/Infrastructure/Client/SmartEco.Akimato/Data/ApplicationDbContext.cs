using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartEco.Akimato.Data;
using SmartEco.Akimato.Models;

namespace SmartEco.Akimato.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<SmartEco.Akimato.Models.Pollutant> Pollutant { get; set; }
        public DbSet<SmartEco.Akimato.Models.PollutionSource> PollutionSource { get; set; }
        public DbSet<SmartEco.Akimato.Models.PollutionSourceData> PollutionSourceData { get; set; }
        public DbSet<SmartEco.Akimato.Models.KazHydrometAirPost> KazHydrometAirPost { get; set; }
        public DbSet<SmartEco.Akimato.Models.KazHydrometSoilPost> KazHydrometSoilPost { get; set; }
        public DbSet<SmartEco.Akimato.Models.KATO> KATO { get; set; }
        public DbSet<SmartEco.Akimato.Models.PollutionEnvironment> PollutionEnvironment { get; set; }
        public DbSet<SmartEco.Akimato.Models.DataProvider> DataProvider { get; set; }
        public DbSet<SmartEco.Akimato.Models.MonitoringPost> MonitoringPost { get; set; }
        public DbSet<SmartEco.Akimato.Models.Project> Project { get; set; }
        public DbSet<SmartEco.Akimato.Models.TerritoryType> TerritoryType { get; set; }
        public DbSet<SmartEco.Akimato.Models.Target> Target { get; set; }
        public DbSet<SmartEco.Akimato.Models.Event> Event { get; set; }
        public DbSet<SmartEco.Akimato.Models.TargetTerritory> TargetTerritory { get; set; }
        public DbSet<SmartEco.Akimato.Models.TargetValue> TargetValue { get; set; }
        public DbSet<SmartEco.Akimato.Models.AActivity> AActivity { get; set; }
        public DbSet<SmartEco.Akimato.Models.LEDScreen> LEDScreen { get; set; }
        public DbSet<SmartEco.Akimato.Models.Executor> Executor { get; set; }
        public DbSet<SmartEco.Akimato.Models.PlantationsType> PlantationsType { get; set; }
        public DbSet<SmartEco.Akimato.Models.PlantationsStateType> PlantationsStateType { get; set; }
        public DbSet<SmartEco.Akimato.Models.AuthorizedAuthority> AuthorizedAuthority { get; set; }
        public DbSet<SmartEco.Akimato.Models.SpeciallyProtectedNaturalTerritory> SpeciallyProtectedNaturalTerritory { get; set; }
        public DbSet<SmartEco.Akimato.Models.GreemPlantsPassport> GreemPlantsPassport { get; set; }
        public DbSet<SmartEco.Akimato.Models.TreesByObjectTableOfTheBreedStateList> TreesByObjectTableOfTheBreedStateList { get; set; }
        public DbSet<SmartEco.Akimato.Models.TreesByFacilityManagementMeasuresList> TreesByFacilityManagementMeasuresList { get; set; }
        public DbSet<SmartEco.Akimato.Models.PlantationsState> PlantationsState { get; set; }
        public DbSet<SmartEco.Akimato.Models.SpeciesDiversity> SpeciesDiversity { get; set; }
        public DbSet<SmartEco.Akimato.Models.Ecopost> Ecopost { get; set; }
        public DbSet<SmartEco.Akimato.Models.ReceptionRecyclingPoint> ReceptionRecyclingPoint { get; set; }
        //public DbSet<SmartEco.Akimato.Models.EcomonMonitoringPoint> EcomonMonitoringPoint { get; set; }
        //public DbSet<SmartEco.Akimato.Models.Log> Log { get; set; }
    }
}
