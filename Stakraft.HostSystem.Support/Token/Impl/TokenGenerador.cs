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
            var tokenString = "";

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:Key"]));

            var signIn = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var dicClaim = ConvertClaims(claims);

            SecurityTokenDescriptor securityTokenDescriptor = new()
            {
                Issuer = _configuration["jwt:Issuer"],
                Audience = _configuration["jwt:Audience"],
                Claims = dicClaim,
                Expires = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["jwt:expires"])),
                SigningCredentials = signIn,
                NotBefore = DateTime.Now
            };

            if (securityTokenDescriptor != null)
            {
                tokenString = new JwtSecurityTokenHandler().CreateEncodedJwt(securityTokenDescriptor);
            }

            return tokenString;
        }

        public IDictionary<string, object> ConvertClaims(IEnumerable<Claim> claims)
        {
            return claims
                .GroupBy(i => i.Type)
                .ToDictionary
                (i => i.Key, i => (object)
                    (i.Count() == 1 ? i.First().Value : i.Select(i => i.Value).ToArray())
                );
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
