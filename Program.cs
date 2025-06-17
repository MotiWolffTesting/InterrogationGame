using InterrogationGame.Data;
using InterrogationGame.Services;
using InterrogationGame.Game;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var configuration = builder.Build();

var services = new ServiceCollection();

// Configure services
var connectionString = configuration.GetConnectionString("DefaultConnection");
services.AddDbContext<GameDbContext>(options =>
    options.UseNpgsql(connectionString));

services.AddScoped<DatabaseService>();
services.AddSingleton<GameManager>();

var serviceProvider = services.BuildServiceProvider();

// Ensure database is created and seeded
using (var scope = serviceProvider.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GameDbContext>();
    await context.Database.EnsureCreatedAsync();
    await SeedData.InitializeAsync(context);
}

var gameManager = serviceProvider.GetRequiredService<GameManager>();
var dbService = serviceProvider.GetRequiredService<DatabaseService>();
var consoleGame = new ConsoleGame(gameManager, dbService);
await consoleGame.RunAsync();
