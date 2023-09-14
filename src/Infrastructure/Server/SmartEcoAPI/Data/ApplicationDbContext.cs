using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Utilities;
using SmartEcoAPI.Models.ASM;
using SmartEcoAPI.Models.ASM.PollutionSources;

namespace SmartEcoAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly string _schemaAsm = "asm";

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SetSchemaAsm(modelBuilder);
            ConfigureProperties(modelBuilder);
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
        public DbSet<SmartEcoAPI.Models.TreesByObjectTableOfTheBreedStateList> TreesByObjectTableOfTheBreedStateList { get; set; }
        public DbSet<SmartEcoAPI.Models.TreesByFacilityManagementMeasuresList> TreesByFacilityManagementMeasuresList { get; set; }
        public DbSet<SmartEcoAPI.Models.PlantationsState> PlantationsState { get; set; }
        public DbSet<SmartEcoAPI.Models.SpeciesDiversity> SpeciesDiversity { get; set; }

        public DbSet<SmartEcoAPI.Models.Ecopost> Ecopost { get; set; }
        public DbSet<SmartEcoAPI.Models.ReceptionRecyclingPoint> ReceptionRecyclingPoint { get; set; }

        public DbSet<SmartEcoAPI.Models.Question> Question { get; set; }
        public DbSet<SmartEcoAPI.Models.Answer> Answer { get; set; }

        #region ASM
        public DbSet<EnterpriseType> EnterpriseType { get; set; }
        public DbSet<Enterprise> Enterprise{ get; set; }
        public DbSet<KatoEnterprise> KatoEnterprise { get; set; }
        public DbSet<IndSiteEnterprise> IndSiteEnterprise { get; set; }
        public DbSet<Workshop> Workshop { get; set; }
        public DbSet<Area> Area { get; set; }
        public DbSet<IndSiteEnterpriseBorder> IndSiteEnterpriseBorder { get; set; }
        public DbSet<SanZoneEnterpriseBorder> SanZoneEnterpriseBorder { get; set; }

        public DbSet<AirPollutionSourceType> AirPollutionSourceType { get; set; }
        public DbSet<AirPollutionSource> AirPollutionSource { get; set; }
        public DbSet<AirPollutionSourceInfo> AirPollutionSourceInfo { get; set; }
        public DbSet<AirPollutionSourceIndSite> AirPollutionSourceIndSite { get; set; }
        public DbSet<AirPollutionSourceWorkshop> AirPollutionSourceWorkshop { get; set; }
        public DbSet<AirPollutionSourceArea> AirPollutionSourceArea { get; set; }
        #endregion

        private void SetSchemaAsm(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EnterpriseType>().ToTable(nameof(EnterpriseType), _schemaAsm);
            modelBuilder.Entity<Enterprise>().ToTable(nameof(Enterprise), _schemaAsm);
            modelBuilder.Entity<KatoEnterprise>().ToTable(nameof(KatoEnterprise), _schemaAsm);
            modelBuilder.Entity<IndSiteEnterprise>().ToTable(nameof(IndSiteEnterprise), _schemaAsm);
            modelBuilder.Entity<Workshop>().ToTable(nameof(Workshop), _schemaAsm);
            modelBuilder.Entity<Area>().ToTable(nameof(Area), _schemaAsm);
            modelBuilder.Entity<IndSiteEnterpriseBorder>().ToTable(nameof(IndSiteEnterpriseBorder), _schemaAsm);
            modelBuilder.Entity<SanZoneEnterpriseBorder>().ToTable(nameof(SanZoneEnterpriseBorder), _schemaAsm);

            modelBuilder.Entity<AirPollutionSourceType>().ToTable(nameof(AirPollutionSourceType), _schemaAsm);
            modelBuilder.Entity<AirPollutionSource>().ToTable(nameof(AirPollutionSource), _schemaAsm);
            modelBuilder.Entity<AirPollutionSourceInfo>().ToTable(nameof(AirPollutionSourceInfo), _schemaAsm);
            modelBuilder.Entity<AirPollutionSourceIndSite>().ToTable(nameof(AirPollutionSourceIndSite), _schemaAsm);
            modelBuilder.Entity<AirPollutionSourceWorkshop>().ToTable(nameof(AirPollutionSourceWorkshop), _schemaAsm);
            modelBuilder.Entity<AirPollutionSourceArea>().ToTable(nameof(AirPollutionSourceArea), _schemaAsm);
        }

        private void ConfigureProperties(ModelBuilder modelBuilder)
        {
            //AirPollutionSource
            modelBuilder.Entity<AirPollutionSource>()
                .Property(a => a.Number)
                .HasMaxLength(4)
                .IsFixedLength()
                .IsRequired()
                .IsNumeric();

            modelBuilder.Entity<AirPollutionSource>()
                .Property(a => a.Name)
                .HasMaxLength(255);

            modelBuilder.Entity<AirPollutionSourceInfo>()
                .HasKey(a => a.SourceId);

            //AirPollutionSource Relations
            modelBuilder.Entity<AirPollutionSourceIndSite>()
                .HasKey(a => a.AirPollutionSourceId);

            modelBuilder.Entity<AirPollutionSourceWorkshop>()
                .HasKey(a => a.AirPollutionSourceId);

            modelBuilder.Entity<AirPollutionSourceArea>()
                .HasKey(a => a.AirPollutionSourceId);
        }
    }
}
