using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models;

namespace SmartEcoAPI.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    //public class PersonsController : ControllerBase
    //{
    //    private readonly ApplicationDbContext _context;

    //    public PersonsController(ApplicationDbContext context)
    //    {
    //        _context = context;
    //    }

    //    // GET: api/Persons
    //    [HttpGet]
    //    public async Task<ActionResult<IEnumerable<Person>>> GetPerson()
    //    {
    //        return await _context.Person.ToListAsync();
    //    }

    //    // GET: api/Persons/5
    //    [HttpGet("{id}")]
    //    public async Task<ActionResult<Person>> GetPerson(int id)
    //    {
    //        var person = await _context.Person.FindAsync(id);

    //        if (person == null)
    //        {
    //            return NotFound();
    //        }

    //        return person;
    //    }

    //    // PUT: api/Persons/5
    //    [HttpPut("{id}")]
    //    public async Task<IActionResult> PutPerson(int id, Person person)
    //    {
    //        if (id != person.Id)
    //        {
    //            return BadRequest();
    //        }

    //        _context.Entry(person).State = EntityState.Modified;

    //        try
    //        {
    //            await _context.SaveChangesAsync();
    //        }
    //        catch (DbUpdateConcurrencyException)
    //        {
    //            if (!PersonExists(id))
    //            {
    //                return NotFound();
    //            }
    //            else
    //            {
    //                throw;
    //            }
    //        }

    //        return NoContent();
    //    }

    //    // POST: api/Persons
    //    [HttpPost]
    //    public async Task<ActionResult<Person>> PostPerson(Person person)
    //    {
    //        _context.Person.Add(person);
    //        await _context.SaveChangesAsync();

    //        return CreatedAtAction("GetPerson", new { id = person.Id }, person);
    //    }

    //    // DELETE: api/Persons/5
    //    [HttpDelete("{id}")]
    //    public async Task<ActionResult<Person>> DeletePerson(int id)
    //    {
    //        var person = await _context.Person.FindAsync(id);
    //        if (person == null)
    //        {
    //            return NotFound();
    //        }

    //        _context.Person.Remove(person);
    //        await _context.SaveChangesAsync();

    //        return person;
    //    }

    //    private bool PersonExists(int id)
    //    {
    //        return _context.Person.Any(e => e.Id == id);
    //    }
    //}
}
