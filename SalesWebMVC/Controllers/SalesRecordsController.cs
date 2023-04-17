using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using SalesWebMVC.Data;
using SalesWebMVC.Models;
using SalesWebMVC.Models.ViewModels;
using SalesWebMVC.Services;
using System.Diagnostics;

namespace SalesWebMVC.Controllers
{
    public class SalesRecordsController : Controller
    {
        private readonly SalesRecordService _salesRecordService;
        private readonly SellerService _sellerService;
        private readonly SalesWebMVCContext _context;
        public SalesRecordsController(SalesRecordService salesRecordService, SellerService sellerService, SalesWebMVCContext context)
        {
            _salesRecordService = salesRecordService;
            _sellerService = sellerService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SimpleSearch(DateTime? minDate, DateTime? maxDate)
        {
            if(!minDate.HasValue)
            {
                minDate = new DateTime(DateTime.Now.Year, 1, 1);
            }
            if(!maxDate.HasValue)
            {
                maxDate = DateTime.Now;
            }
            ViewData["minDate"] = minDate.Value.ToString("yyyy-MM-dd");
            ViewData["maxDate"] = maxDate.Value.ToString("yyyy-MM-dd"); 
            var result = await _salesRecordService.FindByDateAsync(minDate, maxDate);
            return View(result);
        }

        public async Task<IActionResult> GroupingSearch(DateTime? minDate, DateTime? maxDate)
        {
            if (!minDate.HasValue)
            {
                minDate = new DateTime(DateTime.Now.Year, 1, 1);
            }
            if (!maxDate.HasValue)
            {
                maxDate = DateTime.Now;
            }
            ViewData["minDate"] = minDate.Value.ToString("yyyy-MM-dd");
            ViewData["maxDate"] = maxDate.Value.ToString("yyyy-MM-dd");
            var result = await _salesRecordService.FindByDateGroupingAsync(minDate, maxDate);
            return View(result);
        }

        public async  Task<IActionResult> AllSales(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not provided" });

            }

            var allSales = await _salesRecordService.FindAllSales(id.Value);
            var sellerSales = new SalesRecordsFormViewModel { SellerId = id.Value, AllSales = allSales };
            return View(sellerSales);
        }

        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not provided" });

            }
            var allSales = await _salesRecordService.FindAllSales(id.Value);
            var sellerSales = new SalesRecordsFormViewModel { SellerId = id.Value, AllSales = allSales };
            return View(sellerSales);
        }

        public IActionResult Error(string message)
        {
            var viewModel = new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            SalesRecord saleRef = await _salesRecordService.FindByIdAsync(id);
            List<Seller> allSellers = await _sellerService.FindAllAsync();
            Seller theTrueSeller = _sellerService.Any(allSellers, id);
            SalesRecord theTrueSale = _salesRecordService.Any(theTrueSeller.Sales, id);
            return View(theTrueSale);
        }

        public async Task<IActionResult> Delete(int id)
        {
            
            List<Seller> sellers = await _sellerService.FindAllAsync();
            SalesRecord saleRef = await _salesRecordService.FindByIdAsync(id);
            return View(saleRef);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SalesRecordsFormViewModel? obj)
        {
            if (obj.TemplateSale.Amount <50 || obj.TemplateSale.Date.Year <= 2000 || obj.TemplateSale.Status == null)
            {
                var Id = obj.SellerId;
                return RedirectToAction(nameof(Create), new { id = Id });
            }

            Seller seila = await _sellerService.FindByIdAsync(obj.SellerId);
            seila.AddSales(obj.TemplateSale);
            var seile = seila.Sales;
            _context.Update(seila);
            _context.SaveChanges();            
            return RedirectToAction(nameof(AllSales), new {id = seila.Id});

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SalesRecord obj)
        {
            SalesRecord saleRef = await _salesRecordService.FindByIdAsync(obj.Id);
            Seller seller = await _sellerService.FindByIdAsync(obj.Seller.Id);
            SalesRecord saleToRemove = _salesRecordService.Any(seller.Sales, obj.Id);

            seller.RemoveSales(saleToRemove);
            seller.AddSales(obj);
            _context.Update(seller);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AllSales), new { id = obj.Seller.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(SalesRecord sellerSale)
        {
            SalesRecord saleRef = await _salesRecordService.FindByIdAsync(sellerSale.Id);
            List<Seller> sellers = await _sellerService.FindAllAsync();
            Seller seller = _sellerService.Any(sellers, sellerSale.Id);

            await _salesRecordService.RemoveAsync(sellerSale.Id);


            return RedirectToAction(nameof(AllSales), new { id = seller.Id });
        }
    }
}
