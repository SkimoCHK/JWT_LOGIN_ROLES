using Dapper;
using JWT_API.Datos;
using JWT_API.Interfaces;
using JWT_API.Models;
using JWT_API.Models.Custom;
using Microsoft.Data.SqlClient;

namespace JWT_API.Repository
{
    public class UserRepository : IUserRepository
    {
        public async Task<Usuario> AutenticarUsuario(AutorizacionRequest request)
        {
            var usuario = new Usuario();
            var cn = new Conexion();
            string query = "SELECT * FROM " +
                           "USUARIO u " +
                           "INNER JOIN Rol r ON u.rol = r.idRol " +
                           "WHERE u.NombreUsuario = @NombreUsuario AND u.Clave = @Clave";
            var parametros = new { NombreUsuario = request.Nombre, Clave = request.Clave };

            using (var conexion = new SqlConnection(cn.GetConnection()))
            {

                await conexion.OpenAsync();
                 var result = await conexion.QueryAsync<Usuario, Rol, Usuario>(
                    query,
                    (usr, rol) =>
                    {
                        usr.RolReference = rol;
                        return usr;
                    },
                    splitOn: "IdRol",
                    param: parametros
                    );
                usuario = result.FirstOrDefault();   
            }

            return usuario;
        }

        public async Task<IEnumerable<Rol>> GetRoles()
        {
            IEnumerable<Rol> roles = new List<Rol>();
            var cn = new Conexion();
            string query = "SELECT * FROM Rol";

            using(var conexion = new SqlConnection( cn.GetConnection()))
            {
                await conexion.OpenAsync();
                roles = await conexion.QueryAsync<Rol>(query);
            }
            return roles;

        }
    }
}
