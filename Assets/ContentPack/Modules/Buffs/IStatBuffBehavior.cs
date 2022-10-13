using RoR2;

namespace TreasureTrove
{
    public interface IStatBuffBehavior
    {
        void RecalculateStatsEnd();

        void RecalculateStatsStart(ref CharacterBody characterBody);
    }
}