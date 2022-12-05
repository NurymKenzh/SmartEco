using SmartEco.Common.Data.Repositories.Abstractions;

namespace SmartEco.API.Services
{
    public class CrudService<Entity> : ICrudService<Entity> where Entity : class
    {
        private readonly ISmartEcoRepository _repository;

        public CrudService(ISmartEcoRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Entity> Get()
            =>  _repository.Get<Entity>();

        public async Task<Entity> Get(long id)
            => await _repository.Get<Entity>(id);

        public async Task Create(Entity entity)
            => await _repository.Create(entity);

        public async Task Update(Entity entity)
            => await _repository.Update(entity);

        public async Task Delete(long id)
            => await _repository.Delete<Entity>(id);
    }

    public interface ICrudService<Entity> where Entity : class
    {
        IEnumerable<Entity> Get();
        Task<Entity> Get(long id);
        Task Create(Entity entity);
        Task Update(Entity entity);
        Task Delete(long id);
    }
}
