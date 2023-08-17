using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Interface;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Model.Dto.User;
using Model.Other;

namespace Service;

public class CustomJwtService : ICustomJwtService
{
    private readonly JWTTokenOptions _jwtTokenOptions;

    public CustomJwtService(IOptionsMonitor<JWTTokenOptions> jwtTokenOptions)
    {
        _jwtTokenOptions = jwtTokenOptions.CurrentValue;
    }


    public async Task<string> GetToken(UserRes user)
    {
        var result = await Task.Run(() =>
        {
            var claims = new[]
            {
                new Claim("id", user.Id),
                new Claim("NickName", user.NickName),
                new Claim("name", user.Name),
                new Claim("UserType", user.UserType.ToString()),
                new Claim("Image", string.IsNullOrEmpty(user.Image) ? "" : user.Image)
            };
            // 需要加密：需要加密的key:
            // Nuget:Microsoft.IdentityModel.Tokens
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenOptions.SecurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // nuget引入：System.IdentityModel.Tokens.Jwt
            var token = new JwtSecurityToken(
                issuer: _jwtTokenOptions.Issuer,
                audience: _jwtTokenOptions.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30), // 十分钟
                notBefore: null,
                signingCredentials: creds);
            var res = new JwtSecurityTokenHandler().WriteToken(token);
            return res;
        });

        return result;
    }
}