using Microsoft.AspNetCore.Mvc.Rendering;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SmartEco.Common.Enums;
using SmartEco.Common.Models.Filters.Directories;
using SmartEco.Web.Models.Auth;

namespace SmartEco.Web.Models.Filters.Directories
{
    public class PersonFilterPager : Pager, IPersonFilter
    {
        public PersonFilterPager(int totalItems, int? page = null, int? pageSize = null)
            : base(totalItems, page, pageSize)
        {
        }

        public string Email { get; set; }
        public int? RoleId { get; set; }
        public string SortOrder { get; set; }

        public string EmailSort { get; set; }
        public string RoleSort { get; set; }
    }

    public record PersonFilterBase(int? PageNumber, int? PageSize, string SortOrder, string Email, int? RoleId) 
        : BaseFilter(PageNumber, PageSize), IPersonFilter
    {
        public PersonFilterBase()
            : this(null, null, null, null, null) { }

        public PersonFilterBase(PersonFilterId personFilterId)
            : this(personFilterId.PageNumber, personFilterId.PageSize, personFilterId.SortOrder, personFilterId.Email, personFilterId.RoleId) { }

        public string Email { get; set; } = Email;
        public int? RoleId { get; set; } = RoleId;
        public string SortOrder { get; set; } = SortOrder;
    }

    public record PersonFilterId : PersonFilterBase, IPersonFilter
    {
        public PersonFilterId(long Id, int? PageNumber, int? PageSize, string SortOrder, string Email, int? RoleId)
            : base(PageNumber, PageSize, SortOrder, Email, RoleId)
        {
            this.SortOrder = SortOrder;
            this.Email = Email;
            this.RoleId = RoleId;
            this.Id = Id;
        }

        public long Id { get; set; }
    }

    public record PersonViewModelFilter(PersonFilterId Filter, PersonViewModel Person, SelectList Roles);
    public record PersonViewModelFilterList(PersonFilterPager Filter, IEnumerable<PersonViewModel> Persons, SelectList Roles);

    public record PersonAuthViewModelFilter(PersonFilterId Filter, PersonAuthViewModel Person, SelectList Roles);
}
