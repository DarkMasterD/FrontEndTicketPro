using FrontEndTicketPro.Models.Admin;
using Microsoft.AspNetCore.Mvc;
using FrontEndTicketPro.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace FrontEndTicketPro.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public DashboardController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [SessionAuthorize("admin")]
        public async Task<IActionResult> Index(DateTime? fechaInicio, DateTime? fechaFin, string categoria, string prioridad, string estado)
        {
            var client = _httpClientFactory.CreateClient("ApiInsegura");
            var apiBaseUrl = _configuration["ApiBaseUrl"];

            var query = $"?";
            if (fechaInicio.HasValue)
                query += $"fechaInicio={fechaInicio.Value:yyyy-MM-dd}&";
            if (fechaFin.HasValue)
                query += $"fechaFin={fechaFin.Value:yyyy-MM-dd}&";
            if (!string.IsNullOrEmpty(categoria))
                query += $"categoria={categoria}&";
            if (!string.IsNullOrEmpty(prioridad))
                query += $"prioridad={prioridad}&";
            if (!string.IsNullOrEmpty(estado))
                query += $"estado={estado}&";

            var tickets = await client.GetFromJsonAsync<List<TablaTicketsInicio>>($"{apiBaseUrl}/dashboardfiltro/tickets-filtrados{query}");
            var resumen = await client.GetFromJsonAsync<DashboardResumen>($"{apiBaseUrl}/ticket/resumen-dashboard-filtrado{query}");


            ViewBag.Resumen = resumen;
            ViewBag.Tickets = tickets;

            return View();
        }

        [SessionAuthorize("admin")]
        public async Task<IActionResult> PorCategoria(DateTime? fechaInicio, DateTime? fechaFin, string categoria)
        {
            var client = _httpClientFactory.CreateClient("ApiInsegura");
            var apiBaseUrl = _configuration["ApiBaseUrl"];

            var query = "?";
            if (fechaInicio.HasValue)
                query += $"fechaInicio={fechaInicio.Value:yyyy-MM-dd}&";
            if (fechaFin.HasValue)
                query += $"fechaFin={fechaFin.Value:yyyy-MM-dd}&";
            if (!string.IsNullOrEmpty(categoria))
                query += $"categoria={categoria}&";

            var categorias = await client.GetFromJsonAsync<List<TicketsPorCategoria>>($"{apiBaseUrl}/ticket/dashboard-categoria-filtrada{query}");
            ViewBag.Categorias = categorias;

            return View();
        }


      

        [SessionAuthorize("admin")]
        public async Task<IActionResult> TiemposPromedioPorDia()
        {
            var client = _httpClientFactory.CreateClient("ApiInsegura");
            var apiBaseUrl = _configuration["ApiBaseUrl"];

            var tiempos = await client.GetFromJsonAsync<List<TiempoResolucion>>($"{apiBaseUrl}/ticket/dashboard-resolucion");
            ViewBag.Tiempos = tiempos;

            return View();
        }
        [SessionAuthorize("admin")]
        public async Task<IActionResult> PorTecnico(DateTime? fechaInicio, DateTime? fechaFin, string tecnico)
        {
            var client = _httpClientFactory.CreateClient("ApiInsegura");
            var apiBaseUrl = _configuration["ApiBaseUrl"];

            var query = $"?";
            if (fechaInicio.HasValue)
                query += $"fechaInicio={fechaInicio.Value:yyyy-MM-dd}&";
            if (fechaFin.HasValue)
                query += $"fechaFin={fechaFin.Value:yyyy-MM-dd}&";
            if (!string.IsNullOrEmpty(tecnico))
                query += $"tecnico={tecnico}&";

            var rendimiento = await client.GetFromJsonAsync<List<RendimientoTecnico>>($"{apiBaseUrl}/ticket/dashboard-tecnico-filtrado{query}");
            ViewBag.Tecnicos = rendimiento;

            return View();
        }

    }
}
