namespace JWT_API.Models
{
    public class Usuario
    {
        public string? IdUsuario { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Clave { get; set; }
        public int Rol { get; set; }
        public Rol? RolReference { get; set; }
    }
}
