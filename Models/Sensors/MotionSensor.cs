using InterrogationGame.Models;

namespace InterrogationGame.Models.Sensors;

public class MotionSensor : Sensor
{
    public MotionSensor()
    {
        Type = SensorType.Motion;
        CooldownTurns = 2;
    }

    public override void Activate(int currentTurn)
    {
        base.Activate(currentTurn);

    }
}