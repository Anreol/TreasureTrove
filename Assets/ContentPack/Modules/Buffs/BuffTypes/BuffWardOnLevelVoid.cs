using RoR2;

namespace TreasureTrove.Buffs
{
    public class BuffVoidWarbanner : Buff
    {
        public override BuffDef buffDef { get; set; } = TTContent.Buffs.WardOnLevelVoid;

        public override void RecalcStatsEnd(ref CharacterBody body)
        {
            body.armor += body.baseArmor * 0.2f;
            body.regen += body.baseRegen * 0.2f;
            body.moveSpeed += body.baseMoveSpeed * 0.2f;
            body.attackSpeed += body.baseAttackSpeed * 0.2f;
        }
    }
}