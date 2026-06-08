using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using TechMoveMVC.Models;

namespace TechMoveMVC.Controllers
{
    public class ClientController : Controller
    {
        private readonly HttpClient _httpClient;

        public ClientController(
            IHttpClientFactory factory)
        {
            _httpClient =
                factory.CreateClient("TechMoveAPI");
        }

        public async Task<IActionResult> Index()
        {
            var clients =
                await _httpClient
                .GetFromJsonAsync<List<Client>>(
                    "api/clients");

            return View(clients);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Client client)
        {
            if (!ModelState.IsValid)
                return View(client);

            await _httpClient.PostAsJsonAsync(
                "api/clients",
                client);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client =
                await _httpClient
                .GetFromJsonAsync<Client>(
                    $"api/clients/{id}");

            return View(client);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Client client)
        {
            if (!ModelState.IsValid)
                return View(client);

            await _httpClient.PutAsJsonAsync(
                $"api/clients/{client.ClientId}",
                client);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client =
                await _httpClient
                .GetFromJsonAsync<Client>(
                    $"api/clients/{id}");

            return View(client);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            await _httpClient.DeleteAsync(
                $"api/clients/{id}");

            return RedirectToAction(nameof(Index));
        }
    }
}