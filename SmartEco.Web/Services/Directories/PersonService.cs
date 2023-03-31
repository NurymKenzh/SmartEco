using Microsoft.AspNetCore.Mvc.Rendering;
using SmartEco.Common.Enums;
using SmartEco.Common.Models.Responses;
using SmartEco.Web.Models.Auth;
using SmartEco.Web.Models.Filters.Directories;
using SmartEco.Web.Services.Providers;

namespace SmartEco.Web.Services.Directories
{
    public class PersonService
    {
        private readonly ISmartEcoApi _smartEcoApi;

        public PersonService(ISmartEcoApi smartEcoApi)
        {
            _smartEcoApi = smartEcoApi;
        }

        public async Task<PersonViewModelFilterList> GetPersons(PersonFilterBase filter)
        {
            var persons = await _smartEcoApi.GetObjectsByFilter<GeneralListResponse<PersonViewModel>>("Persons/Get", filter);

            var filterPager = new PersonFilterPager(persons.CountTotal, filter.PageNumber, filter.PageSize)
            {
                Email = filter.Email,
                RoleId = filter.RoleId,
                SortOrder = filter.SortOrder,
                EmailSort = filter.SortOrder == "Email" ? "EmailDesc" : "Email",
                RoleSort = filter.SortOrder == "Role" ? "RoleDesc" : "Role"
            };

            return new PersonViewModelFilterList(filterPager, persons.Objects, GetSelectListRoles());
        }

        public async Task<PersonViewModelFilter> GetPerson(PersonFilterId filter)
        {
            var person = await _smartEcoApi.GetObjectById<PersonViewModel>("Persons/Get", filter.Id);
            return new PersonViewModelFilter(filter, person, GetSelectListRoles(person.Role));
        }

        public async Task<(bool, PersonAuthViewModelFilter)> CreatePerson(PersonAuthViewModelFilter personAuthViewModelFilter)
        {
            var response = await _smartEcoApi.CreateObject("Persons/Create", personAuthViewModelFilter.Person);
            return (response.IsSuccessStatusCode, personAuthViewModelFilter with
            {
                Roles = GetSelectListRoles(personAuthViewModelFilter.Person.Role)
            });
        }

        public async Task<(bool, PersonViewModelFilter)> UpdatePerson(PersonViewModelFilter personViewModelFilter)
        {
            var response = await _smartEcoApi.UpdateObject("Persons/Update", personViewModelFilter.Person);
            return (response.IsSuccessStatusCode, personViewModelFilter with 
            { 
                Roles = GetSelectListRoles(personViewModelFilter.Person.Role) 
            });
        }

        public async Task<bool> DeletePerson(PersonFilterId filter)
        {
            var response = await _smartEcoApi.DeleteObjectById("Persons/Delete", filter.Id);
            return response.IsSuccessStatusCode;
        }

        public static SelectList GetSelectListRoles(Role? role = null)
            => new(Enum.GetValues(typeof(Role)).Cast<Role>().Select(role => new SelectListItem 
            {
                Text = role.ToString(),
                Value = ((int)role).ToString()
            }).ToList(), "Value", "Text", role);
    }
}
