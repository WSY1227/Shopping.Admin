using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Model.Other;
using SqlSugar;
using WebApi.Config;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// 替换默认的容器
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    #region 通过模块化注册接口和实现类
    containerBuilder.RegisterModule(new AutofacModuleRegister());
    #endregion

    #region sqlSugar注入

    containerBuilder.Register<ISqlSugarClient>(_ =>
    {
        var db = new SqlSugarClient(new ConnectionConfig()
        {
            ConnectionString = builder.Configuration.GetConnectionString("conn"),
            DbType = DbType.MySql,
            IsAutoCloseConnection = true
        });
        return db;
    });

    #endregion
});
// 注册AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperConfigs));

builder.Services.Configure<JWTTokenOptions>(builder.Configuration.GetSection("JWTTokenOptions"));

#region jwt校验 
{
    //第二步，增加鉴权逻辑
    JWTTokenOptions tokenOptions = new JWTTokenOptions();
    builder.Configuration.Bind("JWTTokenOptions", tokenOptions);
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//Scheme
        .AddJwtBearer(options =>  //这里是配置的鉴权的逻辑
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                //JWT有一些默认的属性，就是给鉴权时就可以筛选了
                ValidateIssuer = true,//是否验证Issuer
                ValidateAudience = true,//是否验证Audience
                ValidateLifetime = true,//是否验证失效时间
                ValidateIssuerSigningKey = true,//是否验证SecurityKey
                ValidAudience = tokenOptions.Audience,//
                ClockSkew = TimeSpan.FromSeconds(0),//设置token过期后多久失效，默认过期后300秒内仍有效
                ValidIssuer = tokenOptions.Issuer,//Issuer，这两项和前面签发jwt的设置一致
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey))//拿到SecurityKey 
            };
        });
}
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
