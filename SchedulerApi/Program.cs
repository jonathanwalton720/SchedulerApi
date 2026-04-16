using SchedulerApi.Services;
using Microsoft.EntityFrameworkCore;
using SchedulerDb;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Entity Framework Core with SQL Server
builder.Services.AddDbContext<SchedulerDbContext>(options =>
    options.UseSqlServer("Server=(local)\\MSSQLSERVER01;Database=SchedulerDb;Integrated Security=true;TrustServerCertificate=true;"));

// Register Repository
builder.Services.AddScoped<IRepository, Repository>();

// Configure logging
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

// Auto-migrate on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<SchedulerDbContext>();
    dbContext.Database.Migrate();
}

app.Run();
