using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiForSalePortal.Services;

namespace WebApiForSalePortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NovaPoshtaController : ControllerBase
    {
        private readonly INovaPoshtaService _novaPoshtaService;
        public NovaPoshtaController(INovaPoshtaService novaPoshtaService)
        {
            _novaPoshtaService = novaPoshtaService;
        }
        [HttpGet]
        [Authorize]
        public async ValueTask<IActionResult> GetAdressesByCityName(string cityName)
        {

            return Ok(await _novaPoshtaService.GetPostOfficesAsync(cityName));
        }
    }
}
