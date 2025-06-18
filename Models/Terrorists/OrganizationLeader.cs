using System.Collections.Generic;
using InterrogationGame.Models;

namespace InterrogationGame.Models.Terrorists;

public class OrganizationLeader : Terrorist
{
    public override TerroristRank Rank => TerroristRank.OrganizationLeader;
    public override int MaxSensorSlots => 8;
    public override void PerformCounterattack()
    {
        if (TurnCount % 10 == 0)
        {
            SensorSlots.Clear();

            var allTypes = Enum.GetValues<SensorType>();
            RequiredSensorTypes = Enumerable.Range(0, MaxSensorSlots)
                .Select(_ => allTypes[new Random().Next(allTypes.Length)])
                .ToList();
        }
        else if (TurnCount % 3 == 0)
        {
            RemoveRandomSensor(1);
        }
    }
}