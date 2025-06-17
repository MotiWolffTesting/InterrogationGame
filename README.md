# InterrogationGame

A C# console-based interrogation simulation game where players investigate and interrogate different types of terrorists using various sensor technologies. This project implements a complete MVP (Phase 1) and extension (Phase 2) according to IDF 8200 training requirements.

## 🎯 Project Overview

This game simulates an interrogation scenario where players use different types of sensors to gather information about suspects and determine their terrorist classification and threat level. The implementation follows OOP and SOLID principles with a professional development workflow.

## 📋 Requirements Compliance

### ✅ Phase 1 - MVP Requirements (FULLY IMPLEMENTED)

#### Iranian Agent (Foot Soldier)
- **Requirement**: One Iranian agent type - junior level
- **Implementation**: `FootSoldier` class with 2 sensor slots, no counterattack behavior
- **Location**: `Models/Terrorists/FootSoldier.cs`

#### Sensor System (2 Types)
- **Requirement**: 2 sensor types, identical except for name, no special abilities
- **Implementation**: `AudioSensor` and `ChemicalSensor` as basic sensors
- **Location**: `Models/Sensors/AudioSensor.cs`, `Models/Sensors/ChemicalSensor.cs`

#### Core Game Mechanics
- **Requirement**: Secret weakness array of 2 sensors (can be same type)
- **Implementation**: `RequiredSensorTypes` list in `ITerrorist` interface
- **Requirement**: Empty attached sensor array
- **Implementation**: `SensorSlots` list in terrorist classes
- **Requirement**: Activate method that returns matching sensor count
- **Implementation**: `ActivateAllSensors()` and `GetCorrectSensorCount()` in `GameManager`
- **Requirement**: Menu asking player which sensor to add
- **Implementation**: `PromptForSensor()` method in `ConsoleGame`
- **Requirement**: Continue until X/X → agent exposed
- **Implementation**: `CheckForVictory()` method with proper victory condition

#### Required Classes
- **Requirement**: Sensor class
- **Implementation**: Abstract `Sensor` base class with all sensor types
- **Requirement**: IranianAgent class  
- **Implementation**: `ITerrorist` interface with `Terrorist` base class and concrete implementations
- **Requirement**: Game/InvestigationManager class
- **Implementation**: `GameManager` class handling core game logic

### ✅ Phase 2 - Extension Requirements (FULLY IMPLEMENTED)

#### New Sensor Type: Pulse Sensor
- **Requirement**: Breaks after 3 activations
- **Implementation**: `PulseSensor` with activation limit handled in base `Sensor` class
- **Location**: `Models/Sensors/PulseSensor.cs`

#### New Terrorist Type: Squad Leader
- **Requirement**: 4 sensors, counterattack every 3 turns removes 1 sensor
- **Implementation**: `SquadLeader` class with `PerformCounterattack()` method
- **Location**: `Models/Terrorists/SquadLeader.cs`

#### Advanced Terrorist Types
- **Senior Commander**: 6 slots, removes 2 sensors every 3 turns
- **Organization Leader**: 8 slots, removes 1 sensor every 3 turns, resets weakness list every 10 turns
- **Implementation**: Respective classes in `Models/Terrorists/`

#### OOP & SOLID Principles
- **Requirement**: Proper object-oriented design
- **Implementation**: Inheritance hierarchy, interface segregation, single responsibility
- **Interfaces**: `IRevealable`, `ICancellable`, `ISlotVerifiable`

#### Player Progress Tracking
- **Requirement**: Save player information including highest exposed agent
- **Implementation**: Database persistence with `GameDbContext` and `Person` entities
- **Location**: `Data/GameDbContext.cs`, `Models/Person.cs`

## 🏗️ Architecture & Design

