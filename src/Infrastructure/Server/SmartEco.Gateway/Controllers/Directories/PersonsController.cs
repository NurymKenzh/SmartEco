using Microsoft.AspNetCore.Mvc;
using SmartEco.Gateway.Helpers;
using SmartEco.Gateway.Helpers.Attributes;
using SmartEco.Gateway.Models.Filters.Directories;
using SmartEco.Gateway.Services;
using SmartEco.Gateway.Services.Directories;
using SmartEco.Common.Data.Entities;
using SmartEco.Common.Data.Repositories.Abstractions;
using SmartEco.Common.Enums;
using SmartEco.Common.Models.Responses;

namespace SmartEco.Gateway.Controllers.Directories
{
    [AuthorizeEnum(Role.Admin)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PersonsController : BaseController
    {
        private readonly PersonFilteringService _personFiltering;
        private readonly ICrudService<Person> _crudService;
        private readonly ISmartEcoRepository _repository;

        public PersonsController(ICrudService<Person> crudService, PersonFilteringService personFiltering, ISmartEcoRepository repository)
        {
            _crudService = crudService;
            _personFiltering = personFiltering;
            _repository = repository;
        }

        [HttpPost(nameof(Get))]
        public async Task<GeneralListResponse<Person>> Get(PersonFilter filter)
            => await _personFiltering.GetPersons(filter);

        [HttpGet("Get/{id}")]
        public async Task<ActionResult<Person?>> Get(long id)
            => await _crudService.Get(id);

        [HttpPost(nameof(Create))]
        public async Task<ActionResult> Create(Person person)
            => await _crudService.Create(CreateWithHash(person));

        [HttpPut(nameof(Update))]
        public async Task<ActionResult> Update(Person person)
            => await Change(person);

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> Delete(long id)
            => await _crudService.Delete(id);

        private static Person CreateWithHash(Person person)
        {
            ArgumentNullException.ThrowIfNull(person, nameof(person));
            person.PasswordHash = PasswordHasher.GetHash(person.Password!);
            return person;
        }

        private async Task<ActionResult> Change(Person personChanged)
        {
            ArgumentNullException.ThrowIfNull(personChanged, nameof(personChanged));

            var person = await _repository.Get<Person>(personChanged.Id);
            if (person is null)
                return new StatusCodeResult(StatusCodes.Status404NotFound);

            person.Email = personChanged.Email;
            person.Role = personChanged.Role;
            return await _crudService.Update(person.Id, person);
        }
    }
}
