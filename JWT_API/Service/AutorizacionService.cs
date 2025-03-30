using Dapper;
using JWT_API.Datos;
using JWT_API.Interfaces;
using JWT_API.Models;
using JWT_API.Models.Custom;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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
      // Obtén la clave secreta desde la configuración y conviértela a un array de bytes.
      // Esta clave es usada para firmar el token y debe mantenerse en secreto.
      var keyBytes = Encoding.UTF8.GetBytes(_config.GetValue<string>("JwtSettings:key"));

      // Define los claims (reclamaciones) que se incluirán en el payload del token.
      // Estos son datos asociados al usuario que serán codificados en el token.
      var claims = new[]
      {
        new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()), // ID del usuario.
        new Claim(ClaimTypes.Name, user.NombreUsuario), // Nombre del usuario.
        new Claim(ClaimTypes.Role, user.RolReference.Nombre),
      };

      // Configura las credenciales del token, especificando el algoritmo de firma y la clave secreta.
      var credencialesToken = new SigningCredentials(
          new SymmetricSecurityKey(keyBytes), // Clave secreta para firmar el token.
          SecurityAlgorithms.HmacSha256 // Algoritmo de firma HMAC con SHA-256.
      );

      // Configura el descriptor del token, incluyendo los claims, fecha de expiración y credenciales.
      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(claims), // Los claims que se incluirán en el token.
        Expires = DateTime.UtcNow.AddMinutes(3), // Fecha de expiración del token (3 minutos en este caso).
        SigningCredentials = credencialesToken // Credenciales usadas para firmar el token.
      };

      // Crea una instancia de JwtSecurityTokenHandler para generar el token.
      var tokenHandler = new JwtSecurityTokenHandler();

      // Crea el token usando el descriptor configurado.
      var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

      // Convierte el token en una cadena de texto.
      string tokenCreado = tokenHandler.WriteToken(tokenConfig);

      // Retorna el token generado.
      return tokenCreado;
    }


    private string GenerateRefreshToken()
    {
      var arregloByte = new byte[64];
      string refreshToken = "";
      using (var rng = RandomNumberGenerator.Create())
      {
        rng.GetBytes(arregloByte);
        refreshToken = Convert.ToBase64String(arregloByte);
      }

      return refreshToken;
    }

    public async Task<AutorizacionResponse> SaveRefreshToken(int idUsuario, string token, string refreshToken)
    {

      var parametros = new { IdUsuario = idUsuario, Token = token, RefreshToken = refreshToken, FechaCreacion = DateTime.UtcNow, FechaExpiracion = DateTime.UtcNow.AddMinutes(5) };

      var cn = new Conexion();
      using (var conexion = new SqlConnection(cn.GetConnection()))
      {
        await conexion.OpenAsync();
        await conexion.ExecuteAsync("SP_InsertRefreshToken", parametros, commandType: CommandType.StoredProcedure);
      }
      return new AutorizacionResponse() { Token = token, refreshToken = refreshToken, Resultado = true, Msg = "Ok" };

    }



    public async Task<AutorizacionResponse> DevolverToken(AutorizacionRequest request)
    {
      var user_target = await _userRepository.AutenticarUsuario(request);

      if (user_target == null) return await Task.FromResult<AutorizacionResponse>(null);

      string tokenCreado = GenerateToken(user_target);
      string refreshToken = GenerateRefreshToken();

      return await SaveRefreshToken(Convert.ToInt32(user_target.IdUsuario), tokenCreado, refreshToken);


    }

    public async Task<AutorizacionResponse> DevolverRefreshToken(RefreshTokenRequest request, Usuario user)
    {
      var cn = new Conexion();
      string query = "SELECT * FROM HistorialRefreshToken WHERE IdUsuario = @IdUsuario AND Token = @Token AND RefreshToken = @RefreshToken";
      HistorialRefreshToken? refreshToken = null;
      var parametros = new { IdUsuario = user.IdUsuario, Token = request.tokenExpirado, RefreshToken = request.refreshToken };
      using (var conexion = new SqlConnection(cn.GetConnection()))
      {
        await conexion.OpenAsync();

        refreshToken = await conexion.QueryFirstOrDefaultAsync<HistorialRefreshToken>(query, parametros);

      }

      if (refreshToken == null) return new AutorizacionResponse() { Resultado = false, Msg = "Token no encontrado" };

      var refreshTokenCreado = GenerateRefreshToken();
      var tokenCreado = GenerateToken(user);

      return await SaveRefreshToken(Convert.ToInt32(user.IdUsuario), tokenCreado, refreshTokenCreado);
    }

    public async Task<AutorizacionResponse> ValidateRefreshToken(RefreshTokenRequest request)
    {

      var tokenHanlder = new JwtSecurityTokenHandler();
      var tokenExpirado = tokenHanlder.ReadJwtToken(request.tokenExpirado);

      if (tokenExpirado.ValidTo > DateTime.UtcNow) return new AutorizacionResponse() { Resultado = false, Msg = "Este token aun no ha expirado" };

      string idUsuario = tokenExpirado.Claims.First(x => x.Type == JwtRegisteredClaimNames.NameId).Value.ToString();

      var user = await _userRepository.GetUserById(Convert.ToInt32(idUsuario));

      return await DevolverRefreshToken(request, user);

    }

  }
}
