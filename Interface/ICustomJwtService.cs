using Model.Dto.User;

namespace Interface;

public interface ICustomJwtService
{
    // 获取token
    Task<string> GetToken(UserRes userRes);
}