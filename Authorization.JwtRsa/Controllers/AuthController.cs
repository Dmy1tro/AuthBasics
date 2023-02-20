using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Authorization.OAuthGitHub.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpGet("trusted-token")]
        public IActionResult Login()
        {
            // Get private key.
            using var rsa = RSA.Create();
            var privateKey = System.IO.File.ReadAllBytes("rsa_private_key");
            rsa.ImportRSAPrivateKey(privateKey, out _);

            // Generate token using private key.
            var rsaKey = new RsaSecurityKey(rsa);
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Audience = "jwt_audience",
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Upn, Guid.NewGuid().ToString()),
                }),
                SigningCredentials = new SigningCredentials(rsaKey, SecurityAlgorithms.RsaSha256)
            });

            var jwtTokenInStringFormat = tokenHandler.WriteToken(token);

            return Ok(jwtTokenInStringFormat);
        }

        [HttpGet("villain-token")]
        public IActionResult GetVilliainToken()
        {
            // Generate fake token without using private key. Villain supposed not know private key =)
            using var rsa = RSA.Create();
            var rsaKey = new RsaSecurityKey(rsa);
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Audience = "jwt_audience",
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Upn, Guid.NewGuid().ToString()),
                }),
                SigningCredentials = new SigningCredentials(rsaKey, SecurityAlgorithms.RsaSha256)
            });

            var jwtTokenInStringFormat = tokenHandler.WriteToken(token);

            return Ok(jwtTokenInStringFormat);
        }

        // Villain token won't work here.
        [HttpGet("protected")]
        public IActionResult Protected(string token)
        {
            // Get public key.
            using var rsa = RSA.Create();
            var publicKey = System.IO.File.ReadAllBytes("rsa_public_key").Select(b => b).ToArray();
            rsa.ImportRSAPublicKey(publicKey, out _);

            // Validate token using public key.
            var key = new RsaSecurityKey(rsa);
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidAudience = "jwt_audience",
                IssuerSigningKey = new RsaSecurityKey(rsa),
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            };

            try
            {
                tokenHandler.ValidateToken(token, validationParams, out _);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("Success!");
        }
    }
}