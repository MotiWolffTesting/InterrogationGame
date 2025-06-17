namespace InterrogationGame.Models;

public enum PersonalityType
{
    Aggressive,
    Deceptive,
    Fearful,
    Neutral
}

public class Person
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string FavoriteWeapon { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string SecretPhrase { get; set; } = string.Empty;
    public string Affiliation { get; set; } = string.Empty;
    public bool IsExposed { get; set; }
    public int? SuperiorId { get; set; }

    // AI & Behavior properties
    public PersonalityType Personality { get; set; } = PersonalityType.Neutral;
    public int InterrogationCount { get; set; } = 0;
    public DateTime LastInterrogation { get; set; } = DateTime.UtcNow;
    public bool HasLied { get; set; } = false;
    public List<string> PreviousResponses { get; set; } = new();
}