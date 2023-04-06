using Microsoft.AspNetCore.Mvc.Rendering;
using SmartEco.Common.Models.Filters.Directories;
using SmartEco.Web.Models.Auth;

namespace SmartEco.Web.Models.Filters.Directories
{
    //Filter with additional sorting fields and Pager for 'Index'
    public class PersonFilterPager : Pager, IPersonFilter
    {
        public PersonFilterPager(int totalItems, int? page = null, int? pageSize = null)
            : base(totalItems, page, pageSize) { }

        public string? Email { get; set; }
        public int? RoleId { get; set; }
        public string? SortOrder { get; set; }

        public string? EmailSort { get; set; }
        public string? RoleSort { get; set; }
    }

    //Basic filter for all pages
    //Required to send filtering to the server
    public record PersonFilterBase: BaseFilter, IPersonFilter
    {
        //To initialize empty object for controller
        public PersonFilterBase() { }

        public PersonFilterBase(int? PageNumber, int? PageSize, string? SortOrder, string? Email, int? RoleId)
            : base(PageNumber, PageSize) 
        {
            this.SortOrder = SortOrder;
            this.RoleId = RoleId;
            this.Email = Email;
        }

        public PersonFilterBase(PersonFilterId personFilterId)
            : this(personFilterId.PageNumber, personFilterId.PageSize, personFilterId.SortOrder, personFilterId.Email, personFilterId.RoleId) { }

        public string? Email { get; set; }
        public int? RoleId { get; set; }
        public string? SortOrder { get; set; }
    }

    //Filter with additional field 'Id'
    //Required to get the item after click 'Create', 'Edit', 'Delete'
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

    //To display list users in 'Index'
    public record PersonViewModelFilterList(PersonFilterPager Filter, IEnumerable<PersonViewModel> Persons, SelectList Roles);

    //To create new user in system. Used model with passwords
    public record PersonAuthViewModelFilter(PersonFilterId Filter, PersonAuthViewModel? Person, SelectList Roles);
}
