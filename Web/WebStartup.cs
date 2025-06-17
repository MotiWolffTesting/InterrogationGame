using InterrogationGame.Data;
using InterrogationGame.Services;
using InterrogationGame.Game;
using Microsoft.EntityFrameworkCore;

namespace InterrogationGame.Web;

public static class WebStartup
{
    public static WebApplication CreateWebApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Configure database
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<GameDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Add game services
        builder.Services.AddScoped<DatabaseService>();
        builder.Services.AddScoped<RealAIBehaviorService>();
        builder.Services.AddSingleton<GameManager>();

        // Add CORS for web client
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("AllowAll");
        app.UseAuthorization();

        // Add static file serving
        app.UseStaticFiles();

        app.MapControllers();

        // Map the default route to serve index.html
        app.MapFallbackToFile("index.html");

        return app;
    }
}