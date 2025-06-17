# Real AI Integration Setup Guide

## Overview
This guide explains how to set up real AI integration for the Interrogation Game using OpenAI's GPT-3.5 Turbo model.

## Features Added
- **Real AI Responses**: Characters now respond intelligently to specific questions
- **Deception Detection**: AI analyzes responses for lying behavior
- **Confidence Scoring**: AI provides confidence levels for its analysis
- **Contextual Reasoning**: AI explains why it thinks someone is lying or telling the truth
- **Fallback System**: If AI fails, the system falls back to basic responses

## Setup Instructions

### 1. Get OpenAI API Key
1. Go to [OpenAI Platform](https://platform.openai.com/)
2. Sign up or log in to your account
3. Navigate to "API Keys" in your dashboard
4. Create a new API key
5. Copy the key (it starts with `sk-`)

### 2. Configure the Application
1. Open `appsettings.json`
2. Replace `"your-openai-api-key-here"` with your actual API key:
   ```json
   {
     "OpenAI": {
       "ApiKey": "sk-your-actual-api-key-here"
     }
   }
   ```

### 3. Install Dependencies
The project now includes the OpenAI SDK. Run:
```bash
dotnet restore
```

### 4. Run the Application
```bash
# For web version
dotnet run --web

# For console version
dotnet run
```

## How It Works

### AI Response Generation
1. **Character Context**: The AI receives detailed information about the character:
   - Name, location, affiliation
   - Personality type (Aggressive, Deceptive, Fearful, Neutral)
   - Previous interrogation history
   - Background details

2. **Question Processing**: The AI analyzes the specific question being asked and generates a contextual response

3. **Personality Simulation**: Responses are tailored to the character's personality:
   - **Aggressive**: Defiant, resistant responses
   - **Deceptive**: Evasive, manipulative responses
   - **Fearful**: Anxious, potentially cooperative responses
   - **Neutral**: Cautious, balanced responses

### Deception Detection
The AI performs a second analysis to detect lying:

1. **Linguistic Analysis**: Examines response patterns and language use
2. **Consistency Check**: Compares with character background and previous responses
3. **Personality Consideration**: Factors in typical behavior for the personality type
4. **Confidence Scoring**: Provides a confidence level (0-100%) for the analysis

### Example AI Interactions

**Question**: "Who is your superior?"
**Character**: Aggressive personality, previous interrogations: 2

**AI Response**: "I don't answer to anyone! You think I'm some kind of informant?"

**AI Analysis**: 
- Is Lying: true
- Confidence: 85%
- Reasoning: "Aggressive personality showing defensive behavior, avoiding direct answer about hierarchy"

## Cost Considerations
- Each interrogation uses approximately 2 API calls
- GPT-3.5 Turbo costs ~$0.002 per 1K tokens
- Typical interrogation costs: $0.01-0.05 per session
- Monitor usage in your OpenAI dashboard

## Fallback System
If the AI service is unavailable:
- Basic rule-based responses are used
- Simple probability-based lying detection
- No confidence scoring or detailed reasoning
- Game continues to function normally

## Advanced Configuration

### Model Selection
You can change the AI model in `RealAIBehaviorService.cs`:
```csharp
Model = Models.Gpt_4, // More expensive but more capable
Model = Models.Gpt_3_5_Turbo, // Current setting - good balance
```

### Temperature Settings
Adjust creativity vs consistency:
```csharp
Temperature = 0.8f, // More creative responses
Temperature = 0.3f, // More consistent responses
```

### Token Limits
Control response length:
```csharp
MaxTokens = 150, // Current setting
MaxTokens = 300, // Longer responses
```

## Troubleshooting

### Common Issues

1. **"OpenAI API key not found"**
   - Check `appsettings.json` has the correct API key
   - Ensure the key starts with `sk-`

2. **"AI request failed"**
   - Check your OpenAI account has credits
   - Verify internet connection
   - Check API key permissions

3. **Slow responses**
   - Normal for AI calls (1-3 seconds)
   - Consider using fallback mode for testing

4. **High costs**
   - Monitor usage in OpenAI dashboard
   - Consider setting usage limits
   - Use fallback mode for development

### Development Mode
For testing without AI costs, you can temporarily disable AI:
```csharp
// In RealAIBehaviorService.cs, comment out the AI calls
// and use only fallback responses
```

## Security Notes
- Never commit your API key to version control
- Use environment variables for production
- Consider API key rotation for security
- Monitor usage for unexpected charges

## Next Steps
- Implement conversation memory for more realistic interactions
- Add emotion detection and response
- Create character-specific knowledge bases
- Implement multi-turn conversation analysis 