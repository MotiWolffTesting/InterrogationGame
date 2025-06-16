using System.Collections.Generic;

namespace InterrogationGame.Models.Terrorists;

public class FootSoldier : Terrorist
{
    public override TerroristRank Rank => TerroristRank.FootSoldier;
    public override int MaxSensorSlots => 2;
    public override void PerformCounterattack()
    {
        // No counterattack for Foot Soldier
    }
}