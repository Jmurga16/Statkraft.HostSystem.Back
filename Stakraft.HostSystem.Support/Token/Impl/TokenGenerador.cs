using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Stakraft.HostSystem.Support.Token.Impl
{
    public class TokenGenerador : ITokenGenerador
    {
        private static Dictionary<string, string> mapUserRefresToken = new Dictionary<string, string>();
        private readonly IConfiguration _configuration;
        public TokenGenerador(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:Key"]));
            var signIn = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokeOptions = new JwtSecurityToken(
                issuer: _configuration["jwt:Issuer"],
                audience: _configuration["jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["jwt:expires"])),
                signingCredentials: signIn
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return tokenString;
        }
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:Key"]));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = secretKey,
                ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }

        public bool RefreshTokenActive(string usuario, string refreshToken)
        {
            var existeRefresh = mapUserRefresToken.ContainsKey(usuario);
            if (existeRefresh)
            {
                return mapUserRefresToken[usuario] == refreshToken;
            }
            return false;
        }

        public void RemoveUserRefreshToken(string user)
        {
            mapUserRefresToken.Remove(user);
        }

        public void SaveUserRefresToken(string user, string refreshToken)
        {
            var getUser = mapUserRefresToken.ContainsKey(user);
            if (getUser)
            {
                mapUserRefresToken[user] = refreshToken;
            }
            else
            {
                mapUserRefresToken.Add(user, refreshToken);
            }
        }
    }
}
