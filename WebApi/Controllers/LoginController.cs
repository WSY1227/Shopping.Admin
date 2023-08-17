using Interface;
using Microsoft.AspNetCore.Mvc;
using Model.Dto.Login;
using Model.Dto.User;
using Model.Other;
using WebApi.Config;

namespace WebApi.Controllers;

[Route("api/[controller]/[action]")]
public class LoginController : ControllerBase
{
    private readonly ILogger<LoginController> _logger;
    private readonly IUserService _userService;
    private readonly ICustomJwtService _jwtService;

    public LoginController(ILogger<LoginController> logger, IUserService userService, ICustomJwtService jwtService)
    {
        _logger = logger;
        _userService = userService;
        _jwtService = jwtService;
    }

    public async Task<ApiResult> GetToken([FromBody] LoginReq req)
    {
        if (ModelState.IsValid)
        {
            var user = await _userService.GetUser(req);
            if (user == null)
            {
                return ResultHelper.Error("账号或密码错误");
            }

            _logger.LogInformation("登录");
            return ResultHelper.Success(await _jwtService.GetToken(user));
        }
        else
        {
            return ResultHelper.Error("参数格式错误");
        }
    }
}