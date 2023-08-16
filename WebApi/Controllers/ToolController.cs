using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Model.Entitys;
using SqlSugar;

namespace WebApi.Controllers;

[Route("api/[controller]/[action]")]
public class ToolController : ControllerBase
{
    private readonly ISqlSugarClient _db;

    public ToolController(ISqlSugarClient db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<bool> CodeFirst()
    {
        // 1.创建数据库
        _db.DbMaintenance.CreateDatabase();

        // 2.通过反射，加载程序集，读取所有的实体类，然后根据实体类创建表
        var nspace = "Model.Entitys";
        var ass = Assembly.LoadFrom(AppContext.BaseDirectory + "Model.dll")
            .GetTypes()
            .Where(t => t.Namespace == nspace)
            .ToArray();
        _db.CodeFirst.SetStringDefaultLength(200).InitTables(ass);
        //初始化炒鸡管理员和菜单
        var user = new Users()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "admin",
            NickName = "炒鸡管理员",
            PassWord = "123456",
            UserType = 0,
            IsEnable = true,
            Description = "数据库初始化时默认添加的炒鸡管理员",
            CreateDate = DateTime.Now,
            CreateUserId = "",
        };
        var userId = (await _db.Insertable(user).ExecuteReturnEntityAsync()).Id;
        var m1 = new Menu()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "菜单管理",
            Index = "/menu",
            FilePath = "menu.vue",
            ParentId = "",
            Order = 1,
            IsEnable = true,
            Icon = "folder",
            Description = "数据库初始化时默认添加的默认菜单",
            CreateDate = DateTime.Now,
            CreateUserId = userId
        };
        var mid1 = (await _db.Insertable(m1).ExecuteReturnEntityAsync()).Id;
        var m11 = new Menu()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "菜单列表",
            Index = "/menu",
            FilePath = "menu.vue",
            ParentId = mid1,
            Order = 1,
            IsEnable = true,
            Icon = "notebook",
            Description = "数据库初始化时默认添加的默认菜单",
            CreateDate = DateTime.Now,
            CreateUserId = userId
        };
        await _db.Insertable(m11).ExecuteReturnEntityAsync();

        var m2 = new Menu()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "角色管理",
            Index = "/role",
            FilePath = "role.vue",
            ParentId = "",
            Order = 1,
            IsEnable = true,
            Icon = "folder",
            Description = "数据库初始化时默认添加的默认菜单",
            CreateDate = DateTime.Now,
            CreateUserId = userId
        };
        var mid2 = (await _db.Insertable(m2).ExecuteReturnEntityAsync()).Id;
        var m22 = new Menu()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "角色列表",
            Index = "/role",
            FilePath = "role.vue",
            ParentId = mid2,
            Order = 1,
            IsEnable = true,
            Icon = "notebook",
            Description = "数据库初始化时默认添加的默认菜单",
            CreateDate = DateTime.Now,
            CreateUserId = userId
        };
        await _db.Insertable(m22).ExecuteReturnEntityAsync();

        var m3 = new Menu()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "用户管理",
            Index = "/user",
            FilePath = "user.vue",
            ParentId = "",
            Order = 1,
            IsEnable = true,
            Icon = "folder",
            Description = "数据库初始化时默认添加的默认菜单",
            CreateDate = DateTime.Now,
            CreateUserId = userId
        };
        var mid3 = (await _db.Insertable(m3).ExecuteReturnEntityAsync()).Id;
        var m33 = new Menu()
        {
            Id = Guid.NewGuid().ToString(),
            Name = "用户列表",
            Index = "/user",
            FilePath = "user.vue",
            ParentId = mid3,
            Order = 1,
            IsEnable = true,
            Icon = "notebook",
            Description = "数据库初始化时默认添加的默认菜单",
            CreateDate = DateTime.Now,
            CreateUserId = userId
        };
        return await _db.Insertable(m33).ExecuteCommandIdentityIntoEntityAsync();
    }
}