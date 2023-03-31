using Microsoft.EntityFrameworkCore;
using SmartEco.API.Models.Filters.Directories;
using SmartEco.Common.Data.Entities;
using SmartEco.Common.Data.Repositories.Abstractions;
using SmartEco.Common.Models.Responses;

namespace SmartEco.API.Services.Directories
{
    public class PersonFilteringService
    {
        private readonly ISmartEcoRepository _repository;

        public PersonFilteringService(ISmartEcoRepository repository)
        {
            _repository = repository;
        }

        public async Task<GeneralListResponse<Person>> GetPersons(PersonFilter filter)
        {
            var persons = _repository.GetAll<Person>();

            if (!string.IsNullOrEmpty(filter.Email))
            {
                persons = persons.Where(m => m.Email.ToLower().Contains(filter.Email.ToLower()));
            }
            if (filter.RoleId != null)
            {
                persons = persons.Where(m => m.RoleId == filter.RoleId);
            }

            persons = filter.SortOrder switch
            {
                "Email" => persons.OrderBy(m => m.Email),
                "EmailDesc" => persons.OrderByDescending(m => m.Email),
                "Role" => persons.OrderBy(m => m.Role),
                "RoleDesc" => persons.OrderByDescending(m => m.Role),
                _ => persons.OrderBy(m => m.Id),
            };

            var count = await persons.CountAsync();
            if (filter.PageSize != null && filter.PageNumber != null)
            {
                var pageSize = (int)filter.PageSize;
                var currentPage = (int)filter.PageNumber;
                persons = persons.Skip((currentPage - 1) * pageSize).Take(pageSize);
            }

            return new GeneralListResponse<Person>(await persons.ToListAsync(), count);
        }
    }
}
