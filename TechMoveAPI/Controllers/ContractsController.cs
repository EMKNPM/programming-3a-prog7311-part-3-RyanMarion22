using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMoveMVC.Data;
using TechMoveMVC.Models;

namespace TechMoveAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContractsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contract>>> GetContracts()
        {
            return await _context.Contracts
                .Include(c => c.Client)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contract>> GetContract(int id)
        {
            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.ContractId == id);

            if (contract == null)
                return NotFound();

            return contract;
        }

        [HttpPost]
        public async Task<ActionResult<Contract>> CreateContract(Contract contract)
        {
            _context.Contracts.Add(contract);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetContract),
                new { id = contract.ContractId },
                contract);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            [FromBody] string status)
        {
            var contract =
                await _context.Contracts.FindAsync(id);

            if (contract == null)
                return NotFound();

            contract.Status = status;

            await _context.SaveChangesAsync();

            return Ok(contract);

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContract(
    int id,
    Contract contract)
        {
            if (id != contract.ContractId)
                return BadRequest();

            _context.Entry(contract).State =
                EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var contract =
                await _context.Contracts.FindAsync(id);

            if (contract == null)
                return NotFound();

            _context.Contracts.Remove(contract);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}