using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using OpenDeepSpace.EntityFrameworkCore;
using OpenDeepSpace.NetCore.Hangfire.Extensions;
using OpenDeepSpace.NetCore.Hangfire;
using OpenDeepSpaceEntityFrameworkCore.Test;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Hangfire.MySql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




//Hangfire的使用
builder.Services.AddHangfire(
        opt =>
        {
            opt.UseStorage(new MySqlStorage("Data Source=127.0.0.1;Initial Catalog=ods;User ID=root;Password=wy.023;Charset=utf8;Port=3306;Allow User Variables=true;", new MySqlStorageOptions()
            {

                TablesPrefix = "hangfire"
            }));

            //注册使用了RecurringJobAttribute特性的周期性Job
            opt.RegisterRecurringJobs();

        }
    );

//添加HangfireServer
builder.Services.AddHangfireServer(opt =>
{
    //添加Hangfire服务的配置

    opt.Queues = new[] { "default", "local", "recurringjobqueue" };//队列一定要指定 如果Job指定了队列 这里没加入 Job将无法执行 建议至少指定前两个队列即default local

});

//注册参数化Job
builder.Services.RegisterParametricJobs();
//添加JobState状态监控 用于成功或失败执行结果的处理
GlobalJobFilters.Filters.Add(new JobStateFilter(builder.Services));
//设置成功执行的Job持久化时间
GlobalStateHandlers.Handlers.Add(new SucceededJobExpiredHandler());




//sql连接解析
builder.Services.AddScoped<IConnectionStringResolver, ConnectionStringResolver>();

//sql共享连接
builder.Services.AddScoped<DbConnectionSharedSource>();

//添加一个上下文Options
builder.Services.AddScoped<DbContextOptions<CustomDbContext>>(serviceProvider =>
{
    var builder = new DbContextOptionsBuilder<CustomDbContext>();
    var dbConnectionSharedSource = serviceProvider.GetRequiredService<DbConnectionSharedSource>();
    //解析连接字符串
    var connectionStringResolver = serviceProvider.GetRequiredService<IConnectionStringResolver>();
    string connectionString = connectionStringResolver.Resolve<CustomDbContext>();
    var connection = dbConnectionSharedSource[connectionString];
    if (connection == null) //连接不存在
    {
        //构造一个连接
        connection = new MySqlConnection(connectionString);
        dbConnectionSharedSource[connectionString] = connection;
    }
    builder.UseMySql(connection, ServerVersion.AutoDetect(connection as MySqlConnection), null);//这里第三个参数可拓展 如果封装的话

    return builder.Options;

});

builder.Services.AddScoped<DbContextOptions<OtherDbContext>>(serviceProvider =>
{
    var builder = new DbContextOptionsBuilder<OtherDbContext>();
    var dbConnectionSharedSource = serviceProvider.GetRequiredService<DbConnectionSharedSource>();
    //解析连接字符串
    var connectionStringResolver = serviceProvider.GetRequiredService<IConnectionStringResolver>();
    string connectionString = connectionStringResolver.Resolve<OtherDbContext>();
    var connection = dbConnectionSharedSource[connectionString];
    if (connection == null) //连接不存在
    {
        //构造一个连接
        connection = new MySqlConnection(connectionString);
        dbConnectionSharedSource[connectionString] = connection;
    }
    builder.UseMySql(connection, ServerVersion.AutoDetect(connection as MySqlConnection), null);//这里第三个参数可拓展 如果封装的话

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


//添加控制面板选项 设置权限 需要登录才能访问控制面板
DashboardOptions dashboardOptions = new DashboardOptions();
dashboardOptions.Authorization = new[] { new BasicAuthorizationFilter("wy", "wy.023") };
//使用控制面板
app.UseHangfireDashboard(options: dashboardOptions);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
