using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace FrontEndTicketPro.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public LoginController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var rol = HttpContext.Session.GetString("Rol");

            if (!string.IsNullOrEmpty(rol))
            {
                if (rol == "admin") return RedirectToAction("Inicio", "Admin");
                if (rol == "tecnico") return RedirectToAction("Inicio", "Tecnico");
                if (rol == "cliente") return RedirectToAction("Inicio", "Cliente");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string usuario, string contrasenia)
        {
            var client = _httpClientFactory.CreateClient("ApiInsegura");
            var apiUrl = _configuration["ApiBaseUrl"] + "/auth/login";

            var request = new { Usuario = usuario, Contrasenia = contrasenia };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

                var tipoUsuario = data.GetProperty("tipo").GetString();
                var idUsuario = data.GetProperty("id").GetInt32();

                string rolFinal = "cliente";

                if (tipoUsuario == "I")
                {
                    var rolResponse = await client.GetAsync(_configuration["ApiBaseUrl"] + $"/auth/rol/{idUsuario}");
                    if (rolResponse.IsSuccessStatusCode)
                    {
                        var rolJson = await rolResponse.Content.ReadAsStringAsync();
                        rolFinal = rolJson.Replace("\"", "").ToLower();

                        if (rolFinal == "administrador")
                            rolFinal = "admin";
                        else if (rolFinal == "técnico" || rolFinal == "tecnico") // <- con o sin tilde
                            rolFinal = "tecnico";
                    }

                    var internoResponse = await client.GetAsync(_configuration["ApiBaseUrl"] + $"/auth/interno/{idUsuario}");
                    if (internoResponse.IsSuccessStatusCode)
                    {
                        var internoJson = await internoResponse.Content.ReadAsStringAsync();
                        var internoData = JsonSerializer.Deserialize<JsonElement>(internoJson);
                        var idInterno = internoData.GetProperty("id_usuario_interno").GetInt32();
                        HttpContext.Session.SetInt32("id_usuario_interno", idInterno);
                    }

                }


                HttpContext.Session.SetInt32("id_usuario", idUsuario);
                HttpContext.Session.SetString("tipo_usuario", tipoUsuario);
                HttpContext.Session.SetString("Rol", rolFinal);

                if (tipoUsuario == "E")
                    return RedirectToAction("Inicio", "Cliente");
                else if (rolFinal == "administrador")
                    return RedirectToAction("Inicio", "Admin");
                else
                    return RedirectToAction("Inicio", "Tecnico");
            }

            ViewBag.Error = "Credenciales incorrectas.";
            return View();
        }

        [HttpPost]
        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}
