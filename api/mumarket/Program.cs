using System.Reflection;
using Microsoft.EntityFrameworkCore;
using mumarket;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<MuMarketDbContext>(o => o.UseSqlite("Data Source=mydb.db"));
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(x =>
{
    x.AllowAnyOrigin();
});
app.UseAuthorization();

app.MapControllers();

app.Run();
