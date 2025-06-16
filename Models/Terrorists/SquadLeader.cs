using System.Collections.Generic;

namespace InterrogationGame.Models.Terrorists;

public class SquadLeader : Terrorist
{
    public override TerroristRank Rank => TerroristRank.SquadLeader;
    public override int MaxSensorSlots => 4;
    public override void PerformCounterattack()
    {
        if (TurnCount % 3 == 0)
        {
            RemoveRandomSensor(1);
        }
    }
}