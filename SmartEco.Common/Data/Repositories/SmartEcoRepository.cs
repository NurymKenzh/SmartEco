using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SmartEco.Common.Data.Contexts;
using SmartEco.Common.Data.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SmartEco.Common.Data.Repositories
{
    public class SmartEcoRepository : ISmartEcoRepository
    {
        private readonly SmartEcoDbContext _context;

        public SmartEcoRepository(SmartEcoDbContext context)
        {
            _context = context;
        }

        #region Base generic CRUD methods
        public IQueryable<Entity> GetAll<Entity>() where Entity : class
            => _context.Set<Entity>().AsQueryable();

        public IEnumerable<Entity> Get<Entity>() where Entity : class
            => _context.Set<Entity>().AsQueryable();

        public async Task<Entity> Get<Entity>(long id) where Entity : class
            => await _context.Set<Entity>().FindAsync(id);

        public async Task Create<Entity>(Entity entity) where Entity : class
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            _context.Set<Entity>().Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update<Entity>(Entity entity) where Entity : class
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task Delete<Entity>(long id) where Entity : class
        {
            var entity = await _context.Set<Entity>().FindAsync(id);
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }
        #endregion


        public async Task<Entity> GetFirstOrDefault<Entity>(Expression<Func<Entity, bool>> predicate) where Entity : class
            => await _context.Set<Entity>().FirstOrDefaultAsync(predicate);
    }
}
