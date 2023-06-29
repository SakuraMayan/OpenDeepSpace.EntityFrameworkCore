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




//Hangfire��ʹ��
builder.Services.AddHangfire(
        opt =>
        {
            opt.UseStorage(new MySqlStorage("Data Source=127.0.0.1;Initial Catalog=ods;User ID=root;Password=wy.023;Charset=utf8;Port=3306;Allow User Variables=true;", new MySqlStorageOptions()
            {

                TablesPrefix = "hangfire"
            }));

            //ע��ʹ����RecurringJobAttribute���Ե�������Job
            opt.RegisterRecurringJobs();

        }
    );

//���HangfireServer
builder.Services.AddHangfireServer(opt =>
{
    //���Hangfire���������

    opt.Queues = new[] { "default", "local", "recurringjobqueue" };//����һ��Ҫָ�� ���Jobָ���˶��� ����û���� Job���޷�ִ�� ��������ָ��ǰ�������м�default local

});

//ע�������Job
builder.Services.RegisterParametricJobs();
//���JobState״̬��� ���ڳɹ���ʧ��ִ�н���Ĵ���
GlobalJobFilters.Filters.Add(new JobStateFilter(builder.Services));
//���óɹ�ִ�е�Job�־û�ʱ��
GlobalStateHandlers.Handlers.Add(new SucceededJobExpiredHandler());




//sql���ӽ���
builder.Services.AddScoped<IConnectionStringResolver, ConnectionStringResolver>();

//sql��������
builder.Services.AddScoped<DbConnectionSharedSource>();

//���һ��������Options
builder.Services.AddScoped<DbContextOptions<CustomDbContext>>(serviceProvider =>
{
    var builder = new DbContextOptionsBuilder<CustomDbContext>();
    var dbConnectionSharedSource = serviceProvider.GetRequiredService<DbConnectionSharedSource>();
    //���������ַ���
    var connectionStringResolver = serviceProvider.GetRequiredService<IConnectionStringResolver>();
    string connectionString = connectionStringResolver.Resolve<CustomDbContext>();
    var connection = dbConnectionSharedSource[connectionString];
    if (connection == null) //���Ӳ�����
    {
        //����һ������
        connection = new MySqlConnection(connectionString);
        dbConnectionSharedSource[connectionString] = connection;
    }
    builder.UseMySql(connection, ServerVersion.AutoDetect(connection as MySqlConnection), null);//�����������������չ �����װ�Ļ�

    return builder.Options;

});

builder.Services.AddScoped<DbContextOptions<OtherDbContext>>(serviceProvider =>
{
    var builder = new DbContextOptionsBuilder<OtherDbContext>();
    var dbConnectionSharedSource = serviceProvider.GetRequiredService<DbConnectionSharedSource>();
    //���������ַ���
    var connectionStringResolver = serviceProvider.GetRequiredService<IConnectionStringResolver>();
    string connectionString = connectionStringResolver.Resolve<OtherDbContext>();
    var connection = dbConnectionSharedSource[connectionString];
    if (connection == null) //���Ӳ�����
    {
        //����һ������
        connection = new MySqlConnection(connectionString);
        dbConnectionSharedSource[connectionString] = connection;
    }
    builder.UseMySql(connection, ServerVersion.AutoDetect(connection as MySqlConnection), null);//�����������������չ �����װ�Ļ�

    return builder.Options;

});

//���������
builder.Services.AddScoped<CustomDbContext>();
builder.Services.AddScoped<OtherDbContext>();

//builder.Services.AddDbContext<CustomDbContext>(opt =>
//{
//    opt.UseMySql("server=localhost;uid=root;pwd=wy.023;port=3306;database=ods;Allow User Variables=True;IgnoreCommandTransaction=true",
//        ServerVersion.AutoDetect("server=localhost;uid=root;pwd=wy.023;port=3306;database=ods;Allow User Variables=True;IgnoreCommandTransaction=true"));
//}/*,ServiceLifetime.Transient*/);//ע��Ϊ˲ʱ���� ���Ƿ�Ṳ������ ���������� �Ṳ��

/*builder.Services.AddDbContext<OtherDbContext>(opt =>
{


    opt.UseMySql("server=localhost;uid=root;pwd=wy.023;port=3306;database=ods;Allow User Variables=True;IgnoreCommandTransaction=true",
        ServerVersion.AutoDetect("server=localhost;uid=root;pwd=wy.023;port=3306;database=ods;Allow User Variables=True;IgnoreCommandTransaction=true"));
}*//*,ServiceLifetime.Transient*//*);*/


//������Ԫע��
builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
builder.Services.AddScoped(typeof(IUnitOfWorkDbContextProvider<>), typeof(UnitOfWorkDbContextProvider<>));

//Ĭ�ϵ�������ָ��ע��
//builder.Services.AddScoped(typeof(IRepository<>).MakeGenericType(typeof(Role)), typeof(Repository<,>).MakeGenericType(typeof(CustomDbContext), typeof(Role)));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//��ӿ������ѡ�� ����Ȩ�� ��Ҫ��¼���ܷ��ʿ������
DashboardOptions dashboardOptions = new DashboardOptions();
dashboardOptions.Authorization = new[] { new BasicAuthorizationFilter("wy", "wy.023") };
//ʹ�ÿ������
app.UseHangfireDashboard(options: dashboardOptions);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
