using InterrogationGame.Models;

namespace InterrogationGame.Models.Sensors;

public class SignalSensor : Sensor, IRevealable
{
    public SignalSensor()
    {
        Type = SensorType.Signal;
        CooldownTurns = 2;
    }

    public override void Activate(int currentTurn)
    {
        base.Activate(currentTurn);
    }

    public string RevealInfo(ITerrorist terrorist)
    {
        // For demonstration, reveal the rank
        return $"Terrorist Rank: {terrorist.Rank}";
    }
}