using InterrogationGame.Models;

namespace InterrogationGame.Models.Sensors;

// Chemical sensor
public class ChemicalSensor : Sensor
{
    public ChemicalSensor()
    {
        Type = SensorType.Chemical;
        CooldownTurns = 2;
    }

    public override void Activate(int currentTurn)
    {
        base.Activate(currentTurn);
    }

    public override string GetStatus()
    {
        return $"{base.GetStatus()}, Chemical Analysis Ready";
    }

    public string DetectFalsePositives(List<SensorType> requiredTypes, List<SensorType> otherTypes)
    {
        var falsePositives = requiredTypes.Intersect(otherTypes).ToList();
        if (falsePositives.Count == 0)
            return "No false positives detected.";
        return $"False positives: {string.Join(", ", falsePositives)}";
    }
}