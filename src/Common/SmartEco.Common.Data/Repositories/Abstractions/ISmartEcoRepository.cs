using System.Linq.Expressions;

namespace SmartEco.Common.Data.Repositories.Abstractions
{
    public interface ISmartEcoRepository
    {
        IQueryable<Entity> GetAll<Entity>() where Entity : class;
        IEnumerable<Entity> Get<Entity>() where Entity : class;
        Task<Entity?> Get<Entity>(long id) where Entity : class;
        Task Create<Entity>(Entity entity) where Entity : class;
        Task Update<Entity>(Entity entity) where Entity : class;
        Task Delete<Entity>(long id) where Entity : class;


        Task<Entity?> GetFirstOrDefault<Entity>(Expression<Func<Entity, bool>> predicate) where Entity : class;
        Task<bool> IsAnyEntity<Entity>(Expression<Func<Entity, bool>> predicate) where Entity : class;
    }
}
