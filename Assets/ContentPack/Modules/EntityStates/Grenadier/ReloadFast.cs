using EntityStates;
using RoR2;
using TreasureTrove.Components;
using UnityEngine;

namespace TreasureTrove.EntityStates.Grenadier.Weapon
{
    public class ReloadFast : Reload
    {
        public override Reload GetNextState()
        {
            return new ReloadFast();
        }
    }
}