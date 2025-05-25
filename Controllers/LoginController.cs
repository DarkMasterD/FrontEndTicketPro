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
        public IActionResult Index() => View();

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

                HttpContext.Session.SetInt32("id_usuario", idUsuario);
                HttpContext.Session.SetString("tipo_usuario", tipoUsuario);

                string rolFinal = "Cliente";

                if (tipoUsuario == "I")
                {
                    var rolResponse = await client.GetAsync(_configuration["ApiBaseUrl"] + $"/auth/rol/{idUsuario}");
                    if (rolResponse.IsSuccessStatusCode)
                    {
                        var rolJson = await rolResponse.Content.ReadAsStringAsync();
                        rolFinal = rolJson.Replace("\"", "");
                    }
                }

                HttpContext.Session.SetString("rol_usuario", rolFinal);

                if (tipoUsuario == "E")
                    return RedirectToAction("Inicio", "Cliente");
                else if (rolFinal == "Administrador")
                    return RedirectToAction("Inicio", "Admin");
                else
                    return RedirectToAction("Inicio", "Tecnico");
            }

            ViewBag.Error = "Credenciales incorrectas.";
            return View();
        }
    }
}
