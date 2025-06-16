using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace InterrogationGame.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<GameDbContext>
{
    public GameDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GameDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=terrorist_game;Username=postgres;Password=your_password");

        return new GameDbContext(optionsBuilder.Options);
    }
}