### Project Structure
```
InterrogationGame/
├── Data/                    # Database context and initial data
│   ├── GameDbContext.cs     # Entity Framework context
│   ├── InitialData.cs       # Database seeding
│   └── DesignTimeDbContextFactory.cs
├── Models/                  # Core game models
│   ├── Sensors/            # Sensor implementations
│   │   ├── Interfaces/     # Sensor capability interfaces
│   │   ├── AudioSensor.cs  # Basic sensor
│   │   ├── ThermalSensor.cs # Reveals sensor types
│   │   ├── PulseSensor.cs  # Breaks after 3 uses
│   │   ├── MotionSensor.cs # 3 activation limit
│   │   ├── MagneticSensor.cs # Cancels counterattacks
│   │   ├── SignalSensor.cs # Reveals terrorist rank
│   │   ├── LightSensor.cs  # Reveals slot information
│   │   ├── ChemicalSensor.cs # Detects false positives
│   │   └── SensorFactory.cs # Factory pattern
│   ├── Terrorists/         # Terrorist type implementations
│   │   ├── FootSoldier.cs  # Basic terrorist (2 slots)
│   │   ├── SquadLeader.cs  # Mid-level (4 slots, counterattack)
│   │   ├── SeniorCommander.cs # High-level (6 slots, strong counterattack)
│   │   └── OrganizationLeader.cs # Top-level (8 slots, reset ability)
│   ├── ITerrorist.cs       # Terrorist interface
│   ├── Terrorist.cs        # Abstract base class
│   ├── Sensor.cs           # Abstract sensor base
│   ├── SensorType.cs       # Sensor type enumeration
│   ├── Person.cs           # Database entity
│   └── GameLog.cs          # Game action logging
├── Services/               # Game services
│   └── DatabaseService.cs  # Database operations
├── ConsoleGame.cs          # Console interface and game flow
├── GameManager.cs          # Core game logic and state management
└── Program.cs             # Application entry point with DI
```

### Design Patterns Used

#### Factory Pattern
- **Purpose**: Create sensor instances based on type
- **Implementation**: `SensorFactory` class
- **Usage**: `CreateSensor(SensorType type)`

#### Strategy Pattern
- **Purpose**: Different terrorist behaviors
- **Implementation**: `PerformCounterattack()` method in each terrorist class
- **Usage**: Polymorphic counterattack behavior

#### Observer Pattern
- **Purpose**: Sensor activation and feedback
- **Implementation**: `ActivateAllSensors()` method
- **Usage**: Automatic sensor processing

#### Repository Pattern
- **Purpose**: Database access abstraction
- **Implementation**: `DatabaseService` class
- **Usage**: CRUD operations for game data

## 🎮 Game Mechanics

### Core Gameplay Loop
1. **Terrorist Selection**: Random unexposed person assigned terrorist rank
2. **Sensor Assignment**: Player assigns sensors to available slots
3. **Sensor Activation**: All sensors activate and provide feedback
4. **Victory Check**: Game checks if required sensors match
5. **Counterattack**: Terrorist performs counterattack if applicable
6. **Turn End**: Game progresses to next turn or ends

### Victory Conditions
- **Exact Match**: All required sensor types must be attached
- **Count Matching**: Correct number of each sensor type
- **Order Independent**: Sensor order doesn't matter

### Sensor Special Abilities

| Sensor | Special Ability | Implementation |
|--------|----------------|----------------|
| **Audio** | Basic sensor, no special ability | `AudioSensor` - simple activation |
| **Thermal** | Reveals one correct sensor type | `ThermalSensor` with `IRevealable` interface |
| **Pulse** | Breaks after 3 activations | `PulseSensor` with activation limit |
| **Motion** | Can activate 3 times total | `MotionSensor` with usage tracking |
| **Magnetic** | Cancels terrorist counterattack twice | `MagneticSensor` with `ICancellable` interface |
| **Signal** | Reveals terrorist rank | `SignalSensor` with `IRevealable` interface |
| **Light** | Reveals 2 fields of information | `LightSensor` with `ISlotVerifiable` interface |
| **Chemical** | Detects false positives | `ChemicalSensor` with false positive detection |

### Terrorist Types & Behavior

