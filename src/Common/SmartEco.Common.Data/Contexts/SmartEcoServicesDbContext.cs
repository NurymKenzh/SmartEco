using Microsoft.EntityFrameworkCore;
using SmartEco.Common.Data.Entities.SmartEcoServices;
using SmartEco.Common.Enums;
using System.Reflection.Emit;

namespace SmartEco.Common.Data.Contexts
{
    public class SmartEcoServicesDbContext : DbContext
    {
        private readonly string _schemaReporter = "reporter";

        public SmartEcoServicesDbContext(DbContextOptions<SmartEcoServicesDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SetSchema(modelBuilder);
            ConfigureProperties(modelBuilder);
        }

        public DbSet<WorkerLastStart> WorkerLastStart { get; set; }
        public DbSet<SendEmailEventLog> SendEmailEventLog { get; set; }

        private void SetSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SendEmailEventLog>().ToTable(nameof(SendEmailEventLog), _schemaReporter);
        }

        private static void ConfigureProperties(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<WorkerLastStart>()
                .Property(worker => worker.WorkerType)
                .HasConversion(role => role.ToString(), role => (WorkerType)Enum.Parse(typeof(WorkerType), role));
            modelBuilder
                .Entity<WorkerLastStart>()
                .HasKey(worker => worker.WorkerType);

            modelBuilder.Entity<SendEmailEventLog>()
                .HasNoKey();
        }
    }
}
