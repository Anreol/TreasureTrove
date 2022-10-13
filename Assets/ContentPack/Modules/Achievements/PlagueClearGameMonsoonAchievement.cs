using RoR2;
using RoR2.Achievements;

namespace TreasureTrove.Achievements
{
    [RegisterAchievement("PlagueClearGameMonsoon", "Skins.Grenadier.Alt1", null, null)]
    internal class PlagueClearGameMonsoonAchievement : BasePerSurvivorClearGameMonsoonAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("PlagueBody");
        }
    }
}