using Microsoft.EntityFrameworkCore;
using SmartEco.Common.Data.Contexts;
using SmartEco.Common.Data.Repositories.Abstractions;
using System.Linq.Expressions;

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

        public async Task<Entity?> Get<Entity>(long id) where Entity : class
            => await _context.Set<Entity>().FindAsync(id);

        public async Task Create<Entity>(Entity entity) where Entity : class
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));

            _context.Set<Entity>().Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update<Entity>(Entity entity) where Entity : class
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));

            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task Delete<Entity>(long id) where Entity : class
        {
            var entity = await _context.Set<Entity>().FindAsync(id);
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));

            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }
        #endregion


        public async Task<Entity?> GetFirstOrDefault<Entity>(Expression<Func<Entity, bool>> predicate) where Entity : class
            => await _context.Set<Entity>().FirstOrDefaultAsync(predicate);

        public async Task<bool> IsAnyEntity<Entity>(Expression<Func<Entity, bool>> predicate) where Entity : class
            => await _context.Set<Entity>().AnyAsync(predicate);
    }
}
