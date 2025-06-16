using System.Collections.Generic;
using InterrogationGame.Models.Sensors;

namespace InterrogationGame.Models;

public enum TerroristRank
{
    FootSoldier,
    SquadLeader,
    SeniorCommander,
    OrganizationLeader
}

public abstract class Terrorist : ITerrorist
{
    public Person Person { get; set; } = new();
    public abstract TerroristRank Rank { get; }
    public List<Sensor> SensorSlots { get; set; } = new();
    public List<SensorType> RequiredSensorTypes { get; set; } = new();
    public int TurnCount { get; set; }
    public abstract int MaxSensorSlots { get; }
    public abstract void PerformCounterattack();
    public void RemoveRandomSensor(int count)
    {
        var random = new Random();
        for (int i = 0; i < count && SensorSlots.Count > 0; i++)
        {
            int index = random.Next(SensorSlots.Count);
            SensorSlots.RemoveAt(index);
        }
    }
}