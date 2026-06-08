using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Json;
using TechMoveMVC.Models;

namespace TechMoveMVC.Controllers
{
    public class ServiceRequestController : Controller
    {
        private readonly HttpClient _httpClient;

        public ServiceRequestController(
            IHttpClientFactory factory)
        {
            _httpClient =
                factory.CreateClient("TechMoveAPI");
        }

        public async Task<IActionResult> Index()
        {
            var requests =
                await _httpClient
                .GetFromJsonAsync<List<ServiceRequest>>(
                    "api/servicerequests");

            return View(requests);
        }

        public async Task<IActionResult> Create()
        {
            var contracts =
                await _httpClient
                .GetFromJsonAsync<List<Contract>>(
                    "api/contracts");

            ViewBag.Contracts =
                new SelectList(
                    contracts,
                    "ContractId",
                    "ContractId");

            ViewBag.ServiceTypes =
                new SelectList(
                    new List<string>
                    {
                        "Air",
                        "Sea",
                        "Road"
                    });

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            ServiceRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            await _httpClient.PostAsJsonAsync(
                "api/servicerequests",
                request);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var request =
                await _httpClient
                .GetFromJsonAsync<ServiceRequest>(
                    $"api/servicerequests/{id}");

            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
            ServiceRequest request)
        {
            await _httpClient.PutAsJsonAsync(
                $"api/servicerequests/{request.ServiceRequestId}",
                request);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var request =
                await _httpClient
                .GetFromJsonAsync<ServiceRequest>(
                    $"api/servicerequests/{id}");

            return View(request);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            await _httpClient.DeleteAsync(
                $"api/servicerequests/{id}");

            return RedirectToAction(nameof(Index));
        }
    }
}