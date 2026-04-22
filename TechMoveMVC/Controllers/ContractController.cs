using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMoveMVC.Data;
using TechMoveMVC.Models;
using TechMoveMVC.Services;

namespace TechMoveMVC.Controllers
{
    public class ContractController : Controller
    {
        private readonly AppDbContext _context;

        public ContractController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string status, DateTime? start, DateTime? end)
        {
            var contracts = _context.Contracts
                .Include(c => c.Client)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                contracts = contracts.Where(c => c.Status == status);

            if (start.HasValue)
                contracts = contracts.Where(c => c.StartDate >= start);

            if (end.HasValue)
                contracts = contracts.Where(c => c.EndDate <= end);

            return View(await contracts.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewBag.Clients = new SelectList(_context.Clients, "ClientId", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Contract contract, IFormFile file)
        {
            var notifier = new NotificationService();
            contract.Attach(notifier);

            if (!ModelState.IsValid)
            {
                ViewBag.Clients = new SelectList(_context.Clients, "ClientId", "Name");
                return View(contract);
            }

            if (file != null)
            {
                if (Path.GetExtension(file.FileName).ToLower() != ".pdf")
                {
                    ModelState.AddModelError("", "Only PDF allowed");
                    ViewBag.Clients = new SelectList(_context.Clients, "ClientId", "Name");
                    return View(contract);
                }

                var fileName = Guid.NewGuid() + ".pdf";
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                contract.FilePath = "/files/" + fileName;
            }

            _context.Add(contract);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // EDIT 

        public async Task<IActionResult> Edit(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null) return NotFound();

            ViewBag.Clients = new SelectList(_context.Clients, "ClientId", "Name", contract.ClientId);
            return View(contract);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Contract contract)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Clients = new SelectList(_context.Clients, "ClientId", "Name", contract.ClientId);
                return View(contract);
            }

            _context.Update(contract);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //  DELETE

        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.ContractId == id);

            if (contract == null) return NotFound();

            return View(contract);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract != null)
            {
                _context.Contracts.Remove(contract);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}