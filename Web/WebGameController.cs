using Microsoft.AspNetCore.Mvc;
using InterrogationGame.Game;
using InterrogationGame.Models;
using InterrogationGame.Models.Sensors;
using InterrogationGame.Services;

namespace InterrogationGame.Web;

[ApiController]
[Route("api/[controller]")]
public class WebGameController : ControllerBase
{
    private readonly GameManager _gameManager;
    private readonly DatabaseService _dbService;
    private readonly RealAIBehaviorService _aiService;

    public WebGameController(GameManager gameManager, DatabaseService dbService, RealAIBehaviorService aiService)
    {
        _gameManager = gameManager;
        _dbService = dbService;
        _aiService = aiService;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartNewGame()
    {
        try
        {
            await _gameManager.StartNewGame();
            var terrorist = _gameManager.GetCurrentTerrorist();

            return Ok(new
            {
                success = true,
                terrorist = new
                {
                    id = terrorist?.Person.Id,
                    name = $"{terrorist?.Person.FirstName} {terrorist?.Person.LastName}",
                    rank = terrorist?.Rank.ToString(),
                    affiliation = terrorist?.Person.Affiliation,
                    maxSlots = terrorist?.MaxSensorSlots,
                    currentTurn = _gameManager.GetCurrentTurn()
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    [HttpGet("status")]
    public IActionResult GetGameStatus()
    {
        var terrorist = _gameManager.GetCurrentTerrorist();
        if (terrorist == null)
        {
            return NotFound(new { success = false, error = "No active game" });
        }

        var (correct, total) = _gameManager.GetCorrectSensorCount();
        var isVictory = _gameManager.CheckForVictory();

        return Ok(new
        {
            success = true,
            terrorist = new
            {
                id = terrorist.Person.Id,
                name = $"{terrorist.Person.FirstName} {terrorist.Person.LastName}",
                rank = terrorist.Rank.ToString(),
                affiliation = terrorist.Person.Affiliation,
                maxSlots = terrorist.MaxSensorSlots,
                currentSlots = terrorist.SensorSlots.Count,
                currentTurn = _gameManager.GetCurrentTurn()
            },
            sensors = terrorist.SensorSlots.Select((s, i) => new
            {
                slot = i,
                type = s.Type.ToString(),
                status = s.GetStatus(),
                batteryLevel = s.BatteryLevel,
                isActive = s.IsActive,
                activationCount = s.ActivationCount
            }).ToArray(),
            progress = new
            {
                correct = correct,
                total = total,
                isVictory = isVictory
            }
        });
    }

    [HttpPost("assign-sensors")]
    public IActionResult AssignSensors([FromBody] AssignSensorsRequest request)
    {
        try
        {
            var sensors = new List<Sensor>();
            foreach (var sensorType in request.SensorTypes)
            {
                Sensor sensor = sensorType switch
                {
                    "Audio" => new AudioSensor(),
                    "Thermal" => new ThermalSensor(),
                    "Pulse" => new PulseSensor(),
                    "Motion" => new MotionSensor(),
                    "Magnetic" => new MagneticSensor(),
                    "Signal" => new SignalSensor(),
                    "Light" => new LightSensor(),
                    "Chemical" => new ChemicalSensor(),
                    _ => throw new ArgumentException($"Unknown sensor type: {sensorType}")
                };
                sensors.Add(sensor);
            }

            _gameManager.AssignSensorsForTurn(sensors);
            var (correct, total) = _gameManager.GetCorrectSensorCount();
            var isVictory = _gameManager.CheckForVictory();

            return Ok(new
            {
                success = true,
                progress = new
                {
                    correct = correct,
                    total = total,
                    isVictory = isVictory
                },
                feedback = GetSensorFeedback()
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    [HttpPost("end-turn")]
    public async Task<IActionResult> EndTurn()
    {
        try
        {
            await _gameManager.EndTurn();
            var terrorist = _gameManager.GetCurrentTerrorist();

            return Ok(new
            {
                success = true,
                currentTurn = _gameManager.GetCurrentTurn(),
                terrorist = new
                {
                    id = terrorist?.Person.Id,
                    name = $"{terrorist?.Person.FirstName} {terrorist?.Person.LastName}",
                    rank = terrorist?.Rank.ToString(),
                    affiliation = terrorist?.Person.Affiliation,
                    maxSlots = terrorist?.MaxSensorSlots,
                    currentSlots = terrorist?.SensorSlots.Count ?? 0
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    [HttpPost("victory")]
    public async Task<IActionResult> MarkVictory()
    {
        try
        {
            await _gameManager.MarkCurrentTerroristExposed();
            var hasMoreTerrorists = await _gameManager.HasUnexposedTerroristsAsync();

            return Ok(new
            {
                success = true,
                hasMoreTerrorists = hasMoreTerrorists,
                message = hasMoreTerrorists
                    ? "Terrorist exposed! A new target has been identified."
                    : "All terrorists have been exposed! Mission complete!"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    [HttpGet("available-sensors")]
    public IActionResult GetAvailableSensors()
    {
        var sensors = Enum.GetValues<SensorType>().Select(st => new
        {
            type = st.ToString(),
            description = GetSensorDescription(st)
        }).ToArray();

        return Ok(new { success = true, sensors });
    }

    [HttpGet("clue/{personId}")]
    public async Task<IActionResult> GetSuperiorClue(int personId)
    {
        var clue = await _dbService.GetSuperiorClue(personId);
        if (clue == null)
            return NotFound(new { success = false, error = "No clue available for this person." });
        return Ok(new { success = true, clue });
    }

    private string GetSensorDescription(SensorType sensorType)
    {
        return sensorType switch
        {
            SensorType.Audio => "Basic sensor with no special abilities",
            SensorType.Thermal => "Reveals one correct sensor type from the secret list",
            SensorType.Pulse => "Breaks after 3 activations",
            SensorType.Motion => "Can activate 3 times total, then breaks",
            SensorType.Magnetic => "Cancels terrorist counterattack twice if matched correctly",
            SensorType.Signal => "Reveals one field of information about the terrorist",
            SensorType.Light => "Reveals 2 fields of information about the terrorist",
            SensorType.Chemical => "Detects false positives in sensor readings",
            _ => "Unknown sensor type"
        };
    }

    private object GetSensorFeedback()
    {
        var terrorist = _gameManager.GetCurrentTerrorist();
        if (terrorist == null) return new { };

        var feedback = new List<object>();

        for (int i = 0; i < terrorist.SensorSlots.Count; i++)
        {
            var sensor = terrorist.SensorSlots[i];
            var sensorFeedback = new
            {
                slot = i,
                type = sensor.Type.ToString(),
                status = sensor.GetStatus()
            };

            // Add special ability feedback
            object specialFeedback = sensor switch
            {
                ThermalSensor thermal => new { thermalAnalysis = "One required sensor type revealed" },
                SignalSensor signal => new { signalAnalysis = $"Terrorist Rank: {terrorist.Rank}" },
                LightSensor light => new { lightAnalysis = "Slot verification active" },
                MagneticSensor magnetic => new { magneticField = "Counterattack cancellation active" },
                _ => new { }
            };

            feedback.Add(new { sensorFeedback, specialFeedback });
        }

        return feedback;
    }

    public class HierarchyRequest
    {
        public List<int> PersonIds { get; set; } = new();
    }

    [HttpPost("check-hierarchy")]
    public async Task<IActionResult> CheckHierarchy([FromBody] HierarchyRequest request)
    {
        var isValid = await _dbService.CheckHierarchy(request.PersonIds);
        return Ok(new { success = true, isValid });
    }

    public class ScoreRequest
    {
        public string PlayerName { get; set; } = string.Empty;
        public int Points { get; set; }
    }

    [HttpPost("submit-score")]
    public async Task<IActionResult> SubmitScore([FromBody] ScoreRequest request)
    {
        await _dbService.SaveScore(request.PlayerName, request.Points);
        return Ok(new { success = true });
    }

    [HttpGet("top-scores")]
    public async Task<IActionResult> GetTopScores([FromQuery] int count = 10)
    {
        var scores = await _dbService.GetTopScores(count);
        return Ok(new { success = true, scores });
    }

    [HttpGet("all-people")]
    public async Task<IActionResult> GetAllPeople()
    {
        var people = await _dbService.GetAllPeople();
        return Ok(new { success = true, people });
    }

    // New AI & Behavior endpoints
    public class InterrogationRequest
    {
        public string Question { get; set; } = string.Empty;
    }

    [HttpPost("interrogate")]
    public async Task<IActionResult> Interrogate([FromBody] InterrogationRequest request)
    {
        var terrorist = _gameManager.GetCurrentTerrorist();
        if (terrorist == null)
        {
            return NotFound(new { success = false, error = "No active terrorist to interrogate" });
        }

        var result = await _aiService.ProcessInterrogation(terrorist.Person, request.Question, _gameManager.GetCurrentTurn());

        return Ok(new
        {
            success = true,
            response = result.Response,
            isLying = result.IsLying,
            personality = result.Personality.ToString(),
            unlocksHint = result.UnlocksHint,
            hint = result.Hint,
            interrogationCount = terrorist.Person.InterrogationCount,
            hasLied = terrorist.Person.HasLied,
            confidence = result.Confidence,
            reasoning = result.Reasoning
        });
    }

    [HttpGet("personality/{personId}")]
    public async Task<IActionResult> GetPersonality(int personId)
    {
        var person = await _dbService.GetPersonById(personId);
        if (person == null)
        {
            return NotFound(new { success = false, error = "Person not found" });
        }

        return Ok(new
        {
            success = true,
            personality = person.Personality.ToString(),
            interrogationCount = person.InterrogationCount,
            hasLied = person.HasLied,
            lastInterrogation = person.LastInterrogation
        });
    }

    // New Sensor Extension endpoints
    public class SensorActionRequest
    {
        public int SlotIndex { get; set; }
        public string Action { get; set; } = string.Empty; // "repair", "upgrade", "charge"
        public int? Amount { get; set; } // For repair amount
    }

    [HttpPost("sensor-action")]
    public IActionResult PerformSensorAction([FromBody] SensorActionRequest request)
    {
        var terrorist = _gameManager.GetCurrentTerrorist();
        if (terrorist == null)
        {
            return NotFound(new { success = false, error = "No active terrorist" });
        }

        if (request.SlotIndex < 0 || request.SlotIndex >= terrorist.SensorSlots.Count)
        {
            return BadRequest(new { success = false, error = "Invalid slot index" });
        }

        var sensor = terrorist.SensorSlots[request.SlotIndex];
        string message = "";

        switch (request.Action.ToLower())
        {
            case "repair":
                var repairAmount = request.Amount ?? 25;
                sensor.Repair(repairAmount);
                message = $"Sensor repaired by {repairAmount} points";
                break;

            case "upgrade":
                sensor.Upgrade();
                message = "Sensor upgraded!";
                break;

            case "charge":
                sensor.StartCharging();
                message = "Sensor charging started";
                break;

            default:
                return BadRequest(new { success = false, error = "Invalid action" });
        }

        return Ok(new
        {
            success = true,
            message = message,
            sensor = GetSensorInfo(sensor)
        });
    }

    [HttpPost("update-sensors")]
    public IActionResult UpdateSensors()
    {
        var terrorist = _gameManager.GetCurrentTerrorist();
        if (terrorist == null)
        {
            return NotFound(new { success = false, error = "No active terrorist" });
        }

        foreach (var sensor in terrorist.SensorSlots)
        {
            sensor.UpdateCooldown();
        }

        var sensors = terrorist.SensorSlots.Select((s, i) => new
        {
            slot = i,
            sensor = GetSensorInfo(s)
        }).ToArray();

        return Ok(new { success = true, sensors });
    }

    private object GetSensorInfo(ISensor sensor)
    {
        return new
        {
            type = sensor.Type.ToString(),
            batteryLevel = sensor.BatteryLevel,
            isActive = sensor.IsActive,
            activationCount = sensor.ActivationCount,
            durability = sensor.Durability,
            maxDurability = sensor.MaxDurability,
            cooldownTurns = sensor.CooldownTurns,
            isCharging = sensor.IsCharging,
            chargeTime = sensor.ChargeTime,
            maxChargeTime = sensor.MaxChargeTime,
            isUpgraded = sensor.IsUpgraded,
            upgradeLevel = sensor.UpgradeLevel,
            isBroken = sensor.IsBroken(),
            isOnCooldown = sensor.IsOnCooldown(),
            status = sensor.GetStatus()
        };
    }
}

public class AssignSensorsRequest
{
    public List<string> SensorTypes { get; set; } = new();
}