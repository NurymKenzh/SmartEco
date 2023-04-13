using Microsoft.AspNetCore.Mvc;
using SmartEco.Web.Controllers;
using SmartEco.Web.Helpers.Constants;
using SmartEco.Web.Models.Filters.Directories;
using SmartEco.Web.Services.Directories;

namespace SmartEco.Controllers
{
    public class PersonsController : BaseController
    {
        private readonly PersonService _personService;

        public PersonsController(PersonService personService)
        {
            _personService = personService;
        }

        public async Task<IActionResult> Index(PersonFilterBase filter)
        {
            var filterPersons = await _personService.GetPersons(filter);
            return View(Views.Get(HttpContext, Directories), filterPersons);
        }

        public async Task<IActionResult> Details(PersonFilterId filter)
        {
            var filterPersons = await _personService.GetPerson(filter);
            return View(Views.Get(HttpContext, Directories), filterPersons);
        }

        public IActionResult Create(PersonFilterId filter)
        {
            return View(
                Views.Get(HttpContext, Directories), 
                new PersonAuthViewModelFilter(filter, null, PersonService.GetSelectListRoles()));
        }

        [HttpPost]
        public async Task<IActionResult> Create(PersonAuthViewModelFilter personAuthViewModelFilter)
        {
            if (ModelState.IsValid)
            {
                (var success, personAuthViewModelFilter) = await _personService.CreatePerson(personAuthViewModelFilter);
                if (success)
                    return RedirectToAction(nameof(Index), new PersonFilterBase(personAuthViewModelFilter.Filter));
            }
            return View(Views.Get(HttpContext, Directories), personAuthViewModelFilter);
        }

        public async Task<IActionResult> Edit(PersonFilterId filter)
        {
            var filterPersons = await _personService.GetPerson(filter);
            return View(Views.Get(HttpContext, Directories), filterPersons);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PersonViewModelFilter personViewModelFilter)
        {
            if (ModelState.IsValid)
            {
                (var success, personViewModelFilter) = await _personService.UpdatePerson(personViewModelFilter);
                if (success)
                    return RedirectToAction(nameof(Index), new PersonFilterBase(personViewModelFilter.Filter));
            }
            return View(Views.Get(HttpContext, Directories), personViewModelFilter);
        }

        public async Task<IActionResult> Delete(PersonFilterId filter)
        {
            var filterPersons = await _personService.GetPerson(filter);
            return View(Views.Get(HttpContext, Directories), filterPersons);
        }

        [HttpPost, ActionName(nameof(Delete))]
        public async Task<IActionResult> DeleteConfirmed(PersonFilterId filter)
        {
            await _personService.DeletePerson(filter);
            return RedirectToAction(nameof(Index), new PersonFilterBase(filter));
        }
    }
}
