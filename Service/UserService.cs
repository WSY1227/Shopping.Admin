using Interface;
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
}