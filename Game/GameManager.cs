using InterrogationGame.Models;
using InterrogationGame.Models.Sensors;
using InterrogationGame.Models.Terrorists;
using InterrogationGame.Services;

namespace InterrogationGame.Game;

public class GameManager
{
    private readonly DatabaseService _dbService;
    private ITerrorist? _currentTerrorist;
    private int _currentTurn;
    private readonly Random _random = new();

    public GameManager(DatabaseService dbService)
    {
        _dbService = dbService;
    }

    public async Task StartNewGame()
    {
        var person = await _dbService.GetRandomUnexposedPerson();
        if (person == null)
        {
            throw new InvalidOperationException("No unexposed persons available.");
        }

        // Randomly select a terrorist rank
        var rank = (TerroristRank)_random.Next(4);
        _currentTerrorist = rank switch
        {
            TerroristRank.FootSoldier => new FootSoldier(),
            TerroristRank.SquadLeader => new SquadLeader(),
            TerroristRank.SeniorCommander => new SeniorCommander(),
            TerroristRank.OrganizationLeader => new OrganizationLeader(),
            _ => new FootSoldier()
        };

        _currentTerrorist.Person = person;
        _currentTerrorist.TurnCount = 0;

        // Assign required sensor types (allowing duplicates)
        var allTypes = Enum.GetValues<SensorType>();
        _currentTerrorist.RequiredSensorTypes = Enumerable.Range(0, _currentTerrorist.MaxSensorSlots)
            .Select(_ => allTypes[_random.Next(allTypes.Length)])
            .ToList();

        _currentTurn = 1;
    }

    public bool AttachSensor(Sensor sensor, int slotIndex)
    {
        if (_currentTerrorist == null) return false;
        if (slotIndex < 0 || slotIndex >= _currentTerrorist.MaxSensorSlots) return false;
        if (_currentTerrorist.SensorSlots.Count > slotIndex)
        {
            // Replace sensor
            _currentTerrorist.SensorSlots[slotIndex] = sensor;
        }
        else if (_currentTerrorist.SensorSlots.Count < _currentTerrorist.MaxSensorSlots)
        {
            // Add sensor
            _currentTerrorist.SensorSlots.Add(sensor);
        }
        else
        {
            return false;
        }
        ActivateAllSensors();
        return CheckForVictory();
    }

    public bool RemoveSensor(int slotIndex)
    {
        if (_currentTerrorist == null) return false;
        if (slotIndex < 0 || slotIndex >= _currentTerrorist.SensorSlots.Count) return false;
        _currentTerrorist.SensorSlots.RemoveAt(slotIndex);
        return true;
    }

    private void ActivateAllSensors()
    {
        if (_currentTerrorist == null) return;
        foreach (var sensor in _currentTerrorist.SensorSlots)
        {
            sensor.Activate(_currentTurn);
        }
    }

    public bool CheckForVictory()
    {
        if (_currentTerrorist == null) return false;
        var required = _currentTerrorist.RequiredSensorTypes.ToList();
        var attached = _currentTerrorist.SensorSlots.Select(s => s.Type).ToList();
        foreach (var type in required.Distinct())
        {
            int requiredCount = required.Count(t => t == type);
            int attachedCount = attached.Count(t => t == type);
            if (attachedCount < requiredCount)
                return false;
        }
        return true;
    }

    public (int correct, int total) GetCorrectSensorCount()
    {
        if (_currentTerrorist == null) return (0, 0);
        var required = _currentTerrorist.RequiredSensorTypes.ToList();
        var attached = _currentTerrorist.SensorSlots.Select(s => s.Type).ToList();
        int correct = 0;
        foreach (var type in required.Distinct())
        {
            int requiredCount = required.Count(t => t == type);
            int attachedCount = attached.Count(t => t == type);
            correct += Math.Min(requiredCount, attachedCount);
        }
        return (correct, required.Count);
    }

    public async Task EndTurn()
    {
        if (_currentTerrorist == null) return;
        _currentTerrorist.TurnCount++;
        _currentTerrorist.PerformCounterattack();
        _currentTurn++;
        await _dbService.LogGameAction(
            _currentTerrorist.Person.Id,
            "EndTurn",
            $"Turn {_currentTurn} completed. Active sensors: {_currentTerrorist.SensorSlots.Count}"
        );
    }

    public ITerrorist? GetCurrentTerrorist()
    {
        return _currentTerrorist;
    }

    public int GetCurrentTurn()
    {
        return _currentTurn;
    }

    public async Task MarkCurrentTerroristExposed()
    {
        if (_currentTerrorist != null)
        {
            await _dbService.UpdatePersonExposedStatus(_currentTerrorist.Person.Id, true);
        }
    }

    public async Task<bool> HasUnexposedTerroristsAsync()
    {
        var person = await _dbService.GetRandomUnexposedPerson();
        return person != null;
    }

    public void AssignSensorsForTurn(List<Sensor> sensors)
    {
        if (_currentTerrorist == null) return;
        _currentTerrorist.SensorSlots = sensors;
        ActivateAllSensors();
    }
}