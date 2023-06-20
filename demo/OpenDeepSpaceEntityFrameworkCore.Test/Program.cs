using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using OpenDeepSpace.EntityFrameworkCore;
using OpenDeepSpaceEntityFrameworkCore.Test;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();






//sql共享连接
builder.Services.AddScoped<DbConnectionSharedSource>();

//添加一个上下文Options
builder.Services.AddScoped<DbContextOptions<CustomDbContext>>(serviceProvider =>
{
    var builder = new DbContextOptionsBuilder<CustomDbContext>();
    var dbConnectionSharedSource = serviceProvider.GetRequiredService<DbConnectionSharedSource>();
    var connection = dbConnectionSharedSource["server=localhost;uid=root;pwd=wy.023;port=3306;database=ods;Allow User Variables=True;IgnoreCommandTransaction=true"];
    if (connection == null) //连接不存在
    {
        //构造一个连接
        connection = new MySqlConnection("server=localhost;uid=root;pwd=wy.023;port=3306;database=ods;Allow User Variables=True;IgnoreCommandTransaction=true");
        dbConnectionSharedSource["server=localhost;uid=root;pwd=wy.023;port=3306;database=ods;Allow User Variables=True;IgnoreCommandTransaction=true"] = connection;
    }
    builder.UseMySql(connection, ServerVersion.AutoDetect(connection as MySqlConnection), null);//这里第三个参数可拓展 如果封装的话

    return builder.Options;

});

builder.Services.AddScoped<DbContextOptions<OtherDbContext>>(serviceProvider =>
{
    var builder = new DbContextOptionsBuilder<OtherDbContext>();
    var dbConnectionSharedSource = serviceProvider.GetRequiredService<DbConnectionSharedSource>();
    var connection = dbConnectionSharedSource["server=localhost;uid=root;pwd=wy.023;port=3306;database=ods;Allow User Variables=True;IgnoreCommandTransaction=true"];
    if (connection == null) //连接不存在
    {
        //构造一个连接
        connection = new MySqlConnection("server=localhost;uid=root;pwd=wy.023;port=3306;database=ods;Allow User Variables=True;IgnoreCommandTransaction=true");
        dbConnectionSharedSource["server=localhost;uid=root;pwd=wy.023;port=3306;database=ods;Allow User Variables=True;IgnoreCommandTransaction=true"] = connection;
    }
    builder.UseMySql(connection, ServerVersion.AutoDetect(connection as MySqlConnection),null);//这里第三个参数可拓展 如果封装的话

    return builder.Options;

});

//添加上下文
builder.Services.AddScoped<CustomDbContext>();
builder.Services.AddScoped<OtherDbContext>();

//builder.Services.AddDbContext<CustomDbContext>(opt =>
//{
//    opt.UseMySql("server=localhost;uid=root;pwd=wy.023;port=3306;database=ods;Allow User Variables=True;IgnoreCommandTransaction=true",
//        ServerVersion.AutoDetect("server=localhost;uid=root;pwd=wy.023;port=3306;database=ods;Allow User Variables=True;IgnoreCommandTransaction=true"));
//}/*,ServiceLifetime.Transient*/);//注册为瞬时类型 看是否会共享事务 单个上下文 会共享

/*builder.Services.AddDbContext<OtherDbContext>(opt =>
{


    opt.UseMySql("server=localhost;uid=root;pwd=wy.023;port=3306;database=ods;Allow User Variables=True;IgnoreCommandTransaction=true",
        ServerVersion.AutoDetect("server=localhost;uid=root;pwd=wy.023;port=3306;database=ods;Allow User Variables=True;IgnoreCommandTransaction=true"));
}*//*,ServiceLifetime.Transient*//*);*/


//工作单元注入
builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
builder.Services.AddScoped(typeof(IUnitOfWorkDbContextProvider<>), typeof(UnitOfWorkDbContextProvider<>));

//默认的上下文指定注入
//builder.Services.AddScoped(typeof(IRepository<>).MakeGenericType(typeof(Role)), typeof(Repository<,>).MakeGenericType(typeof(CustomDbContext), typeof(Role)));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
