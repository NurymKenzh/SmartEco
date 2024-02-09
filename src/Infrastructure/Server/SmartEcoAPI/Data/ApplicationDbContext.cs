using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Utilities;
using SmartEcoAPI.Models.ASM;
using SmartEcoAPI.Models.ASM.PollutionSources;
using SmartEcoAPI.Models.ASM.Uprza;

namespace SmartEcoAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly string _schemaAsm = "asm";
        private readonly string _schemaAsmUprza = "asmUprza";

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
        public DbSet<RelationBackground> RelationBackground { get; set; }
        public DbSet<AirPollutionSourceIndSite> AirPollutionSourceIndSite { get; set; }
        public DbSet<AirPollutionSourceWorkshop> AirPollutionSourceWorkshop { get; set; }
        public DbSet<AirPollutionSourceArea> AirPollutionSourceArea { get; set; }

        public DbSet<KatoCatalog> KatoCatalog { get; set; }

        public DbSet<OperationMode> OperationMode { get; set; }
        public DbSet<GasAirMixture> GasAirMixture { get; set; }
        public DbSet<HazardLevel> HazardLevel { get; set; }
        public DbSet<AirPollutant> AirPollutant { get; set; }
        public DbSet<AirEmission> AirEmission { get; set; }

        #region UPRZA
        public DbSet<CalculationType> CalculationType { get; set; }
        public DbSet<CalculationStatus> CalculationStatus { get; set; }
        public DbSet<Calculation> Calculation { get; set; }

        public DbSet<CalculationToEnterprise> CalculationToEnterprise { get; set; }
        public DbSet<CalculationToSource> CalculationToSource { get; set; }
        public DbSet<CalculationPoint> CalculationPoint { get; set; }
        public DbSet<CalculationRectangle> CalculationRectangle { get; set; }
        public DbSet<CalculationSetting> CalculationSetting { get; set; }
        public DbSet<StateCalculation> StateCalculation { get; set; }
        public DbSet<ResultEmission> ResultEmission { get; set; }

        #endregion
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
            modelBuilder.Entity<RelationBackground>().ToTable(nameof(RelationBackground), _schemaAsm);
            modelBuilder.Entity<AirPollutionSourceIndSite>().ToTable(nameof(AirPollutionSourceIndSite), _schemaAsm);
            modelBuilder.Entity<AirPollutionSourceWorkshop>().ToTable(nameof(AirPollutionSourceWorkshop), _schemaAsm);
            modelBuilder.Entity<AirPollutionSourceArea>().ToTable(nameof(AirPollutionSourceArea), _schemaAsm);

            modelBuilder.Entity<OperationMode>().ToTable(nameof(OperationMode), _schemaAsm);
            modelBuilder.Entity<GasAirMixture>().ToTable(nameof(GasAirMixture), _schemaAsm);
            modelBuilder.Entity<HazardLevel>().ToTable(nameof(HazardLevel), _schemaAsm);
            modelBuilder.Entity<AirPollutant>().ToTable(nameof(AirPollutant), _schemaAsm);
            modelBuilder.Entity<AirEmission>().ToTable(nameof(AirEmission), _schemaAsm);

            modelBuilder.Entity<KatoCatalog>().ToTable(nameof(KatoCatalog), _schemaAsm);

            //UPRZA
            modelBuilder.Entity<CalculationType>().ToTable(nameof(CalculationType), _schemaAsmUprza);
            modelBuilder.Entity<CalculationStatus>().ToTable(nameof(CalculationStatus), _schemaAsmUprza);
            modelBuilder.Entity<Calculation>().ToTable(nameof(Calculation), _schemaAsmUprza);
            modelBuilder.Entity<CalculationToEnterprise>().ToTable(nameof(CalculationToEnterprise), _schemaAsmUprza);
            modelBuilder.Entity<CalculationToSource>().ToTable(nameof(CalculationToSource), _schemaAsmUprza);
            modelBuilder.Entity<CalculationPoint>().ToTable(nameof(CalculationPoint), _schemaAsmUprza);
            modelBuilder.Entity<CalculationRectangle>().ToTable(nameof(CalculationRectangle), _schemaAsmUprza);
            modelBuilder.Entity<CalculationSetting>().ToTable(nameof(CalculationSetting), _schemaAsmUprza);
            modelBuilder.Entity<StateCalculation>().ToTable(nameof(StateCalculation), _schemaAsmUprza);
            modelBuilder.Entity<ResultEmission>().ToTable(nameof(ResultEmission), _schemaAsmUprza);
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

            //Operation modes and Emissions
            modelBuilder.Entity<GasAirMixture>()
                .HasKey(mode => mode.OperationModeId);

            modelBuilder.Entity<HazardLevel>()
                .HasKey(lvl => lvl.Code);

            //Kato
            modelBuilder.Entity<KatoCatalog>()
                .Property(t => t.Id)
                .ValueGeneratedNever();

            ConfigureUprza(modelBuilder);
        }

        private void ConfigureUprza(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CalculationStatus>()
                .Property(t => t.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<CalculationToEnterprise>()
                .HasKey(c => new 
                { 
                    c.CalculationId,
                    c.EnterpriseId 
                });

            modelBuilder.Entity<CalculationToSource>()
                .HasKey(c => new
                {
                    c.CalculationId,
                    c.SourceId
                });

            modelBuilder.Entity<CalculationPoint>()
                .HasKey(c => new
                {
                    c.CalculationId,
                    c.Number
                });

            modelBuilder.Entity<CalculationRectangle>()
                .HasKey(c => new
                {
                    c.CalculationId,
                    c.Number
                });

            modelBuilder.Entity<CalculationSetting>()
                .HasKey(c => c.CalculationId);

            //Primary Key
            modelBuilder.Entity<StateCalculation>()
                .HasKey(s => s.CalculationId);
            //Complex Types
            modelBuilder.Entity<StateCalculation>()
                .OwnsOne(s => s.DiagnosticInfo);

            modelBuilder.Entity<ResultEmission>()
                .HasKey(c => new
                {
                    c.CalculationId,
                    c.AirPollutantId
                });
        }
    }
}
