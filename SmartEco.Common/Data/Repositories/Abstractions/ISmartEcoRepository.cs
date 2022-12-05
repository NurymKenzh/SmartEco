using Microsoft.EntityFrameworkCore;
using SmartEco.Common.Data.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SmartEco.Common.Data.Repositories.Abstractions
{
    public interface ISmartEcoRepository
    {
        Task<(IQueryable<Entity>, int)> GetAll<Entity>() where Entity : class;
        IEnumerable<Entity> Get<Entity>() where Entity : class;
        Task<Entity> Get<Entity>(long id) where Entity : class;
        Task Create<Entity>(Entity entity) where Entity : class;
        Task Update<Entity>(Entity entity) where Entity : class;
        Task Delete<Entity>(long id) where Entity : class;


        Task<Entity> GetFirstOrDefault<Entity>(Expression<Func<Entity, bool>> predicate) where Entity : class;
    }
}
