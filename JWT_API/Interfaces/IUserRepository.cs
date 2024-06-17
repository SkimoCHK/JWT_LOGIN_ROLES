using JWT_API.Models;
using JWT_API.Models.Custom;

namespace JWT_API.Interfaces
{
    public interface IUserRepository
    {
        Task<Usuario> AutenticarUsuario(AutorizacionRequest request);
        Task<IEnumerable<Rol>> GetRoles();
    }
}
