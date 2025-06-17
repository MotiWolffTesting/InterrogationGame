using InterrogationGame;
using InterrogationGame.Models;
using InterrogationGame.Services;
using InterrogationGame.Models.Sensors;

namespace InterrogationGame.Game;

public class ConsoleGame
{
    private readonly GameManager _gameManager;
    private readonly DatabaseService _dbService;

    public ConsoleGame(GameManager gameManager, DatabaseService dbService)
    {
        _gameManager = gameManager;
        _dbService = dbService;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("Welcome to the Interrogation Game!");
        await _gameManager.StartNewGame();
        PrintTerroristInfo();
        bool running = true;
        while (running)
        {
            var terrorist = _gameManager.GetCurrentTerrorist();
            if (terrorist == null)
            {
                Console.WriteLine("No terrorist loaded.");
                return;
            }
            Console.WriteLine($"\nAssign sensors for this turn (slots: {terrorist.MaxSensorSlots}):");
            var sensors = new List<Sensor>();
            for (int i = 0; i < terrorist.MaxSensorSlots; i++)
            {
                sensors.Add(PromptForSensor(i));
            }
            _gameManager.AssignSensorsForTurn(sensors);
            ShowSensorFeedback();
            if (_gameManager.CheckForVictory())
            {
                Console.WriteLine("Terrorist fully exposed! You win!");
                await _gameManager.MarkCurrentTerroristExposed();
                if (await _gameManager.HasUnexposedTerroristsAsync())
                {
                    Console.WriteLine("A new terrorist has been selected. Interrogation continues!");
                    await _gameManager.StartNewGame();
                    PrintTerroristInfo();
                }
                else
                {
                    Console.WriteLine("All terrorists have been exposed. Mission complete!");
                    return;
                }
            }
            else
            {
                await _gameManager.EndTurn();
                PrintTerroristInfo();
            }
        }
        Console.WriteLine("Thanks for playing!");
    }

    private Sensor PromptForSensor(int slotIndex)
    {
        while (true)
        {
            Console.Write($"Slot {slotIndex}: Enter sensor type (Audio, Thermal, Pulse, Motion, Magnetic, Signal, Light, Chemical): ");
            var input = Console.ReadLine();
            if (Enum.TryParse<SensorType>(input, true, out var sensorType))
            {
                return sensorType switch
                {
                    SensorType.Audio => new AudioSensor(),
                    SensorType.Thermal => new ThermalSensor(),
                    SensorType.Pulse => new PulseSensor(),
                    SensorType.Motion => new MotionSensor(),
                    SensorType.Magnetic => new MagneticSensor(),
                    SensorType.Signal => new SignalSensor(),
                    SensorType.Light => new LightSensor(),
                    SensorType.Chemical => new ChemicalSensor(),
                    _ => new AudioSensor()
                };
            }
            Console.WriteLine("Invalid sensor type. Try again.");
        }
    }

    private void ShowSensorFeedback()
    {
        var terrorist = _gameManager.GetCurrentTerrorist();
        if (terrorist == null)
        {
            Console.WriteLine("No terrorist loaded.");
            return;
        }

        Console.WriteLine("\n=== Sensor Feedback ===");

        // Basic sensor matching feedback
        var (correct, total) = _gameManager.GetCorrectSensorCount();
        Console.WriteLine($"\nBasic Feedback: {correct}/{total} sensors are correct (type and count, order doesn't matter).");

        // Process each sensor's special abilities
        for (int i = 0; i < terrorist.SensorSlots.Count; i++)
        {
            var sensor = terrorist.SensorSlots[i];
            Console.WriteLine($"\nSlot {i} - {sensor.Type} Sensor:");
            Console.WriteLine($"Status: {sensor.GetStatus()}");

            // Process special abilities based on sensor type
            switch (sensor)
            {
                case ThermalSensor thermal:
                    var revealedType = thermal.RevealCorrectType(terrorist.RequiredSensorTypes);
                    if (revealedType.HasValue)
                        Console.WriteLine($"Thermal Analysis: One required sensor type is {revealedType.Value}");
                    break;

                case SignalSensor signal:
                    Console.WriteLine($"Signal Analysis: {signal.RevealInfo(terrorist)}");
                    break;

                case LightSensor light:
                    Console.WriteLine($"Light Analysis: {light.RevealCorrectSlots(terrorist)}");
                    break;

                case ChemicalSensor chemical:
                    // Get all possible sensor types for comparison
                    var allTypes = Enum.GetValues<SensorType>().ToList();
                    Console.WriteLine($"Chemical Analysis: {chemical.DetectFalsePositives(terrorist.RequiredSensorTypes, allTypes)}");
                    break;

                case MagneticSensor magnetic:
                    if (magnetic.IsAbilityCancelled)
                        Console.WriteLine("Magnetic Field: Terrorist ability cancellation active");
                    break;

                case MotionSensor motion:
                    if (motion.ActivationCount >= 3)
                        Console.WriteLine("WARNING: Motion Sensor is broken (max activations reached)");
                    break;

                case PulseSensor pulse:
                    if (!pulse.CanActivate(_gameManager.GetCurrentTurn()))
                        Console.WriteLine("Pulse Sensor is recharging");
                    break;
            }
        }
        Console.WriteLine("\n===================");
    }

    private void PrintTerroristInfo()
    {
        var terrorist = _gameManager.GetCurrentTerrorist();
        if (terrorist == null) return;
        var person = terrorist.Person;
        Console.WriteLine($"\n--- Terrorist Info ---");
        Console.WriteLine($"Name: {person.FirstName} {person.LastName}");
        Console.WriteLine($"Rank: {terrorist.Rank}");
        Console.WriteLine($"Affiliation: {person.Affiliation}");
        Console.WriteLine($"---------------------\n");
    }
}
