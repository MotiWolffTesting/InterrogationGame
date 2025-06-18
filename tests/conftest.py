import pytest
import tempfile
import os
import sqlite3
import subprocess
from pathlib import Path

@pytest.fixture(scope="session")
def temp_db():
    """Create a temporary database for the entire test session"""
    with tempfile.NamedTemporaryFile(suffix='.db', delete=False) as f:
        db_path = f.name
    
    # Initialize the database with test data
    conn = sqlite3.connect(db_path)
    cursor = conn.cursor()
    
    # Create tables (simplified version based on your C# models)
    cursor.execute('''
        CREATE TABLE IF NOT EXISTS People (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            IsExposed BOOLEAN NOT NULL DEFAULT 0
        )
    ''')
    
    cursor.execute('''
        CREATE TABLE IF NOT EXISTS GameLogs (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            PersonId INTEGER NOT NULL,
            Action TEXT NOT NULL,
            Details TEXT,
            Timestamp DATETIME NOT NULL,
            FOREIGN KEY (PersonId) REFERENCES People(Id)
        )
    ''')
    
    # Insert test data
    test_people = [
        ("John Doe", False),
        ("Jane Smith", False),
        ("Bob Johnson", False),
        ("Alice Brown", True),  # Already exposed
        ("Charlie Wilson", False),
        ("Diana Prince", False),
    ]
    
    cursor.executemany(
        "INSERT INTO People (Name, IsExposed) VALUES (?, ?)",
        test_people
    )
    
    conn.commit()
    conn.close()
    
    yield db_path
    
    # Cleanup
    try:
        os.unlink(db_path)
    except FileNotFoundError:
        pass

@pytest.fixture
def game_executable():
    """Get the path to the compiled game executable"""
    # Get the project root directory (parent of tests directory)
    project_root = Path(__file__).parent.parent
    print(f"Found executable at: {project_root}/bin/Debug/net8.0/InterrogationGame")
    
    # Look for the executable in common build directories
    possible_paths = [
        project_root / "bin/Debug/net8.0/InterrogationGame",
        project_root / "bin/Release/net8.0/InterrogationGame"
    ]
    
    for path in possible_paths:
        if path.exists():
            return str(path), str(project_root)
    
    # If not found, try to build it
    try:
        subprocess.run(["dotnet", "build"], cwd=project_root, check=True, capture_output=True)
        for path in possible_paths:
            if path.exists():
                return str(path), str(project_root)
    except subprocess.CalledProcessError:
        pass
    
    pytest.skip("Game executable not found. Please build the project first.")

@pytest.fixture
def mock_game_output():
    """Mock game output for testing without running the actual game"""
    return {
        "welcome_message": "Welcome to InterrogationGame!",
        "menu_options": ["start", "help", "exit"],
        "sensor_types": ["audio", "chemical", "thermal", "pulse", "motion", "magnetic", "signal", "light"],
        "terrorist_ranks": ["foot_soldier", "squad_leader", "senior_commander", "organization_leader"]
    } 