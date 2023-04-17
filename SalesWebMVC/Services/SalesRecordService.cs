using Microsoft.EntityFrameworkCore;
using SalesWebMVC.Data;
using SalesWebMVC.Models;

namespace SalesWebMVC.Services
{
    public class SalesRecordService
    {
        private readonly SalesWebMVCContext _context;

        public SalesRecordService(SalesWebMVCContext context)
        {
            _context = context;
        }

        public async Task<SalesRecord> FindByIdAsync(int id)
        {
            return _context.SalesRecord.First(x => x.Id == id);
        }

        public async Task<List<SalesRecord>> FindAllSales(int id)
        {
            return _context.SalesRecord
                .Where(x => x.Seller.Id == id)
                .OrderBy(x => x.Status)
                .ToList();
          
        }

        public async Task<SalesRecord> FindFirstBySellerIdAsync(int id)
        {
             return _context.SalesRecord
                .Where(x => x.Seller.Id == id)
                .First();
        }
        public async Task<List<SalesRecord>> FindByDateAsync(DateTime? minDate, DateTime? maxDate)
        {
            var result = from obj in _context.SalesRecord select obj;
            if (minDate.HasValue)
            {
                result = result.Where(x => x.Date >= minDate.Value);
            }
            if (maxDate.HasValue)
            {
                result = result.Where(x => x.Date <= maxDate.Value);
            }
            return await result
                .Include(x => x.Seller)
                .Include(x => x.Seller.Department)
                .OrderByDescending(x => x.Date)
                .ToListAsync();
        }

        public async Task<List<IGrouping<Department,SalesRecord>>> FindByDateGroupingAsync(DateTime? minDate, DateTime? maxDate)
        {
            var result = from obj in _context.SalesRecord select obj;
            if (minDate.HasValue)
            {
                result = result.Where(x => x.Date >= minDate.Value);
            }
            if (maxDate.HasValue)
            {
                result = result.Where(x => x.Date <= maxDate.Value);
            }
            return  result
                .Include(x => x.Seller)
                .Include(x => x.Seller.Department)
                .OrderByDescending(x => x.Date)
                .AsEnumerable()
                .GroupBy(x => x.Seller.Department)
                .ToList();
        }

        public SalesRecord Any(ICollection<SalesRecord> sales, int id)
        {
            foreach (SalesRecord venda in sales)
            {
                if (venda.Id == id)
                {
                    return venda;
                }
            }
            return null;
        }

        public async Task RemoveAsync(int id)
        {
            var obj = await _context.SalesRecord.FindAsync(id);
            _context.SalesRecord.Remove(obj);
            await _context.SaveChangesAsync();
        }
    }
}
