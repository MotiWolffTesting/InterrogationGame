namespace InterrogationGame.Models;

public class GameLog
{
    public int Id { get; set; }
    public int PersonId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public class Score
{
    public int Id { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public int Points { get; set; }
    public DateTime Timestamp { get; set; }
}