using Microsoft.EntityFrameworkCore;
using SmartEcoAPI.Data;
using SmartEcoAPI.Models.ASM.Uprza;
using System.Data;
using System.Threading.Tasks;

namespace SmartEcoAPI.Services
{
    internal class CalculationService : ICalculationService
    {
        private readonly ApplicationDbContext _context;

        public CalculationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task UpdateStatus(int calculationId, CalculationStatuses status)
        {
            var calculation = _context.Calculation.Find(calculationId);
            if (calculation.StatusId == (int)status)
                return;

            calculation.StatusId = (int)status;
            _context.Entry(calculation).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }

    public interface ICalculationService
    {
        Task UpdateStatus(int calculationId, CalculationStatuses status);
    }
}
