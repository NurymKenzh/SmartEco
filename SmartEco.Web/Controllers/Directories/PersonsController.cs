using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SmartEco.Common.Enums;
using SmartEco.Web.Controllers;
using SmartEco.Web.Helpers.Constants;
using SmartEco.Web.Models.Auth;
using SmartEco.Web.Models.Filters.Directories;
using SmartEco.Web.Services;
using SmartEco.Web.Services.Directories;

namespace SmartEco.Controllers
{
    public class PersonsController : BaseController
    {
        private readonly CrudService _crudService;
        private readonly PersonService _personService;

        public PersonsController(CrudService crudService, PersonService personService)
        {
            _crudService = crudService;
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
                    return RedirectToAction(nameof(Index), personAuthViewModelFilter.Filter);
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
                    return RedirectToAction(nameof(Index), personViewModelFilter.Filter);
            }
            return View(Views.Get(HttpContext, Directories), personViewModelFilter);
        }

        public async Task<IActionResult> Delete(PersonFilterId filter)
        {
            var filterPersons = await _personService.GetPerson(filter);
            return View(Views.Get(HttpContext, Directories), filterPersons);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(PersonFilterId filter)
        {
            await _personService.DeletePerson(filter);
            return RedirectToAction(nameof(Index), filter);
        }
    }
}
