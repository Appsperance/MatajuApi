using MatajuApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace MatajuApi.Helpers
{
    public static class JwtHelper
    {
        public static string GenerateToken(User user, IConfiguration configuration)
        {
            string privateKeyPem = configuration["Jwt:PrivateKey"];
            RSA rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyPem.ToCharArray());

            RsaSecurityKey signingKey = new RsaSecurityKey(rsa);
            SigningCredentials credentials = new SigningCredentials(signingKey, SecurityAlgorithms.RsaSha256);

            // 클레임 생성
            Claim[] claims = new[]
                         {
                             new Claim(ClaimTypes.Name, user.Name),
                             new Claim(ClaimTypes.Role, user.Roles),
                             new Claim("Nickname", user.Nickname)
                         };

            // JWT 생성
            var token = new JwtSecurityToken(issuer: configuration["Jwt:Issuer"],
                                             audience: configuration["Jwt:Audience"],
                                             claims: claims,
                                             expires: DateTime.UtcNow.AddMinutes(double.Parse(configuration["Jwt:TokenLifetimeMinutes"])),
                                             signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static RsaSecurityKey GetPublicKey(IConfiguration configuration)
        {
            // RSA Public Key 로드
            string publicKeyPem = configuration["Jwt:PublicKey"];
            var rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyPem.ToCharArray());
            return new RsaSecurityKey(rsa);
        }
    }
}