using InterrogationGame.Models;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels;
using Microsoft.Extensions.Configuration;

namespace InterrogationGame.Services;

public class RealAIBehaviorService
{
    private readonly OpenAIService _openAIService;
    private readonly IConfiguration _configuration;

    public RealAIBehaviorService(IConfiguration configuration)
    {
        _configuration = configuration;
        var apiKey = _configuration["OpenAI:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("OpenAI API key not found in configuration");
        }
        _openAIService = new OpenAIService(new OpenAiOptions { ApiKey = apiKey });
    }

    public class InterrogationResult
    {
        public bool IsLying { get; set; }
        public string Response { get; set; } = string.Empty;
        public bool UnlocksHint { get; set; }
        public string? Hint { get; set; }
        public PersonalityType Personality { get; set; }
        public double Confidence { get; set; }
        public string Reasoning { get; set; } = string.Empty;
    }

    public async Task<InterrogationResult> ProcessInterrogation(Person person, string question, int turnNumber)
    {
        person.InterrogationCount++;
        person.LastInterrogation = DateTime.UtcNow;

        var result = new InterrogationResult
        {
            Personality = person.Personality
        };

        // Create AI prompt based on person's background and personality
        var prompt = CreateInterrogationPrompt(person, question, turnNumber);

        try
        {
            var aiResponse = await GetAIResponse(prompt);
            var parsedResponse = await ParseAIResponse(aiResponse, person, turnNumber);

            result.Response = parsedResponse.Response;
            result.IsLying = parsedResponse.IsLying;
            result.UnlocksHint = parsedResponse.UnlocksHint;
            result.Hint = parsedResponse.Hint;
            result.Confidence = parsedResponse.Confidence;
            result.Reasoning = parsedResponse.Reasoning;
        }
        catch (Exception ex)
        {
            // Fallback to basic responses if AI fails
            result.Response = GetFallbackResponse(person, question);
            result.IsLying = DetermineIfLying(person, turnNumber);
            result.UnlocksHint = DetermineHintUnlock(person, turnNumber);
            result.Hint = result.UnlocksHint ? GenerateHint(person, turnNumber) : null;
            result.Confidence = 0.5;
            result.Reasoning = "AI service unavailable, using fallback";
        }

        person.PreviousResponses.Add(result.Response);
        person.HasLied = person.HasLied || result.IsLying;

        return result;
    }

    private string CreateInterrogationPrompt(Person person, string question, int turnNumber)
    {
        var personalityDescription = person.Personality switch
        {
            PersonalityType.Aggressive => "aggressive and defiant, likely to resist interrogation",
            PersonalityType.Deceptive => "deceptive and manipulative, may lie or give misleading information",
            PersonalityType.Fearful => "fearful and anxious, may break under pressure",
            _ => "neutral and cautious, neither particularly cooperative nor resistant"
        };

        return $@"You are role-playing as a terrorist suspect being interrogated. 

CHARACTER BACKGROUND:
- Name: {person.FirstName} {person.LastName}
- Location: {person.Location}
- Affiliation: {person.Affiliation}
- Favorite Weapon: {person.FavoriteWeapon}
- Contact Number: {person.ContactNumber}
- Personality: {personalityDescription}
- Previous interrogations: {person.InterrogationCount}
- Has lied before: {person.HasLied}

INTERROGATION CONTEXT:
- Turn number: {turnNumber}
- Question being asked: ""{question}""
- Previous responses: {string.Join("; ", person.PreviousResponses.TakeLast(3))}

INSTRUCTIONS:
1. Respond as this character would naturally respond to the specific question
2. Consider the character's personality, background, and interrogation history
3. Your response should be realistic and in-character
4. Keep responses concise (1-2 sentences maximum)
5. If the character would lie, make it subtle and believable
6. If the character would tell the truth, make it reluctant or partial

RESPONSE FORMAT:
Respond only with the character's dialogue, nothing else. Do not include any meta-commentary or explanations.";

    }

