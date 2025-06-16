using InterrogationGame.Models;

namespace InterrogationGame.Models.Sensors;

public class ThermalSensor : Sensor, IRevealable
{
    private SensorType? revealedType;

    public ThermalSensor()
    {
        Type = SensorType.Thermal;
        CooldownTurns = 1;
    }

    public override void Activate(int currentTurn)
    {
        base.Activate(currentTurn);
        // Thermal sensor doesn't drain battery
        BatteryLevel = 100;
    }

    public string RevealInfo(ITerrorist terrorist)
    {
        if (revealedType == null && terrorist.RequiredSensorTypes.Any())
        {
            revealedType = terrorist.RequiredSensorTypes.First();
        }
        return revealedType.HasValue
            ? $"Revealed Type: {revealedType.Value}"
            : "No type revealed yet";
    }

    public override string GetStatus()
    {
        var baseStatus = base.GetStatus();
        return revealedType.HasValue
            ? $"{baseStatus}, Revealed Type: {revealedType.Value}"
            : baseStatus;
    }
}