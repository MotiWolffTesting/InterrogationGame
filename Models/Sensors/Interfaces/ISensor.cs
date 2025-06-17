namespace InterrogationGame.Models.Sensors;

public interface ISensor
{
    SensorType Type { get; }
    double BatteryLevel { get; }
    bool IsActive { get; }
    int ActivationCount { get; }
    bool CanActivate(int currentTurn);
    void Activate(int currentTurn);
    void Deactivate();
    string GetStatus();

    // New sensor extension properties
    int Durability { get; set; }
    int MaxDurability { get; }
    int CooldownTurns { get; set; }
    int MaxCooldownTurns { get; }
    bool IsCharging { get; set; }
    int ChargeTime { get; set; }
    int MaxChargeTime { get; }
    bool IsUpgraded { get; set; }
    int UpgradeLevel { get; set; }

    // New sensor extension methods
    void TakeDamage(int damage);
    void Repair(int amount);
    void Upgrade();
    void StartCharging();
    void UpdateCooldown();
    bool IsBroken();
    bool IsOnCooldown();
    bool IsChargingComplete();
}