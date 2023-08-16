using Interface;
using Microsoft.Extensions.Options;
using Model.Dto.User;
using Model.Other;

namespace Service;

public class CustomJwtService:ICustomJwtService
{
    private readonly JWTTokenOptions _jwtTokenOptions;

    public CustomJwtService(IOptionsMonitor<JWTTokenOptions> jwtTokenOptions)
    {
        _jwtTokenOptions = jwtTokenOptions.CurrentValue;
    }

    
}