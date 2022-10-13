﻿using EntityStates;

namespace TreasureTrove.EntityStates.Grenadier.SideWeapon
{
    internal class FireSecondaryAlt : GenericProjectileBaseState
    {
        public static float minimumDuration;
        public static float selfForce;

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (fixedAge <= minimumDuration)
            {
                return InterruptPriority.PrioritySkill;
            }
            return base.GetMinimumInterruptPriority();
        }
        public override void FireProjectile()
        {
            base.FireProjectile();
            if (base.characterMotor && !characterMotor.isGrounded)
            {
                base.characterMotor.ApplyForce(GetAimRay().direction * -selfForce, false, false);
            }
        }
    }
}