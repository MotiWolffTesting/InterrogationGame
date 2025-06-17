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

        // Step 1: Create the leader
        var leader = new Person
        {
            FirstName = "Ali",
            LastName = "Reza",
            Location = "Tehran",
            FavoriteWeapon = "FN FAL",
            ContactNumber = "+98-21-1234",
            SecretPhrase = "The lion roars at dawn",
            Affiliation = "Quds Force",
            IsExposed = false
        };
        context.People.Add(leader);
        await context.SaveChangesAsync();

        // Step 2: Create 2 squad leaders, each reporting to the leader
        var squadLeaders = new List<Person>
        {
            new Person
            {
                FirstName = "Hassan",
                LastName = "Jafari",
                Location = "Shiraz",
                FavoriteWeapon = "M16",
                ContactNumber = "+98-71-5678",
                SecretPhrase = "The river runs deep",
                Affiliation = "Quds Force",
                IsExposed = false,
                SuperiorId = leader.Id
            },
            new Person
            {
                FirstName = "Omid",
                LastName = "Karimi",
                Location = "Isfahan",
                FavoriteWeapon = "Steyr AUG",
                ContactNumber = "+98-31-4321",
                SecretPhrase = "The falcon circles high",
                Affiliation = "Quds Force",
                IsExposed = false,
                SuperiorId = leader.Id
            }
        };
        context.People.AddRange(squadLeaders);
        await context.SaveChangesAsync();

        // Step 3: Create 3 foot soldiers, each reporting to a squad leader
        var footSoldiers = new List<Person>
        {
            new Person
            {
                FirstName = "Reza",
                LastName = "Moradi",
                Location = "Tabriz",
                FavoriteWeapon = "AK-47",
                ContactNumber = "+98-41-8765",
                SecretPhrase = "The wind whispers east",
                Affiliation = "Quds Force",
                IsExposed = false,
                SuperiorId = squadLeaders[0].Id
            },
            new Person
            {
                FirstName = "Saeed",
                LastName = "Rahimi",
                Location = "Mashhad",
                FavoriteWeapon = "G3",
                ContactNumber = "+98-51-3456",
                SecretPhrase = "The night is silent",
                Affiliation = "Quds Force",
                IsExposed = false,
                SuperiorId = squadLeaders[0].Id
            },
            new Person
            {
                FirstName = "Mehdi",
                LastName = "Farhadi",
                Location = "Ahvaz",
                FavoriteWeapon = "Uzi",
                ContactNumber = "+98-61-6543",
                SecretPhrase = "The desert hides secrets",
                Affiliation = "Quds Force",
                IsExposed = false,
                SuperiorId = squadLeaders[1].Id
            }
        };
        context.People.AddRange(footSoldiers);
        await context.SaveChangesAsync();
    }
}