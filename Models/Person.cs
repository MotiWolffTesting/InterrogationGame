namespace InterrogationGame.Models;

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
}