| Rank | Sensor Slots | Counterattack Behavior | Implementation |
|------|--------------|----------------------|----------------|
| **Foot Soldier** | 2 | No counterattack | `FootSoldier` class |
| **Squad Leader** | 4 | Every 3 turns: removes 1 random sensor | `SquadLeader` class |
| **Senior Commander** | 6 | Every 3 turns: removes 2 random sensors | `SeniorCommander` class |
| **Organization Leader** | 8 | Every 3 turns: removes 1 sensor<br>Every 10 turns: resets weakness list | `OrganizationLeader` class |

## 🛠️ Technical Implementation

### Dependency Injection
- **Framework**: Microsoft.Extensions.DependencyInjection
- **Services**: `GameManager`, `DatabaseService`, `GameDbContext`
- **Configuration**: `appsettings.json` for database connection

### Database Integration
- **ORM**: Entity Framework Core
- **Database**: PostgreSQL
- **Entities**: `Person`, `GameLog`
- **Features**: Automatic migrations, data seeding, logging

### Error Handling
- **Validation**: Input validation in `ConsoleGame`
- **Exception Handling**: Try-catch blocks for database operations
- **Graceful Degradation**: Fallback behaviors for edge cases

### Async/Await Patterns
- **Database Operations**: All database calls are async
- **Game Flow**: Non-blocking user interactions
- **Performance**: Efficient resource utilization

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- PostgreSQL database
- Visual Studio 2022 or VS Code

### Installation & Setup
1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd InterrogationGame
   ```

2. **Configure database**
   - Update `appsettings.json` with your PostgreSQL connection string
   - Ensure PostgreSQL is running

3. **Restore dependencies**
   ```bash
   dotnet restore
   ```

4. **Build the project**
   ```bash
   dotnet build
   ```

5. **Run the game**
   ```bash
   dotnet run
   ```

### Database Setup
The application will automatically:
- Create the database if it doesn't exist
- Run Entity Framework migrations
- Seed initial data with sample terrorists

## 🎯 Development Workflow

### Git Branching Strategy
- **Main Branch**: Production-ready code
- **Feature Branches**: Individual feature development
- **Pull Requests**: Code review and integration

### Feature Branches Created
- `feature/sensor-implementation` - Sensor system
- `feature/terrorist-implementation` - Terrorist types
- `feature/game-mechanic` - Core game logic
- `feature/data-configuration` - Database setup

### Code Quality
- **SOLID Principles**: All classes follow single responsibility, open/closed, etc.
- **Interface Segregation**: Specific interfaces for sensor capabilities
- **Dependency Inversion**: High-level modules don't depend on low-level modules
- **Clean Architecture**: Separation of concerns between layers

## 📊 Compliance Summary

### ✅ Phase 1 MVP Requirements
- [x] Iranian agent (Foot Soldier) with 2 sensor slots
- [x] 2 basic sensor types (Audio, Chemical)
- [x] Secret weakness array system
- [x] Sensor activation and matching logic
- [x] Menu-driven sensor selection
- [x] Victory condition implementation
- [x] Required class structure (Sensor, IranianAgent, GameManager)

### ✅ Phase 2 Extension Requirements
- [x] Pulse sensor with 3-activation limit
- [x] Squad Leader terrorist type with counterattack
- [x] Advanced terrorist types (Senior Commander, Organization Leader)
- [x] OOP and SOLID principles implementation
- [x] Player progress tracking and persistence
- [x] All sensor special abilities implemented
- [x] Complete terrorist behavior system

### ✅ Development Standards
- [x] Git repository with proper branching
- [x] Feature branch development workflow
- [x] Pull request and merge process
- [x] Professional code organization
- [x] Comprehensive documentation

## 🎉 Conclusion

This project successfully implements all requirements from both Phase 1 (MVP) and Phase 2 (Extension) of the IDF 8200 training specification. The codebase demonstrates:

- **Complete feature implementation** with all required sensors, terrorists, and game mechanics
- **Professional software engineering practices** following OOP and SOLID principles
- **Production-ready architecture** with database integration and dependency injection
- **Proper development workflow** with Git branching, commits, and pull requests

The game is fully functional and ready for use, providing an engaging interrogation simulation experience with progressive difficulty levels and strategic gameplay elements.

## 📝 License

This project is for educational purposes only as part of IDF 8200 training exercises.
