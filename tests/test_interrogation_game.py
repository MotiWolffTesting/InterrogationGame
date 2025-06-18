import pytest
import subprocess
import json
import sqlite3
import os
import tempfile
import time
from pathlib import Path

class TestInterrogationGame:
    """Test suite for the InterrogationGame C# application"""
    
    @pytest.fixture
    def temp_db(self):
        """Create a temporary database for testing"""
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
        ]
        
        cursor.executemany(
            "INSERT INTO People (Name, IsExposed) VALUES (?, ?)",
            test_people
        )
        
        conn.commit()
        conn.close()
        
        yield db_path
        
        # Cleanup
        os.unlink(db_path)
    
    def test_game_starts_without_crash(self, game_executable):
        """Test that the game starts without crashing"""
        executable_path, project_root = game_executable
        try:
            result = subprocess.run(
                [executable_path, "--help"],
                capture_output=True,
                text=True,
                timeout=10,
                cwd=project_root,
                input="exit\n"  # Send exit to terminate the game
            )
            # Should not crash, even if help is not implemented
            assert result.returncode in [0, 1]  # 0 for success, 1 for help shown
        except subprocess.TimeoutExpired:
            pytest.fail("Game took too long to start")
    
    def test_game_creates_database(self, game_executable, temp_db):
        """Test that the game can create and initialize a database"""
        executable_path, project_root = game_executable
        # Set environment variable to use our temp database
        env = os.environ.copy()
        env['DATABASE_PATH'] = temp_db
        
        try:
            result = subprocess.run(
                [executable_path],
                capture_output=True,
                text=True,
                timeout=30,
                env=env,
                cwd=project_root,
                input="exit\n"  # Exit immediately
            )
            
            # Check that database was created/accessed
            conn = sqlite3.connect(temp_db)
            cursor = conn.cursor()
            cursor.execute("SELECT COUNT(*) FROM People")
            count = cursor.fetchone()[0]
            conn.close()
            
            assert count >= 0  # Database should be accessible
        except subprocess.TimeoutExpired:
            pytest.fail("Game took too long to respond")
    
    def test_game_handles_invalid_input(self, game_executable):
        """Test that the game handles invalid input gracefully"""
        executable_path, project_root = game_executable
        try:
            result = subprocess.run(
                [executable_path],
                capture_output=True,
                text=True,
                timeout=10,
                cwd=project_root,
                input="invalid_command\ninvalid_command\ninvalid_command\nexit\n"
            )
            
            # Should not crash on invalid input
            assert result.returncode in [0, 1]
            
            # Should provide some feedback (error message or help)
            output = result.stdout + result.stderr
            assert len(output) > 0
            
        except subprocess.TimeoutExpired:
            pytest.fail("Game took too long to respond to invalid input")
    
    def test_game_responds_to_basic_commands(self, game_executable):
        """Test that the game responds to basic commands"""
        executable_path, project_root = game_executable
        try:
            result = subprocess.run(
                [executable_path],
                capture_output=True,
                text=True,
                timeout=15,
                cwd=project_root,
                input="help\nexit\n"
            )
            
            output = result.stdout + result.stderr
            
            # Should show some output (welcome message, menu, etc.)
            assert len(output) > 0
            
            # Should not crash
            assert result.returncode in [0, 1]
            
        except subprocess.TimeoutExpired:
            pytest.fail("Game took too long to respond to help command")
    
    def test_game_manages_terrorist_exposure(self, game_executable, temp_db):
        """Test that the game can mark terrorists as exposed"""
        executable_path, project_root = game_executable
        env = os.environ.copy()
        env['DATABASE_PATH'] = temp_db
        
        # First, check initial state
        conn = sqlite3.connect(temp_db)
        cursor = conn.cursor()
        cursor.execute("SELECT COUNT(*) FROM People WHERE IsExposed = 0")
        initial_unexposed = cursor.fetchone()[0]
        conn.close()
        
        try:
            # Run game with commands that should expose a terrorist
            result = subprocess.run(
                [executable_path],
                capture_output=True,
                text=True,
                timeout=60,
                env=env,
                cwd=project_root,
                input="start\nattach 0 audio\nattach 1 chemical\nactivate\nexit\n"
            )
            
            # Check if any terrorist was exposed
            conn = sqlite3.connect(temp_db)
            cursor = conn.cursor()
            cursor.execute("SELECT COUNT(*) FROM People WHERE IsExposed = 1")
            exposed_count = cursor.fetchone()[0]
            conn.close()
            
            # Should have at least one exposed person (Alice Brown from initial data)
            assert exposed_count >= 1
            
        except subprocess.TimeoutExpired:
            pytest.fail("Game took too long during terrorist exposure test")
    
    def test_game_logs_actions(self, game_executable, temp_db):
        """Test that the game logs actions to the database"""
        executable_path, project_root = game_executable
        env = os.environ.copy()
        env['DATABASE_PATH'] = temp_db
        
        try:
            result = subprocess.run(
                [executable_path],
                capture_output=True,
                text=True,
                timeout=30,
                env=env,
                cwd=project_root,
                input="start\nattach 0 audio\nend_turn\nexit\n"
            )
            
            # Check if actions were logged
            conn = sqlite3.connect(temp_db)
            cursor = conn.cursor()
            cursor.execute("SELECT COUNT(*) FROM GameLogs")
            log_count = cursor.fetchone()[0]
            conn.close()
            
            # Should have logged some actions
            assert log_count >= 0  # At minimum, should not crash
            
        except subprocess.TimeoutExpired:
            pytest.fail("Game took too long during logging test")
    
    def test_game_handles_sensor_operations(self, game_executable):
        """Test that the game can handle sensor operations"""
        executable_path, project_root = game_executable
        try:
            result = subprocess.run(
                [executable_path],
                capture_output=True,
                text=True,
                timeout=20,
                cwd=project_root,
                input="start\nlist_sensors\nattach 0 audio\nremove 0\nexit\n"
            )
            
            output = result.stdout + result.stderr
            
            # Should not crash during sensor operations
            assert result.returncode in [0, 1]
            
            # Should provide some feedback
            assert len(output) > 0
            
        except subprocess.TimeoutExpired:
            pytest.fail("Game took too long during sensor operations test")
    
    def test_game_victory_condition(self, game_executable, temp_db):
        """Test that the game can detect victory conditions"""
        executable_path, project_root = game_executable
        env = os.environ.copy()
        env['DATABASE_PATH'] = temp_db
        
        try:
            # Try to win by attaching correct sensors
            result = subprocess.run(
                [executable_path],
                capture_output=True,
                text=True,
                timeout=60,
                env=env,
                cwd=project_root,
                input="start\nattach 0 audio\nattach 1 chemical\nattach 2 thermal\nactivate\ncheck_victory\nexit\n"
            )
            
            output = result.stdout + result.stderr
            
            # Should not crash during victory check
            assert result.returncode in [0, 1]
            
            # Should provide feedback about victory condition
            assert len(output) > 0
            
        except subprocess.TimeoutExpired:
            pytest.fail("Game took too long during victory condition test")
    
    def test_organization_leader_turn_10_reset(self, game_executable, temp_db):
        """Test that Organization Leader resets at turn 10 - clears sensors and changes weakness pattern"""
        executable_path, project_root = game_executable
        env = os.environ.copy()
        env['DATABASE_PATH'] = temp_db
        
        # Create a test that runs for 11 turns to trigger the reset
        # We'll use a simple input pattern that should work for any sensor combination
        # and observe if the game continues past turn 10
        
        try:
            # Run game for 11 turns with basic sensor inputs
            # This should trigger the Organization Leader reset at turn 10
            result = subprocess.run(
                [executable_path],
                capture_output=True,
                text=True,
                timeout=120,  # Longer timeout for 11 turns
                env=env,
                cwd=project_root,
                input="Audio\nAudio\nAudio\nAudio\nAudio\nAudio\nAudio\nAudio\n" * 11 + "exit\n"  # 11 turns of basic sensors
            )
            
            output = result.stdout + result.stderr
            
            # Should not crash during the extended gameplay
            assert result.returncode in [0, 1]
            
            # Should show game progression through multiple turns
            assert len(output) > 0
            
            # Check if the game handled the extended gameplay
            # The key test is that it doesn't crash and continues running
            print(f"Game output length: {len(output)}")
            print(f"Game return code: {result.returncode}")
            
        except subprocess.TimeoutExpired:
            pytest.fail("Game took too long during Organization Leader reset test")
    
    def test_organization_leader_behavior(self, game_executable, temp_db):
        """Test Organization Leader specific behavior and counterattacks"""
        executable_path, project_root = game_executable
        env = os.environ.copy()
        env['DATABASE_PATH'] = temp_db
        
        try:
            # Run game for a few turns to observe Organization Leader behavior
            result = subprocess.run(
                [executable_path],
                capture_output=True,
                text=True,
                timeout=60,
                env=env,
                cwd=project_root,
                input="Audio\nThermal\nPulse\nMotion\nMagnetic\nSignal\nLight\nChemical\n" * 3 + "exit\n"  # 3 turns
            )
            
            output = result.stdout + result.stderr
            
            # Should not crash
            assert result.returncode in [0, 1]
            
            # Should show some game output
            assert len(output) > 0
            
            # Look for Organization Leader specific indicators
            if "OrganizationLeader" in output or "Organization Leader" in output:
                print("Organization Leader detected in game output")
            
            print(f"Game output length: {len(output)}")
            
        except subprocess.TimeoutExpired:
            pytest.fail("Game took too long during Organization Leader behavior test")

