using System.Collections.Generic;

namespace InterrogationGame.Models.Sensors;

public interface ITerrorist
{
    Person Person { get; set; }
    TerroristRank Rank { get; }
    List<Sensor> SensorSlots { get; set; }
    List<SensorType> RequiredSensorTypes { get; set; }
    int TurnCount { get; set; }
    int MaxSensorSlots { get; }
    void PerformCounterattack();
    void RemoveRandomSensor(int count);
}