using Autofac;
using Autofac.Extensions.DependencyInjection;
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
