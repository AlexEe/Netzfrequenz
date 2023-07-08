using API.Data;
using API.Extensions;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddDbContext<DataContext>(opt => 
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapControllers();

// TO DO: Where does this go?
// Get latest frequency readings every x seconds and store in db.
async Task UpdateFrequencyReadings(TimeSpan timeSpan)
{
    var periodicTimer = new PeriodicTimer(timeSpan);
    while (await periodicTimer.WaitForNextTickAsync())
    {
        var latestFreq = 
        System.Console.WriteLine(latestFreq);
    }
}

await UpdateFrequencyReadings(TimeSpan.FromSeconds(5));

app.Run();
