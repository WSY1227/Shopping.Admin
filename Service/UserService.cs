using Interface;
using Microsoft.IdentityModel.Tokens;
using Model.Dto.Login;
using Model.Dto.User;
using Model.Entitys;
using SqlSugar;

namespace Service;

public class UserService : IUserService
{
    private ISqlSugarClient _db;

    public UserService(ISqlSugarClient db)
    {
        _db = db;
    }

    public async Task<UserRes> GetUser(LoginReq req)
    {
        var user = await _db.Queryable<Users>()
            .Where(p => p.Name == req.UserName && p.PassWord == req.PassWord)
            .Select(p => new UserRes(), true).FirstAsync();
        return user;
    }

    public async Task<bool> EditNickNameOrPassword(string userId, PersonEdit req)
    {
        var info = await _db.Queryable<Users>().FirstAsync(p => p.Id == userId);

        if (info == null) return false;
        // 不为空则修改，为空不修改
        if (!req.NickName.IsNullOrEmpty())
        {
            info.NickName = req.NickName;
        }

        if (!req.Password.IsNullOrEmpty())
        {
            info.PassWord = req.Password;
        }

        if (req.Image.IsNullOrEmpty())
        {
            info.Image = req.Image;
        }

        return await _db.Updateable(info).ExecuteCommandHasChangeAsync();

    }
}