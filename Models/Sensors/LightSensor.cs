using InterrogationGame.Models;

namespace InterrogationGame.Models.Sensors;

// Light sensor
public class LightSensor : Sensor, ISlotVerifiable
{
    public LightSensor()
    {
        Type = SensorType.Light;
        CooldownTurns = 1;
    }

    public override void Activate(int currentTurn)
    {
        base.Activate(currentTurn);
        // Light sensor specific activation logic
    }

    public string RevealCorrectSlots(ITerrorist terrorist)
    {
        var feedback = new List<string>();
        for (int i = 0; i < terrorist.SensorSlots.Count; i++)
        {
            var sensor = terrorist.SensorSlots[i];
            if (i < terrorist.RequiredSensorTypes.Count && sensor.Type == terrorist.RequiredSensorTypes[i])
                feedback.Add($"Slot {i}: {sensor.Type} is correct");
            else
                feedback.Add($"Slot {i}: {sensor.Type} is incorrect");
        }
        return string.Join("; ", feedback);
    }
}
