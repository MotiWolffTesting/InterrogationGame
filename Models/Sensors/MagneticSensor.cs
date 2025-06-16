using InterrogationGame.Models;

namespace InterrogationGame.Models.Sensors;

// Magnetic sensor
public class MagneticSensor : Sensor, ICancellable
{
    private int cancelCount = 2;
    public bool IsAbilityCancelled => cancelCount > 0;

    public MagneticSensor()
    {
        Type = SensorType.Magnetic;
        CooldownTurns = 3;
    }

    public override void Activate(int currentTurn)
    {
        base.Activate(currentTurn);
    }

    public void CancelAbility()
    {
        if (cancelCount > 0)
            cancelCount--;
    }

    public override string GetStatus()
    {
        return $"{base.GetStatus()}, Cancel Count: {cancelCount}";
    }
}