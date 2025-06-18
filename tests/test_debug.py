import pytest
import os

def test_debug_executable_path(game_executable):
    """Debug test to see if the executable path is found"""
    print(f"Current working directory: {os.getcwd()}")
    print(f"Executable path: {game_executable}")
    assert game_executable is not None
    assert os.path.exists(game_executable)
    print(f"Executable exists: {os.path.exists(game_executable)}")

def test_debug_paths():
    """Debug test to check what paths exist"""
    current_dir = os.getcwd()
    print(f"Current directory: {current_dir}")
    
    if current_dir.endswith('tests'):
        parent_dir = os.path.dirname(current_dir)
    else:
        parent_dir = current_dir
    
    print(f"Parent directory: {parent_dir}")
    
    possible_paths = [
        os.path.join(parent_dir, "bin/Debug/net8.0/InterrogationGame"),
        os.path.join(parent_dir, "bin/Release/net8.0/InterrogationGame"),
    ]
    
    for path in possible_paths:
        exists = os.path.exists(path)
        print(f"Path: {path} - Exists: {exists}")
        if exists:
            print(f"  File size: {os.path.getsize(path)}")
            print(f"  Executable: {os.access(path, os.X_OK)}") 