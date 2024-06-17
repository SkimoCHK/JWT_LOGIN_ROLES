namespace JWT_API.Models
{
    public class Rol
    {
        public int IdRol { get; set; }
        public string? Nombre { get; set; }
        public int Nivel { get; set; }
        public bool Activo { get; set; }
    }
}
