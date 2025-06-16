using InterrogationGame.Models;

namespace InterrogationGame.Models.Sensors;

public class AudioSensor : Sensor
{

    // The simplest sensor, no special ability
    public AudioSensor()
    {
        Type = SensorType.Audio;
        CooldownTurns = 0;
    }

    public override void Activate(int currentTurn)
    {
        base.Activate(currentTurn);
    }
}