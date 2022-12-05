using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartEco.Common.Data.Contexts;
using SmartEco.Common.Data.Repositories;
using SmartEco.Common.Data.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartEco.Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterRepository(this IServiceCollection services)
        {
            services.AddScoped<ISmartEcoRepository, SmartEcoRepository>(sp =>
            {
                var dbContext = sp.GetRequiredService<SmartEcoDbContext>();
                if (dbContext == null)
                {
                    throw new ArgumentNullException(nameof(dbContext));
                }
                return new SmartEcoRepository(dbContext);
            });
        }
    }
}
