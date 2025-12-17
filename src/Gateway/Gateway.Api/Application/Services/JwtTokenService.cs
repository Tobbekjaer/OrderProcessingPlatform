using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace Gateway.Api.Application.Services;


public class CreateTokenOptions
{
    public Dictionary<string, string> Claims { get; set; }
}

public class JwtTokenService
{
    private readonly string _authenticationProviderKey;
    
    public JwtTokenService(string authenticationProviderKey)
    {
        _authenticationProviderKey = authenticationProviderKey;
    }
    
    public AuthenticationToken CreateToken(CreateTokenOptions options)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationProviderKey));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        
        var claims = new List<Claim>();
        foreach (var claim in options.Claims)
        {
            if (claim.Key == "sub")
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, claim.Value));
                continue;
            }
            claims.Add(new Claim(claim.Key, claim.Value));
            
            claims.Add(new Claim(ClaimTypes.Role, claim.Value));
        }

        var tokenOptions = new JwtSecurityToken(
            signingCredentials: signingCredentials,
            expires: DateTime.UtcNow.AddHours(1),
            claims: claims
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        var authToken = new AuthenticationToken
        {
            Value = tokenString
        };

        return authToken;
    }
}