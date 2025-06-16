using System.Collections.Generic;

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
            RequiredSensorTypes.Clear();
        }
        else if (TurnCount % 3 == 0)
        {
            RemoveRandomSensor(1);
        }
    }
}