using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models;

namespace SmartEcoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class KATOesController : ControllerBase
    {
        //private readonly ApplicationDbContext _context;

        //public KATOesController(ApplicationDbContext context)
        //{
        //    _context = context;
        //}

        //// GET: KATOes
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.KATO.ToListAsync());
        //}

        //// GET: KATOes/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var kATO = await _context.KATO
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (kATO == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(kATO);
        //}

        //// GET: KATOes/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: KATOes/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Code,Level,AreaType,EgovId,ParentEgovId,NameKK,NameRU")] KATO kATO)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(kATO);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(kATO);
        //}

        //// GET: KATOes/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var kATO = await _context.KATO.FindAsync(id);
        //    if (kATO == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(kATO);
        //}

        //// POST: KATOes/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Level,AreaType,EgovId,ParentEgovId,NameKK,NameRU")] KATO kATO)
        //{
        //    if (id != kATO.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(kATO);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!KATOExists(kATO.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(kATO);
        //}

        //// GET: KATOes/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var kATO = await _context.KATO
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (kATO == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(kATO);
        //}

        //// POST: KATOes/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var kATO = await _context.KATO.FindAsync(id);
        //    _context.KATO.Remove(kATO);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool KATOExists(int id)
        //{
        //    return _context.KATO.Any(e => e.Id == id);
        //}

        private readonly ApplicationDbContext _context;

        public KATOesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/KATOes
        [HttpGet]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<IEnumerable<KATO>>> GetKATO(string SortOrder,
            string Code,
            int? Level,
            string NameKK,
            string NameRU,
            int? ParentEgovId,
            int? PageSize,
            int? PageNumber)
        {
            var KATOes = _context.KATO
                .Where(k => true);

            if (!string.IsNullOrEmpty(Code))
            {
                KATOes = KATOes.Where(m => m.Code.ToLower().Contains(Code.ToLower()));
            }
            if (Level != null)
            {
                KATOes = KATOes.Where(k => k.Level == Level);
            }
            if (!string.IsNullOrEmpty(NameKK))
            {
                KATOes = KATOes.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                KATOes = KATOes.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (ParentEgovId != null)
            {
                KATOes = KATOes.Where(k => k.ParentEgovId == ParentEgovId);
            }

            switch (SortOrder)
            {
                case "Code":
                    KATOes = KATOes.OrderBy(k => k.Code);
                    break;
                case "CodeDesc":
                    KATOes = KATOes.OrderByDescending(k => k.Code);
                    break;
                case "Level":
                    KATOes = KATOes.OrderBy(k => k.Level);
                    break;
                case "LevelDesc":
                    KATOes = KATOes.OrderByDescending(k => k.Level);
                    break;
                case "NameKK":
                    KATOes = KATOes.OrderBy(m => m.NameKK);
                    break;
                case "NameKKDesc":
                    KATOes = KATOes.OrderByDescending(m => m.NameKK);
                    break;
                case "NameRU":
                    KATOes = KATOes.OrderBy(m => m.NameRU);
                    break;
                case "NameRUDesc":
                    KATOes = KATOes.OrderByDescending(m => m.NameRU);
                    break;
                default:
                    KATOes = KATOes.OrderBy(k => k.Id);
                    break;
            }

            if (PageSize != null && PageNumber != null)
            {
                KATOes = KATOes.Skip(((int)PageNumber - 1) * (int)PageSize).Take((int)PageSize);
            }

            return await KATOes.ToListAsync();
        }

        // GET: api/KATOes/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<KATO>> GetKATO(int id)
        {
            var KATO = await _context.KATO.FindAsync(id);

            if (KATO == null)
            {
                return NotFound();
            }

            return KATO;
        }

        // PUT: api/KATOes/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<IActionResult> PutKATO(int id, KATO KATO)
        {
            if (id != KATO.Id)
            {
                return BadRequest();
            }

            _context.Entry(KATO).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KATOExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/KATOes
        [HttpPost]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<KATO>> PostKATO(KATO KATO)
        {
            _context.KATO.Add(KATO);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetKATO", new { id = KATO.Id }, KATO);
        }

        // DELETE: api/KATOes/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<KATO>> DeleteKATO(int id)
        {
            var KATO = await _context.KATO.FindAsync(id);
            if (KATO == null)
            {
                return NotFound();
            }

            _context.KATO.Remove(KATO);
            await _context.SaveChangesAsync();

            return KATO;
        }

        private bool KATOExists(int id)
        {
            return _context.KATO.Any(e => e.Id == id);
        }

        // GET: api/KATOes/Count
        [HttpGet("count")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<IEnumerable<KATO>>> GetKATOCount(string Code,
            int? Level,
            string NameKK,
            string NameRU,
            int? ParentEgovId)
        {
            var KATOes = _context.KATO
                .Where(k => true);

            if (!string.IsNullOrEmpty(Code))
            {
                KATOes = KATOes.Where(m => m.Code.ToLower().Contains(Code.ToLower()));
            }
            if (Level != null)
            {
                KATOes = KATOes.Where(k => k.Level == Level);
            }
            if (!string.IsNullOrEmpty(NameKK))
            {
                KATOes = KATOes.Where(m => m.NameKK.ToLower().Contains(NameKK.ToLower()));
            }
            if (!string.IsNullOrEmpty(NameRU))
            {
                KATOes = KATOes.Where(m => m.NameRU.ToLower().Contains(NameRU.ToLower()));
            }
            if (ParentEgovId != null)
            {
                KATOes = KATOes.Where(k => k.ParentEgovId == ParentEgovId);
            }

            int count = await KATOes.CountAsync();

            return Ok(count);
        }

        // GET: api/KATOes/5
        [HttpGet("GetByCode/{code}")]
        [Authorize(Roles = "admin,moderator")]
        public async Task<ActionResult<KATO>> GetKATOByCode(string code)
        {
            var KATO = await _context.KATO.FirstOrDefaultAsync(k => k.Code == code);

            if (KATO == null)
            {
                return NotFound();
            }

            return KATO;
        }
    }
}
