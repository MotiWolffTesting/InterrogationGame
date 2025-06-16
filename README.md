# InterrogationGame

A C# console-based interrogation simulation game where players investigate and interrogate different types of terrorists using various sensor technologies.

## Overview

This game simulates an interrogation scenario where players use different types of sensors to gather information about suspects and determine their terrorist classification and threat level.

## Features

### Terrorist Types
- **Foot Soldier**: Basic level operatives
- **Squad Leader**: Mid-level commanders
- **Senior Commander**: High-level tactical leaders
- **Organization Leader**: Top-tier strategic leaders

### Sensor Technologies
- **Audio Sensor**: Detects voice patterns and stress levels
- **Chemical Sensor**: Analyzes chemical traces and substances
- **Light Sensor**: Monitors visual cues and reactions
- **Magnetic Sensor**: Detects metallic objects and devices
- **Motion Sensor**: Tracks movement patterns and gestures
- **Pulse Sensor**: Monitors heart rate and physiological responses
- **Signal Sensor**: Intercepts and analyzes electronic communications
- **Thermal Sensor**: Reads body temperature and heat signatures

## Project Structure

```
InterrogationGame/
├── Data/                    # Database context and initial data
├── Models/                  # Core game models
│   ├── Sensors/            # Sensor implementations
│   └── Terrorists/         # Terrorist type implementations
├── Services/               # Game services and utilities
├── ConsoleGame.cs          # Console interface
├── GameManager.cs          # Core game logic
└── Program.cs             # Application entry point
```

## Technologies Used

- **C# .NET 9.0**: Core framework
- **Entity Framework Core**: Database management
- **Console Application**: User interface

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- Visual Studio 2022 or VS Code

### Running the Game
1. Clone the repository
2. Navigate to the project directory
3. Run the following commands:

```bash
dotnet restore
dotnet build
dotnet run
```

## Game Mechanics

Players will:
1. Select suspects for interrogation
2. Choose appropriate sensors for investigation
3. Analyze sensor data to determine terrorist classification
4. Make decisions based on gathered intelligence
5. Track interrogation results and effectiveness

## Development Status

🚧 **Currently in Development** 🚧

This project is actively being developed as part of IDF 8200 training exercises.

## License

This project is for educational purposes only.
