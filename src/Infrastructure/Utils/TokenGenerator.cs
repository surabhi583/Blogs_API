using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Blogs.Infrastructure.Utils.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Blogs.Infrastructure.Utils;

public class TokenGenerator : ITokenGenerator
{
    private readonly AppSettings _appSettings;

    public TokenGenerator(IOptions<AppSettings> settings)
    {
        _appSettings = settings.Value;
    }

    public string CreateToken(string username)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var Subject = new ClaimsIdentity(new[] { new Claim(JwtRegisteredClaimNames.Sub, username) });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = Subject,
                Expires = DateTime.UtcNow.AddMinutes(_appSettings.TokenLifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _appSettings.ValidIssuer,
                Audience = _appSettings.ValidIssuer
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return token;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}
