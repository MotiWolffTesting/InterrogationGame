namespace InterrogationGame.Models.Sensors;

public interface ISensor
{
    SensorType Type { get; }
    double BatteryLevel { get; }
    bool IsActive { get; }
    bool CanActivate(int currentTurn);
    void Activate(int currentTurn);
    void Deactivate();
    string GetStatus();
}