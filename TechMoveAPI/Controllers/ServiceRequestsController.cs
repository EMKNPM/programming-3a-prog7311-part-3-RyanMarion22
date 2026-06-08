using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMoveMVC.Data;
using TechMoveMVC.Models;
using TechMoveMVC.Factory;
using TechMoveMVC.Strategies;
using TechMoveMVC.Services;

namespace TechMoveAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly CurrencyService _currencyService;

        public ServiceRequestsController(
            AppDbContext context,
            CurrencyService currencyService)
        {
            _context = context;
            _currencyService = currencyService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceRequest>>> GetRequests()
        {
            return await _context.ServiceRequests
                .Include(r => r.Contract)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceRequest>> GetRequest(int id)
        {
            var request =
                await _context.ServiceRequests
                .FirstOrDefaultAsync(r =>
                    r.ServiceRequestId == id);

            if (request == null)
                return NotFound();

            return request;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceRequest>> CreateRequest(
            ServiceRequest request)
        {
            var contract =
                await _context.Contracts.FindAsync(
                    request.ContractId);

            if (contract == null)
                return BadRequest("Invalid contract");

            if (contract.Status == "Expired" ||
                contract.Status == "OnHold")
            {
                return BadRequest("Contract not active");
            }

            var serviceRequest =
                ServiceRequestFactory.Create(
                    request.ServiceType);

            serviceRequest.ContractId =
                request.ContractId;

            serviceRequest.Description =
                request.Description;

            serviceRequest.CostUSD =
                request.CostUSD;

            serviceRequest.Status =
                "Pending";

            serviceRequest.ServiceType =
                request.ServiceType;

            var currencyContext =
                new CurrencyContext();

            currencyContext.SetStrategy(
                new USDStrategy(_currencyService));

            serviceRequest.CostZAR =
                await currencyContext.Convert(
                    request.CostUSD);

            _context.ServiceRequests.Add(
                serviceRequest);

            await _context.SaveChangesAsync();

            return Ok(serviceRequest);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRequest(
            int id,
            ServiceRequest request)
        {
            if (id != request.ServiceRequestId)
                return BadRequest();

            _context.Entry(request).State =
                EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(
            int id)
        {
            var request =
                await _context.ServiceRequests
                .FindAsync(id);

            if (request == null)
                return NotFound();

            _context.ServiceRequests.Remove(
                request);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}