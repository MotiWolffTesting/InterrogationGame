namespace InterrogationGame.Models;

public abstract class Sensor
{
    // Properties of each sensor - not all properties are required for each sensor
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
        return true;
    }

    public virtual void Ativate(int currentTurn)
    {
        if (!CanActivate(currentTurn)) return;

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