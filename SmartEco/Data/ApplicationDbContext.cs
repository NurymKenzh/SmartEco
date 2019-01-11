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
        //public DbSet<SmartEco.Models.EcomonMonitoringPoint> EcomonMonitoringPoint { get; set; }
        //public DbSet<SmartEco.Models.Log> Log { get; set; }
    }
}
