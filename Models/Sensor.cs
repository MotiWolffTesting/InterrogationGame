namespace InterrogationGame.Models.Sensors;

public abstract class Sensor : ISensor
{
    // Properties of each sensor - not all properties are required for each sensor
    protected const int MaxActivations = 3;
    public SensorType Type { get; protected set; }
    public double BatteryLevel { get; protected set; } = 100.0;
    public bool IsActive { get; protected set; }
    public int ActivationCount { get; protected set; }
    public int CooldownTurns { get; protected set; }
    public int LastActivationTurn { get; protected set; }

    public virtual bool CanActivate(int currentTurn)
    {
        if (BatteryLevel <= 0) return false;
        if (CooldownTurns > 0 && currentTurn - LastActivationTurn < CooldownTurns) return false;
        if (ActivationCount >= MaxActivations) return false;
        return true;
    }

    public virtual void Activate(int currentTurn)
    {
        if (!CanActivate(currentTurn))
        {
            Console.WriteLine($"{GetType().Name} cannot be activated: " +
                            (BatteryLevel <= 0 ? "Battery depleted" :
                             ActivationCount >= MaxActivations ? "Max activations reached" :
                             "On cooldown"));
            return;
        }

        LastActivationTurn = currentTurn;
        ActivationCount++;
        IsActive = true;

        // Default battery drain
        BatteryLevel -= 10;
    }

    public virtual void Deactivate()
    {
        IsActive = false;
    }

    public virtual string GetStatus()
    {
        return $"Type: {Type}, Battery: {BatteryLevel:F1}%, Active: {IsActive}";
    }
}