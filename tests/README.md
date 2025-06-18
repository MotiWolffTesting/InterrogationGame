# Python Test Suite for InterrogationGame

This directory contains a comprehensive Python test suite for testing the InterrogationGame C# application.

## Why Python Tests?

- **Better macOS compatibility**: Python and pytest work reliably on macOS
- **Easy setup**: No complex .NET test framework dependencies
- **Flexible testing**: Can test the application as a black box
- **Database testing**: Direct database access for verification
- **Process testing**: Tests the actual compiled application

## Setup

1. **Install Python dependencies**:
   ```bash
   pip install -r requirements.txt
   ```

2. **Build the C# application**:
   ```bash
   dotnet build
   ```

## Running Tests

### Run all tests:
```bash
pytest
```

### Run with verbose output:
```bash
pytest -v
```

### Run with coverage:
```bash
pytest --cov=.
```

### Run specific test:
```bash
pytest tests/test_interrogation_game.py::TestInterrogationGame::test_game_starts_without_crash
```

### Run tests with timeout:
```bash
pytest --timeout=30
```

## Test Categories

### 1. **Integration Tests** (`TestInterrogationGame`)
- Tests the actual compiled application
- Runs the game as a subprocess
- Tests user interactions and responses
- Verifies database state changes

### 2. **Component Tests** (`TestGameComponents`)
- Tests individual components
- Database schema validation
- Data integrity checks

## Test Features

- **Automatic executable detection**: Finds the compiled game automatically
- **Temporary database**: Uses isolated test databases
- **Timeout protection**: Prevents hanging tests
- **Comprehensive coverage**: Tests all major game features
- **Error handling**: Tests graceful failure scenarios

## What Gets Tested

1. **Application startup** - No crashes on launch
2. **Database operations** - Schema creation and data persistence
3. **User input handling** - Invalid input doesn't crash the game
4. **Game mechanics** - Sensor attachment, terrorist exposure
5. **Victory conditions** - Game can detect wins
6. **Logging** - Actions are properly logged
7. **Error scenarios** - Graceful handling of edge cases

## Advantages Over C# Tests

- **No framework conflicts**: No MSTest/xUnit compatibility issues
- **Cross-platform**: Works on Windows, macOS, and Linux
- **Easy debugging**: Python's rich ecosystem for debugging
- **Flexible assertions**: pytest's powerful assertion library
- **Better reporting**: Detailed test reports and coverage

## Customization

You can easily extend the tests by:

1. **Adding new test methods** to the existing classes
2. **Creating new test classes** for specific features
3. **Modifying the database schema** in `conftest.py`
4. **Adding new fixtures** for reusable test data

## Troubleshooting

### Game executable not found
- Make sure you've built the project: `dotnet build`
- Check that the executable exists in `bin/Debug/net8.0/`

### Database errors
- Tests use temporary databases that are automatically cleaned up
- If you see permission errors, check your temp directory permissions

### Timeout errors
- Increase timeout values in the test methods
- Some operations (like database initialization) may take longer on slower machines 