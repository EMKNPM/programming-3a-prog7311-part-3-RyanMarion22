using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMoveMVC.Data;
using TechMoveMVC.Models;
using TechMoveMVC.Services;
using TechMoveMVC.Strategies;
using TechMoveMVC.Factory;

namespace TechMoveMVC.Controllers
{
    public class ServiceRequestController : Controller
    {
        private readonly AppDbContext _context;
        private readonly CurrencyService _currencyService;

        public ServiceRequestController(AppDbContext context, CurrencyService currencyService)
        {
            _context = context;
            _currencyService = currencyService;
        }

        //  INDEX 
        public async Task<IActionResult> Index()
        {
            var requests = await _context.ServiceRequests
                .Include(r => r.Contract)
                .ToListAsync();

            return View(requests);
        }

        // CREATE 
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ServiceRequest request, string serviceType)
        {
            var contract = await _context.Contracts.FindAsync(request.ContractId);

            if (contract == null)
                ModelState.AddModelError("", "Invalid contract");

            if (contract != null && (contract.Status == "Expired" || contract.Status == "OnHold"))
                ModelState.AddModelError("", "Contract not active");

            if (string.IsNullOrEmpty(serviceType))
                ModelState.AddModelError("", "Select service type");

            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(request);
            }

            var serviceRequest = ServiceRequestFactory.Create(serviceType);

            serviceRequest.ContractId = request.ContractId;
            serviceRequest.Description = request.Description;
            serviceRequest.CostUSD = request.CostUSD;
            serviceRequest.Status = "Pending";
            serviceRequest.ServiceType = serviceType;

            var currencyContext = new CurrencyContext();
            currencyContext.SetStrategy(new USDStrategy(_currencyService));

            serviceRequest.CostZAR = await currencyContext.Convert(request.CostUSD);

            _context.ServiceRequests.Add(serviceRequest);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // EDIT 

        public async Task<IActionResult> Edit(int id)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request == null) return NotFound();

            LoadDropdowns();
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ServiceRequest request)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns();
                return View(request);
            }

            _context.Update(request);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //  DELETE 

        public async Task<IActionResult> Delete(int id)
        {
            var request = await _context.ServiceRequests
                .Include(r => r.Contract)
                .FirstOrDefaultAsync(r => r.ServiceRequestId == id);

            if (request == null) return NotFound();

            return View(request);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = await _context.ServiceRequests.FindAsync(id);

            if (request != null)
            {
                _context.ServiceRequests.Remove(request);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        //  HELPERS 

        private void LoadDropdowns()
        {
            ViewBag.Contracts = new SelectList(_context.Contracts, "ContractId", "ContractId");

            ViewBag.ServiceTypes = new SelectList(new List<string>
            {
                "Air",
                "Sea",
                "Road"
            });
        }
    }
}