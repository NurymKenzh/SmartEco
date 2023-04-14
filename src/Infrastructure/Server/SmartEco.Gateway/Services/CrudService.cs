using Microsoft.AspNetCore.Mvc;
using SmartEco.Common.Data.Repositories.Abstractions;

namespace SmartEco.Gateway.Services
{
    public class CrudService<Entity> : ICrudService<Entity> where Entity : class
    {
        private readonly ISmartEcoRepository _repository;

        public CrudService(ISmartEcoRepository repository)
        {
            _repository = repository;
        }

        public async Task<ActionResult> Get(long id)
        {
            try
            {
                var entity = await _repository.Get<Entity>(id);
                if(entity is null)
                    return new StatusCodeResult(StatusCodes.Status204NoContent);

                return new ObjectResult(entity);
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ActionResult> Create(Entity entity)
        {
            try
            {
                await _repository.Create(entity);
                return new StatusCodeResult(StatusCodes.Status201Created);
            }
            catch (ArgumentNullException)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            catch(Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ActionResult> Update(long id, Entity entity)
        {
            try
            {
                var entityDb = await _repository.Get<Entity>(id);
                if(entityDb is null)
                    return new StatusCodeResult(StatusCodes.Status404NotFound);

                await _repository.Update(entity);
                return new StatusCodeResult(StatusCodes.Status202Accepted);
            }
            catch (ArgumentNullException)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                var entityDb = await _repository.Get<Entity>(id);
                if (entityDb is null)
                    return new StatusCodeResult(StatusCodes.Status404NotFound);

                await _repository.Delete<Entity>(id);
                return new StatusCodeResult(StatusCodes.Status202Accepted);
            }
            catch (ArgumentNullException)
            {
                return new StatusCodeResult(StatusCodes.Status400BadRequest);
            }
            catch
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }

    public interface ICrudService<Entity> where Entity : class
    {
        Task<ActionResult> Get(long id);
        Task<ActionResult> Create(Entity entity);
        Task<ActionResult> Update(long id, Entity entity);
        Task<ActionResult> Delete(long id);
    }
}
