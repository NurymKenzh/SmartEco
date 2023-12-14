using Microsoft.EntityFrameworkCore;
using SmartEco.Common.Data.Entities.SmartEcoApi;

namespace SmartEco.Common.Data.Contexts
{
    public class SmartEcoApiDbContext : DbContext
    {
        public SmartEcoApiDbContext(DbContextOptions<SmartEcoApiDbContext> options)
            : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<MeasuredData> MeasuredData { get; set; }
        public DbSet<MeasuredParameter> MeasuredParameter { get; set; }
        public DbSet<MonitoringPost> MonitoringPost { get; set; }
        public DbSet<MonitoringPostMeasuredParameters> MonitoringPostMeasuredParameters { get; set; }
        public DbSet<Person> Person { get; set; }
    }
}
