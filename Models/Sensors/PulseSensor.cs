using InterrogationGame.Models;

namespace InterrogationGame.Models.Sensors;

public class PulseSensor : Sensor
{
    public PulseSensor()
    {
        Type = SensorType.Pulse;
        CooldownTurns = 1;
    }

    public override void Activate(int currentTurn)
    {
        base.Activate(currentTurn);
    }

    public override string GetStatus()
    {
        return $"{base.GetStatus()}, Pulse Detection Active";
    }
}