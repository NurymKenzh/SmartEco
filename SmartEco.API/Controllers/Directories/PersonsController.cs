using Microsoft.AspNetCore.Mvc;
using SmartEco.API.Helpers.Attributes;
using SmartEco.API.Models.Filters.Directories;
using SmartEco.API.Services;
using SmartEco.API.Services.Directories;
using SmartEco.Common.Data.Entities;
using SmartEco.Common.Enums;
using SmartEco.Common.Models.Responses;

namespace SmartEco.API.Controllers.Directories
{
    [AuthorizeEnum(Role.Admin)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PersonsController : BaseController
    {
        private readonly PersonFilteringService _personFiltering;
        private readonly ICrudService<Person> _crudService;

        public PersonsController(ICrudService<Person> crudService, PersonFilteringService personFiltering)
        {
            _crudService = crudService;
            _personFiltering = personFiltering;
        }

        [HttpPost(nameof(Get))]
        public async Task<GeneralListResponse<Person>> Get(PersonFilter filter)
            => await _personFiltering.GetPersons(filter);

        [HttpGet("Get/{id}")]
        public async Task<Person> Get(long id)
            => await _crudService.Get(id);

        [HttpPost(nameof(Create))]
        public async Task Create(Person person)
            => await _crudService.Create(CreateWithHash(person));

        [HttpPut(nameof(Update))]
        public async Task Update(Person person)
            => await _crudService.Update(await Change(person));

        [HttpDelete("Delete/{id}")]
        public async Task Delete(long id)
            => await _crudService.Delete(id);

        private static Person CreateWithHash(Person person)
        {
            if (person is not null)
            {
                person.PasswordHash = AuthService.GetHash(person.Password);
                return person;
            }
            return null;
        }

        private async Task<Person> Change(Person personChanged)
        {
            var person = await Get(personChanged.Id);
            if (person is not null)
            {
                person.Email = personChanged.Email;
                person.Role = personChanged.Role;
                return person;
            }
            return null;
        }
    }
}
