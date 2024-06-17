using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWT_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaisesController : ControllerBase
    {
        [HttpGet]
        [Authorize(Policy = "RequireAdminRole")]
        [Route("ListaPaises")]
        public async Task<IActionResult> ObtenerPaises()
        {
            var listaPaises = await Task.FromResult(new List<string> { "Mexico", "Rusia", "Francia", "Alemania" });
            return Ok(listaPaises);
        }

        [HttpGet]
        [Authorize(Policy = "RequireUserRole")]
        [Route("ListaFrutas")]
        public async Task<IActionResult> ObtenerFrutas()
        {
            var listaPaises = await Task.FromResult(new List<string> { "Sandia", "Mango", "Fresa", "Guayaba" });
            return Ok(listaPaises);
        }


    }
}
