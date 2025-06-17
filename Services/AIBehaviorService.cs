using InterrogationGame.Models;

namespace InterrogationGame.Services;

public class AIBehaviorService
{
    private readonly Random _random = new();

    public class InterrogationResult
    {
        public bool IsLying { get; set; }
        public string Response { get; set; } = string.Empty;
        public bool UnlocksHint { get; set; }
        public string? Hint { get; set; }
        public PersonalityType Personality { get; set; }
    }

    public InterrogationResult ProcessInterrogation(Person person, string question, int turnNumber)
    {
        person.InterrogationCount++;
        person.LastInterrogation = DateTime.UtcNow;

        var result = new InterrogationResult
        {
            Personality = person.Personality,
            IsLying = DetermineIfLying(person, turnNumber),
            UnlocksHint = DetermineHintUnlock(person, turnNumber)
        };

        result.Response = GenerateResponse(person, question, result.IsLying, turnNumber);

        if (result.UnlocksHint)
        {
            result.Hint = GenerateHint(person, turnNumber);
        }

        person.PreviousResponses.Add(result.Response);
        person.HasLied = person.HasLied || result.IsLying;

        return result;
    }

    private bool DetermineIfLying(Person person, int turnNumber)
    {
        return person.Personality switch
        {
            PersonalityType.Deceptive => _random.Next(100) < 70, // 70% chance to lie
            PersonalityType.Aggressive => _random.Next(100) < 40, // 40% chance to lie
            PersonalityType.Fearful => _random.Next(100) < 60, // 60% chance to lie when pressured
            _ => _random.Next(100) < 20 // 20% chance for neutral
        };
    }

    private bool DetermineHintUnlock(Person person, int turnNumber)
    {
        // Unlock hints based on interrogation count and personality
        return person.Personality switch
        {
            PersonalityType.Fearful => person.InterrogationCount >= 2, // Fearful people break easily
            PersonalityType.Aggressive => person.InterrogationCount >= 4, // Aggressive people are stubborn
            PersonalityType.Deceptive => person.InterrogationCount >= 3, // Deceptive people need more pressure
            _ => person.InterrogationCount >= 3 // Neutral baseline
        };
    }

    private string GenerateResponse(Person person, string question, bool isLying, int turnNumber)
    {
        var baseResponse = person.Personality switch
        {
            PersonalityType.Aggressive => GenerateAggressiveResponse(question),
            PersonalityType.Deceptive => GenerateDeceptiveResponse(question),
            PersonalityType.Fearful => GenerateFearfulResponse(question),
            _ => GenerateNeutralResponse(question)
        };

        if (isLying)
        {
            return ApplyLyingModification(baseResponse, person);
        }

        return baseResponse;
    }

    private string GenerateAggressiveResponse(string question)
    {
        var responses = new[]
        {
            "I won't tell you anything!",
            "You'll get nothing from me!",
            "I'd rather die than help you!",
            "Go to hell!",
            "I'm not afraid of you!"
        };
        return responses[_random.Next(responses.Length)];
    }

    private string GenerateDeceptiveResponse(string question)
    {
        var responses = new[]
        {
            "I don't know what you're talking about...",
            "Maybe I heard something, but I'm not sure...",
            "I think I might have seen something...",
            "Let me think about that...",
            "I'm not really involved in all that..."
        };
        return responses[_random.Next(responses.Length)];
    }

    private string GenerateFearfulResponse(string question)
    {
        var responses = new[]
        {
            "Please don't hurt me...",
            "I... I don't want any trouble...",
            "Maybe I can help, but I'm scared...",
            "I don't know much, I swear!",
            "Please, I have a family..."
        };
        return responses[_random.Next(responses.Length)];
    }

    private string GenerateNeutralResponse(string question)
    {
        var responses = new[]
        {
            "I don't have much to say.",
            "I'm not sure what you want to know.",
            "I keep to myself mostly.",
            "I don't get involved in politics.",
            "I just work here."
        };
        return responses[_random.Next(responses.Length)];
    }

    private string ApplyLyingModification(string baseResponse, Person person)
    {
        // Add lying indicators based on personality
        return person.Personality switch
        {
            PersonalityType.Deceptive => $"{baseResponse} (nervous laughter)",
            PersonalityType.Fearful => $"{baseResponse} (stuttering)",
            PersonalityType.Aggressive => $"{baseResponse} (defensive tone)",
            _ => $"{baseResponse} (slight hesitation)"
        };
    }

    private string GenerateHint(Person person, int turnNumber)
    {
        return person.Personality switch
        {
            PersonalityType.Fearful => $"I heard someone mention {person.Location}...",
            PersonalityType.Aggressive => $"Fine! The leader uses a {person.FavoriteWeapon}!",
            PersonalityType.Deceptive => $"Maybe check the {person.ContactNumber} area...",
            _ => $"I think I saw something in {person.Location}..."
        };
    }

    public void AssignRandomPersonality(Person person)
    {
        person.Personality = (PersonalityType)_random.Next(4);
    }
}