using JWT_API.Interfaces;
using JWT_API.Models;
using JWT_API.Models.Custom;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JWT_API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UserController : ControllerBase
  {
    private IAutorizacionService _service;

    public UserController(IAutorizacionService service) => _service = service;

    [HttpPost]
    [Route("Autenticar")]
    public async Task<IActionResult> Autenticar([FromBody] AutorizacionRequest request)
    {
      var autor = await _service.DevolverToken(request);
      if (autor == null) return Unauthorized();
      return Ok(autor);
    }
    [HttpPost]
    [Route("obtener-refreshToken")]
    public async Task<IActionResult> GetRefreshToken([FromBody] RefreshTokenRequest request)
    {
      var response = await _service.ValidateRefreshToken(request);
      if (response.Resultado) return Ok(response);

      return BadRequest(response);

    }
    [HttpGet]
    [Route("info-user")]
    public async Task<IActionResult> GetUserInfo()
    {
      string? idusuario = User.FindAll(ClaimTypes.NameIdentifier).FirstOrDefault().ToString();
      string? nombre = User.FindAll(ClaimTypes.Name).FirstOrDefault().ToString();
      string? rol = User.FindAll(ClaimTypes.Role).FirstOrDefault().ToString();
      return Ok(new
      {
        IdUsuario = idusuario,
        Nombre = nombre,
        Rol = rol
      });

    }


  }
}
