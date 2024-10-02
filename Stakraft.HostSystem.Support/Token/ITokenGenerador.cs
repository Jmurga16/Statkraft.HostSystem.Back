using System.Security.Claims;

namespace Stakraft.HostSystem.Support.Token
{
    public interface ITokenGenerador
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        void SaveUserRefresToken(string user, string refreshToken);
        void RemoveUserRefreshToken(string user);
        bool RefreshTokenActive(string usuario, string refreshToken);
    }
}
