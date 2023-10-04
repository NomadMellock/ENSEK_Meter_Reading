using ENSEK_Meter_Reading.Data;
using ENSEK_Meter_Reading.Data.Seeding;
using ENSEK_Meter_Reading.Services;
using ENSEK_Meter_Reading.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Entity Framework Core Db Contexts
builder.Services.AddDbContext<MeterReadingContext>();
// Add Services
builder.Services.AddScoped<IFileService, FileService>();
// Data Seeding
builder.Services.AddTransient<SeedData>();

var app = builder.Build();

if (args.Length == 1 && args[0].ToLower() == "seeddata")
    SeedData(app);

void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    if (scopedFactory != null)
    {
        using (var scope = scopedFactory.CreateScope())
        {
            var service = scope.ServiceProvider.GetService<SeedData>();
            if (service != null)
            {
                service.Seed();
            }
        }
    }
}

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
