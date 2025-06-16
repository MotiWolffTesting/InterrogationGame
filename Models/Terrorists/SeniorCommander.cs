using System.Collections.Generic;

namespace InterrogationGame.Models.Terrorists;

public class SeniorCommander : Terrorist
{
    public override TerroristRank Rank => TerroristRank.SeniorCommander;
    public override int MaxSensorSlots => 6;
    public override void PerformCounterattack()
    {
        if (TurnCount % 3 == 0)
        {
            RemoveRandomSensor(2);
        }
    }
}