    private async Task<string> GetAIResponse(string prompt)
    {
        var completionResult = await _openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            {
                ChatMessage.FromSystem("You are an expert role-player for an interrogation simulation game. Stay in character at all times."),
                ChatMessage.FromUser(prompt)
            },
            Model = "gpt-3.5-turbo",
            MaxTokens = 150,
            Temperature = 0.8f
        });

        if (completionResult.Successful)
        {
            return completionResult.Choices.First().Message.Content.Trim();
        }
        else
        {
            throw new Exception($"AI request failed: {completionResult.Error?.Message}");
        }
    }

    private async Task<InterrogationResult> ParseAIResponse(string aiResponse, Person person, int turnNumber)
    {
        // Use AI to analyze the response for lying detection
        var analysisPrompt = $@"Analyze this interrogation response for deception:

RESPONSE: ""{aiResponse}""
CHARACTER: {person.FirstName} {person.LastName} ({person.Personality} personality)
QUESTION CONTEXT: Turn {turnNumber}, {person.InterrogationCount} previous interrogations

Analyze and respond in this exact JSON format:
{{
    ""isLying"": true/false,
    ""confidence"": 0.0-1.0,
    ""reasoning"": ""brief explanation of why you think they are/aren't lying"",
    ""unlocksHint"": true/false,
    ""hint"": ""relevant hint if unlocksHint is true, otherwise null""
}}

Consider:
- Personality type and typical behavior
- Response consistency with background
- Linguistic markers of deception
- Interrogation pressure level
- Whether they would break and give a hint

Respond only with the JSON, no other text.";

        try
        {
            var analysisResult = await _openAIService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            {
                Messages = new List<ChatMessage>
                {
                    ChatMessage.FromSystem("You are a deception detection expert. Analyze responses and return only valid JSON."),
                    ChatMessage.FromUser(analysisPrompt)
                },
                Model = "gpt-3.5-turbo",
                MaxTokens = 200,
                Temperature = 0.3f
            });

            if (analysisResult.Successful)
            {
                var analysisText = analysisResult.Choices.First().Message.Content.Trim();
                return ParseAnalysisJSON(analysisText, aiResponse, person);
            }
        }
        catch
        {
            // Fallback analysis
        }

        // Fallback analysis if AI parsing fails
        return new InterrogationResult
        {
            Response = aiResponse,
            IsLying = DetermineIfLying(person, turnNumber),
            UnlocksHint = DetermineHintUnlock(person, turnNumber),
            Hint = DetermineHintUnlock(person, turnNumber) ? GenerateHint(person, turnNumber) : null,
            Confidence = 0.6,
            Reasoning = "AI analysis failed, using fallback detection"
        };
    }

    private InterrogationResult ParseAnalysisJSON(string jsonText, string aiResponse, Person person)
    {
        try
        {
            // Simple JSON parsing (you might want to use System.Text.Json for production)
            var isLying = jsonText.Contains("\"isLying\": true");
            var confidence = ExtractConfidence(jsonText);
            var reasoning = ExtractReasoning(jsonText);
            var unlocksHint = jsonText.Contains("\"unlocksHint\": true");
            var hint = unlocksHint ? ExtractHint(jsonText) : null;

            return new InterrogationResult
            {
                Response = aiResponse,
                IsLying = isLying,
                UnlocksHint = unlocksHint,
                Hint = hint,
                Confidence = confidence,
                Reasoning = reasoning,
                Personality = person.Personality
            };
        }
        catch
        {
            // Return fallback if JSON parsing fails
            return new InterrogationResult
            {
                Response = aiResponse,
                IsLying = false,
                UnlocksHint = false,
                Confidence = 0.5,
                Reasoning = "JSON parsing failed",
                Personality = person.Personality
            };
        }
    }

    private double ExtractConfidence(string jsonText)
    {
        try
        {
            var confidenceMatch = System.Text.RegularExpressions.Regex.Match(jsonText, @"""confidence"":\s*([0-9.]+)");
            if (confidenceMatch.Success && double.TryParse(confidenceMatch.Groups[1].Value, out var confidence))
            {
                return Math.Max(0.0, Math.Min(1.0, confidence));
            }
        }
        catch { }
        return 0.5;
    }

    private string ExtractReasoning(string jsonText)
    {
        try
        {
            var reasoningMatch = System.Text.RegularExpressions.Regex.Match(jsonText, @"""reasoning"":\s*""([^""]+)""");
            return reasoningMatch.Success ? reasoningMatch.Groups[1].Value : "Analysis unavailable";
        }
        catch
        {
            return "Analysis unavailable";
        }
    }

    private string? ExtractHint(string jsonText)
    {
        try
        {
            var hintMatch = System.Text.RegularExpressions.Regex.Match(jsonText, @"""hint"":\s*""([^""]+)""");
            return hintMatch.Success ? hintMatch.Groups[1].Value : null;
        }
        catch
        {
            return null;
        }
    }

    // Fallback methods (from original AIBehaviorService)
    private string GetFallbackResponse(Person person, string question)
    {
        var responses = person.Personality switch
        {
            PersonalityType.Aggressive => new[] { "I won't tell you anything!", "You'll get nothing from me!" },
            PersonalityType.Deceptive => new[] { "I don't know what you're talking about...", "Maybe I heard something..." },
            PersonalityType.Fearful => new[] { "Please don't hurt me...", "I... I don't want any trouble..." },
            _ => new[] { "I don't have much to say.", "I'm not sure what you want to know." }
        };
        return responses[new Random().Next(responses.Length)];
    }

    private bool DetermineIfLying(Person person, int turnNumber)
    {
        return person.Personality switch
        {
            PersonalityType.Deceptive => new Random().Next(100) < 70,
            PersonalityType.Aggressive => new Random().Next(100) < 40,
            PersonalityType.Fearful => new Random().Next(100) < 60,
            _ => new Random().Next(100) < 20
        };
    }

    private bool DetermineHintUnlock(Person person, int turnNumber)
    {
        return person.Personality switch
        {
            PersonalityType.Fearful => person.InterrogationCount >= 2,
            PersonalityType.Aggressive => person.InterrogationCount >= 4,
            PersonalityType.Deceptive => person.InterrogationCount >= 3,
            _ => person.InterrogationCount >= 3
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
        person.Personality = (PersonalityType)new Random().Next(4);
    }
}