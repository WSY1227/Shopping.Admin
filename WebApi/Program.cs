using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Model.Other;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SqlSugar;
using WebApi.Config;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // 设置标题和版本
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Admin.Api", Version = "v1" });
    // 设置对象类型参数的默认值
    options.SchemaFilter<DefaultValueSchemaFilter>();
    // 添加安全定义
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = "请输入token,格式为:Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    // 添加安全要求
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
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
    var tokenOptions = new JWTTokenOptions();
    builder.Configuration.Bind("JWTTokenOptions", tokenOptions);
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) //Scheme
        .AddJwtBearer(options => //这里是配置的鉴权的逻辑
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                //JWT有一些默认的属性，就是给鉴权时就可以筛选了
                ValidateIssuer = true, //是否验证Issuer
                ValidateAudience = true, //是否验证Audience
                ValidateLifetime = true, //是否验证失效时间
                ValidateIssuerSigningKey = true, //是否验证SecurityKey
                ValidAudience = tokenOptions.Audience, //
                ClockSkew = TimeSpan.FromSeconds(0), //设置token过期后多久失效，默认过期后300秒内仍有效
                ValidIssuer = tokenOptions.Issuer, //Issuer，这两项和前面签发jwt的设置一致
                IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecurityKey)) //拿到SecurityKey 
            };
        });
}

#endregion

// 设置JSON返回日期的格式
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    // 忽略循环引用
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    // 统一日期格式
    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
    // 设置Json序列化时的Key与model中属性名一致
    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
});
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