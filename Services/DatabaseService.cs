using InterrogationGame.Models;
using InterrogationGame.Data;
using Microsoft.EntityFrameworkCore;

namespace InterrogationGame.Services;

public class DatabaseService
{
    private readonly GameDbContext _context;

    public DatabaseService(GameDbContext context)
    {
        _context = context;
    }

    public async Task<Person?> GetRandomUnexposedPerson()
    {
        return await _context.People
            .Where(p => !p.IsExposed)
            .OrderBy(p => EF.Functions.Random())
            .FirstOrDefaultAsync();
    }

    public async Task UpdatePersonExposedStatus(int personId, bool isExposed)
    {
        var person = await _context.People.FindAsync(personId);
        if (person != null)
        {
            person.IsExposed = isExposed;
            await _context.SaveChangesAsync();
        }
    }

    public async Task LogGameAction(int personId, string action, string details)
    {
        var log = new GameLog
        {
            PersonId = personId,
            Action = action,
            Details = details,
            Timestamp = DateTime.UtcNow
        };

        _context.GameLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task EnsureDatabaseCreated()
    {
        await _context.Database.EnsureCreatedAsync();
    }
}