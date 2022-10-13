using EntityStates;
using RoR2;

namespace TreasureTrove.EntityStates.Grenadier.Weapon
{
    public class EnterReloadFast : EnterReload
    {
        public override Reload GetNextState()
        {
            return new ReloadFast();
        }
    }
}