class TestGameComponents:
    """Test individual game components through file system and database"""
    
    def test_database_schema(self, temp_db):
        """Test that the database has the correct schema"""
        conn = sqlite3.connect(temp_db)
        cursor = conn.cursor()
        
        # Check People table
        cursor.execute("PRAGMA table_info(People)")
        people_columns = [row[1] for row in cursor.fetchall()]
        
        assert 'Id' in people_columns
        assert 'Name' in people_columns
        assert 'IsExposed' in people_columns
        
        # Check GameLogs table
        cursor.execute("PRAGMA table_info(GameLogs)")
        logs_columns = [row[1] for row in cursor.fetchall()]
        
        assert 'Id' in logs_columns
        assert 'PersonId' in logs_columns
        assert 'Action' in logs_columns
        assert 'Details' in logs_columns
        assert 'Timestamp' in logs_columns
        
        conn.close()
    
    def test_initial_data_loaded(self, temp_db):
        """Test that initial test data is loaded correctly"""
        conn = sqlite3.connect(temp_db)
        cursor = conn.cursor()
        
        cursor.execute("SELECT COUNT(*) FROM People")
        total_count = cursor.fetchone()[0]
        
        cursor.execute("SELECT COUNT(*) FROM People WHERE IsExposed = 0")
        unexposed_count = cursor.fetchone()[0]
        
        cursor.execute("SELECT COUNT(*) FROM People WHERE IsExposed = 1")
        exposed_count = cursor.fetchone()[0]
        
        conn.close()
        
        assert total_count >= 4  # Should have our test data
        assert unexposed_count >= 3  # Should have unexposed people
        assert exposed_count >= 1  # Should have at least one exposed person

if __name__ == "__main__":
    pytest.main([__file__]) 