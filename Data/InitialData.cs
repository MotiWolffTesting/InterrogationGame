using InterrogationGame.Models;
using Microsoft.EntityFrameworkCore;

namespace InterrogationGame.Data;

public static class SeedData
{
    public static async Task InitializeAsync(GameDbContext context)
    {
        if (await context.People.AnyAsync())
        {
            return; // Database has been seeded
        }

        var people = new List<Person>
        {
            new Person
            {
                FirstName = "Assaf",
                LastName = "Lutz",
                Location = "New York",
                FavoriteWeapon = "AK-47",
                ContactNumber = "+1-555-0123",
                SecretPhrase = "The eagle flies at midnight",
                Affiliation = "Shaldag",
                IsExposed = false
            },
            new Person
            {
                FirstName = "Moti",
                LastName = "Luchim",
                Location = "Miami",
                FavoriteWeapon = "Glock 17",
                ContactNumber = "+1-555-0124",
                SecretPhrase = "Red sky at night",
                Affiliation = "Maglan",
                IsExposed = false
            },
            new Person
            {
                FirstName = "Natan",
                LastName = "Lahem Barosh",
                Location = "Dubai",
                FavoriteWeapon = "MP5",
                ContactNumber = "+971-555-0125",
                SecretPhrase = "Desert wind whispers",
                Affiliation = "Sayeret Matkal",
                IsExposed = false
            },
            new Person
            {
                FirstName = "Sarah",
                LastName = "Cohen",
                Location = "Tel Aviv",
                FavoriteWeapon = "Uzi",
                ContactNumber = "+972-555-0126",
                SecretPhrase = "Mountain shadows dance",
                Affiliation = "Delta Unit",
                IsExposed = false
            },
            new Person
            {
                FirstName = "Vladimir",
                LastName = "Petrov",
                Location = "Moscow",
                FavoriteWeapon = "Dragunov",
                ContactNumber = "+7-555-0127",
                SecretPhrase = "Winter's first snow",
                Affiliation = "Epsilon Team",
                IsExposed = false
            }
        };

        await context.People.AddRangeAsync(people);
        await context.SaveChangesAsync();
    }
}