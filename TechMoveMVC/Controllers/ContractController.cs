using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Json;
using TechMoveMVC.Models;

namespace TechMoveMVC.Controllers
{
    public class ContractController : Controller
    {
        private readonly HttpClient _httpClient;

        public ContractController(
            IHttpClientFactory factory)
        {
            _httpClient =
                factory.CreateClient("TechMoveAPI");
        }

        public async Task<IActionResult> Index()
        {
            var contracts =
                await _httpClient
                .GetFromJsonAsync<List<Contract>>(
                    "api/contracts");

            return View(contracts);
        }

        public async Task<IActionResult> Create()
        {
            var clients =
                await _httpClient
                .GetFromJsonAsync<List<Client>>(
                    "api/clients");

            ViewBag.Clients =
                new SelectList(
                    clients,
                    "ClientId",
                    "Name");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            Contract contract,
            IFormFile file)
        {
            if (!ModelState.IsValid)
                return View(contract);

            if (file != null)
            {
                var fileName =
                    Guid.NewGuid() + ".pdf";

                var folder =
                    Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/files");

                Directory.CreateDirectory(folder);

                var path =
                    Path.Combine(folder, fileName);

                using var stream =
                    new FileStream(path, FileMode.Create);

                await file.CopyToAsync(stream);

                contract.FilePath =
                    "/files/" + fileName;
            }

            await _httpClient.PostAsJsonAsync(
                "api/contracts",
                contract);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var contract =
                await _httpClient
                .GetFromJsonAsync<Contract>(
                    $"api/contracts/{id}");

            return View(contract);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
            Contract contract)
        {
            await _httpClient.PutAsJsonAsync(
                $"api/contracts/{contract.ContractId}",
                contract);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var contract =
                await _httpClient
                .GetFromJsonAsync<Contract>(
                    $"api/contracts/{id}");

            return View(contract);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            await _httpClient.DeleteAsync(
                $"api/contracts/{id}");

            return RedirectToAction(nameof(Index));
        }
    }
}