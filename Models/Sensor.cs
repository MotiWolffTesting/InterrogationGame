namespace InterrogationGame.Models.Sensors;

public abstract class Sensor : ISensor
{
    // Properties of each sensor - not all properties are required for each sensor
    protected const int MaxActivations = 3;
    public SensorType Type { get; protected set; }
    public double BatteryLevel { get; protected set; } = 100.0;
    public bool IsActive { get; protected set; }
    public int ActivationCount { get; protected set; }
    public int CooldownTurns { get; set; }
    public int LastActivationTurn { get; protected set; }

    // New sensor extension properties
    public int Durability { get; set; } = 100;
    public int MaxDurability { get; protected set; } = 100;
    public int MaxCooldownTurns { get; protected set; } = 2;
    public bool IsCharging { get; set; } = false;
    public int ChargeTime { get; set; } = 0;
    public int MaxChargeTime { get; protected set; } = 3;
    public bool IsUpgraded { get; set; } = false;
    public int UpgradeLevel { get; set; } = 0;

    public virtual bool CanActivate(int currentTurn)
    {
        if (BatteryLevel <= 0) return false;
        if (CooldownTurns > 0 && currentTurn - LastActivationTurn < CooldownTurns) return false;
        if (ActivationCount >= MaxActivations) return false;
        if (IsBroken()) return false;
        if (IsOnCooldown()) return false;
        return true;
    }

    public virtual void Activate(int currentTurn)
    {
        if (!CanActivate(currentTurn))
        {
            Console.WriteLine($"{GetType().Name} cannot be activated: " +
                            (BatteryLevel <= 0 ? "Battery depleted" :
                             ActivationCount >= MaxActivations ? "Max activations reached" :
                             IsBroken() ? "Sensor is broken" :
                             IsOnCooldown() ? "Sensor is on cooldown" :
                             "On cooldown"));
            return;
        }

        LastActivationTurn = currentTurn;
        ActivationCount++;
        IsActive = true;

        // Default battery drain and durability loss
        BatteryLevel -= 10;
        TakeDamage(5); // Small durability damage on activation
    }

    public virtual void Deactivate()
    {
        IsActive = false;
    }

    public virtual string GetStatus()
    {
        var status = $"Type: {Type}, Battery: {BatteryLevel:F1}%, Active: {IsActive}";
        if (IsBroken()) status += ", BROKEN";
        if (IsOnCooldown()) status += $", Cooldown: {CooldownTurns} turns";
        if (IsCharging) status += $", Charging: {ChargeTime}/{MaxChargeTime}";
        if (IsUpgraded) status += $", Level {UpgradeLevel}";
        status += $", Durability: {Durability}/{MaxDurability}";
        return status;
    }

    // New sensor extension methods
    public virtual void TakeDamage(int damage)
    {
        Durability = Math.Max(0, Durability - damage);
        if (Durability <= 0)
        {
            IsActive = false;
            Console.WriteLine($"{GetType().Name} has been destroyed!");
        }
    }

    public virtual void Repair(int amount)
    {
        Durability = Math.Min(MaxDurability, Durability + amount);
        if (Durability > 0 && IsBroken())
        {
            Console.WriteLine($"{GetType().Name} has been repaired!");
        }
    }

    public virtual void Upgrade()
    {
        if (!IsUpgraded)
        {
            IsUpgraded = true;
            UpgradeLevel = 1;
            MaxDurability += 25;
            Durability = MaxDurability;
            MaxChargeTime -= 1;
            Console.WriteLine($"{GetType().Name} has been upgraded to Level 1!");
        }
        else if (UpgradeLevel < 3)
        {
            UpgradeLevel++;
            MaxDurability += 25;
            Durability = MaxDurability;
            MaxChargeTime = Math.Max(1, MaxChargeTime - 1);
            Console.WriteLine($"{GetType().Name} has been upgraded to Level {UpgradeLevel}!");
        }
    }

    public virtual void StartCharging()
    {
        if (!IsCharging && !IsBroken())
        {
            IsCharging = true;
            ChargeTime = 0;
            Console.WriteLine($"{GetType().Name} has started charging.");
        }
    }

    public virtual void UpdateCooldown()
    {
        if (CooldownTurns > 0)
        {
            CooldownTurns--;
        }

        if (IsCharging)
        {
            ChargeTime++;
            if (IsChargingComplete())
            {
                IsCharging = false;
                BatteryLevel = Math.Min(100.0, BatteryLevel + 30.0);
                Console.WriteLine($"{GetType().Name} has finished charging!");
            }
        }
    }

    public virtual bool IsBroken()
    {
        return Durability <= 0;
    }

    public virtual bool IsOnCooldown()
    {
        return CooldownTurns > 0;
    }

    public virtual bool IsChargingComplete()
    {
        return IsCharging && ChargeTime >= MaxChargeTime;
    }
}