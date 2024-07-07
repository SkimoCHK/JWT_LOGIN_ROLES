namespace JWT_API.Models
{
    public class HistorialRefreshToken
    {

        public int IdHistorialToken { get; set; }

        public int? IdUsuario { get; set; }

        public Usuario? refUsuario { get; set; }

        public string? Token { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public DateTime? FechaExpiracion { get; set; }

        public bool? Activo { get; set; }


    }
}
