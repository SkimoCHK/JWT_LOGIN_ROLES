using JWT_API.Interfaces;
using JWT_API.Models;
using JWT_API.Models.Custom;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWT_API.Service
{
    public class AutorizacionService : IAutorizacionService
    {
        private IUserRepository _userRepository;
        private readonly IConfiguration _config;

        public AutorizacionService(IUserRepository repo, IConfiguration config) 
        {
            _userRepository = repo;
            _config = config;
        }

        private string GenerateToken(Usuario user)
        {
            var keyBytes = Encoding.UTF8.GetBytes(_config.GetValue<string>("JwtSettings:key"));
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, user.NombreUsuario),
                new Claim(ClaimTypes.Role, user.RolReference.Nombre)

            };

            var credencialesToken = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256
            );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(3),
                SigningCredentials = credencialesToken
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

            string tokenCreado = tokenHandler.WriteToken(tokenConfig);



            return tokenCreado;

        }

        public async Task<AutorizacionResponse> DevolverToken(AutorizacionRequest request)
        {
            var user_target = await _userRepository.AutenticarUsuario(request);

            if (user_target == null) return await Task.FromResult<AutorizacionResponse>(null);

            string tokenCreado = GenerateToken(user_target);
            return new AutorizacionResponse() { Token = tokenCreado, Resultado = true, Msg = "Ok" };

        }
    }
}
