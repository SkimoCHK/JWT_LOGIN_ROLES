using JWT_API.Models.Custom;

namespace JWT_API.Interfaces
{
    public interface IAutorizacionService
    {
        Task<AutorizacionResponse> DevolverToken(AutorizacionRequest request);
        Task<AutorizacionResponse> ValidateRefreshToken(RefreshTokenRequest request);
    }
}
