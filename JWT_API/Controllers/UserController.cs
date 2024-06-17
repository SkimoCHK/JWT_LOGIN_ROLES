﻿using JWT_API.Interfaces;
using JWT_API.Models.Custom;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> GetUsuario([FromBody] AutorizacionRequest request)
        {
            var autor = await _service.DevolverToken(request);
            if(autor == null) return Unauthorized();
            return Ok(autor);
        }
          

    }
}
