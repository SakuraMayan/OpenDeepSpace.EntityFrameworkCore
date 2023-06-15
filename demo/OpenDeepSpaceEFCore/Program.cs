using Microsoft.EntityFrameworkCore;
using OpenDeepSpaceEFCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CustomDbContext>(opt =>
{
    opt.UseMySql("server=localhost;uid=root;pwd=wy.023;port=3306;database=ods;Allow User Variables=True;IgnoreCommandTransaction=true",
        ServerVersion.AutoDetect("server=localhost;uid=root;pwd=wy.023;port=3306;database=ods;Allow User Variables=True;IgnoreCommandTransaction=true"));
});

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
