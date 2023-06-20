using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenDeepSpace.EntityFrameworkCore;
using OpenDeepSpaceEntityFrameworkCore.Test;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();






//sql��������
builder.Services.AddScoped<DbConnectionSharedSource>();

//���һ��������Options
builder.Services.AddScoped<DbContextOptions<CustomDbContext>>(serviceProvider =>
{
    var builder = new DbContextOptionsBuilder<CustomDbContext>();
    var source = serviceProvider.GetRequiredService<DbConnectionSharedSource>();
    var connection = source["server=localhost;uid=root;pwd=wy.023;port=3306;database=ods;Allow User Variables=True;IgnoreCommandTransaction=true"];
    builder.UseMySql(ServerVersion.AutoDetect(connection.ConnectionString));

    return builder.Options;

});

builder.Services.AddScoped<DbContextOptions<OtherDbContext>>(serviceProvider =>
{
    var builder = new DbContextOptionsBuilder<OtherDbContext>();
    var source = serviceProvider.GetRequiredService<DbConnectionSharedSource>();
    var connection = source["server=localhost;uid=root;pwd=wy.023;port=3306;database=ods;Allow User Variables=True;IgnoreCommandTransaction=true"];
    builder.UseMySql(ServerVersion.AutoDetect(connection.ConnectionString));

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
