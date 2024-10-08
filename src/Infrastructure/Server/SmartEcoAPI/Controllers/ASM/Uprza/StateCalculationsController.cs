﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models.ASM.Uprza;
using SmartEcoAPI.Services;

namespace SmartEcoAPI.Controllers.ASM.Uprza
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class StateCalculationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICalculationService _calcService;

        public StateCalculationsController(ApplicationDbContext context, ICalculationService calcService)
        {
            _context = context;
            _calcService = calcService;
        }

        // GET: api/StateCalculations
        [HttpGet("{calculationId}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult<StateCalculation>> GetStateCalculation(int calculationId)
            => await _context.StateCalculation
                .Include(s => s.Calculation.Status)
                .Where(s => s.CalculationId == calculationId)
                .FirstOrDefaultAsync();

        // POST: api/StateCalculations
        [HttpPost("{calculationId}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult> CreateOrUpdateStateCalculation(int calculationId, StateCalculation stateCalc)
        {
            if (calculationId != stateCalc.CalculationId)
                return BadRequest();

            _context.Entry(stateCalc.Calculation).State = EntityState.Modified;
            if (await _context.StateCalculation.AnyAsync(s => s.CalculationId == calculationId))
                _context.Update(stateCalc);
            else
                _context.StateCalculation.Add(stateCalc);

            await DeleteAllResultEmissions(calculationId);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/StateCalculations
        [HttpDelete("{calculationId}")]
        [Authorize(Roles = "admin,moderator,ASM")]
        public async Task<ActionResult> DeleteStateCalculation(int calculationId)
        {
            var stateCalc = await _context.StateCalculation.FindAsync(calculationId);
            if (stateCalc is null)
                return NotFound();

            _context.StateCalculation.Remove(stateCalc);
            await DeleteAllResultEmissions(calculationId);
            await _context.SaveChangesAsync();
            await _calcService.UpdateStatus(calculationId, CalculationStatuses.Configuration);

            return NoContent();
        }

        private async Task DeleteAllResultEmissions(int calculationId)
        {
            if (await _context.ResultEmission.AnyAsync(r => r.CalculationId == calculationId))
                _context.ResultEmission.RemoveRange(_context.ResultEmission.Where(r => r.CalculationId == calculationId));
        }
    }
}