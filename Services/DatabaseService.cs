using InterrogationGame.Models;
using InterrogationGame.Data;
using Microsoft.EntityFrameworkCore;

namespace InterrogationGame.Services;

public class PersonDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? SuperiorId { get; set; }
}

public class DatabaseService
{
    private readonly GameDbContext _context;
    private readonly RealAIBehaviorService _aiService;

    public DatabaseService(GameDbContext context, RealAIBehaviorService aiService)
    {
        _context = context;
        _aiService = aiService;
    }

    public async Task<Person?> GetRandomUnexposedPerson()
    {
        var person = await _context.People
            .Where(p => !p.IsExposed)
            .OrderBy(p => EF.Functions.Random())
            .FirstOrDefaultAsync();

        if (person != null && person.Personality == PersonalityType.Neutral)
        {
            _aiService.AssignRandomPersonality(person);
            await _context.SaveChangesAsync();
        }

        return person;
    }

    public async Task<Person?> GetPersonById(int personId)
    {
        return await _context.People.FindAsync(personId);
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

    public async Task<string?> GetSuperiorClue(int personId)
    {
        var person = await _context.People.FindAsync(personId);
        if (person == null || person.SuperiorId == null) return null;
        var superior = await _context.People.FindAsync(person.SuperiorId);
        if (superior == null) return null;
        // Example clue: favorite weapon
        return $"My superior's favorite weapon is {superior.FavoriteWeapon}.";
    }

    public async Task<bool> CheckHierarchy(List<int> submittedIds)
    {
        if (submittedIds == null || submittedIds.Count < 2)
            return false;
        for (int i = 0; i < submittedIds.Count - 1; i++)
        {
            var person = await _context.People.FindAsync(submittedIds[i]);
            if (person == null || person.SuperiorId != submittedIds[i + 1])
                return false;
        }
        // The last person should be the leader (no superior)
        var last = await _context.People.FindAsync(submittedIds.Last());
        return last != null && last.SuperiorId == null;
    }

    public async Task SaveScore(string playerName, int points)
    {
        var score = new Score
        {
            PlayerName = playerName,
            Points = points,
            Timestamp = DateTime.UtcNow
        };
        _context.Scores.Add(score);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Score>> GetTopScores(int count = 10)
    {
        return await _context.Scores
            .OrderByDescending(s => s.Points)
            .ThenBy(s => s.Timestamp)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<PersonDto>> GetAllPeople()
    {
        return await _context.People
            .Select(p => new PersonDto
            {
                Id = p.Id,
                Name = p.FirstName + " " + p.LastName,
                SuperiorId = p.SuperiorId
            })
            .ToListAsync();
    }
}