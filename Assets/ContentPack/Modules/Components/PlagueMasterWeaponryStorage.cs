using RoR2.Skills;
using UnityEngine;

namespace TreasureTrove.Components
{
    public class PlagueMasterWeaponryStorage : MonoBehaviour
    {
        public SkillDef ChosenBombPowderSkill
        {
            get => chosenBombPowderSkill;
            set
            {
                if (value != chosenBombPowderSkill)
                {
                    chosenBombCasingSkill = value;
                }
            }
        }

        public SkillDef ChosenBombCasingSkill
        {
            get => chosenBombCasingSkill; set
            {
                if (value != chosenBombCasingSkill)
                {
                    chosenBombCasingSkill = value;
                }
            }
        }

        public SkillDef ChosenBombFuseSkill
        {
            get => chosenBombFuseSkill;
            set
            {
                if (value != chosenBombFuseSkill)
                {
                    chosenBombFuseSkill = value;
                }
            }
        }

        public SkillDef ChosenArcanaSlot1Skill
        {
            get => chosenArcanaSlot1Skill;
            set
            {
                if (value != chosenArcanaSlot1Skill)
                {
                    chosenArcanaSlot1Skill = value;
                }
            }
        }

        public SkillDef ChosenArcanaSlot2Skill
        {
            get => chosenArcanaSlot2Skill;
            set
            {
                if (value != chosenArcanaSlot2Skill)
                {
                    chosenArcanaSlot2Skill = value;
                }
            }
        }

        private SkillDef chosenBombPowderSkill;
        private SkillDef chosenBombCasingSkill;
        private SkillDef chosenBombFuseSkill;
        private SkillDef chosenArcanaSlot1Skill;
        private SkillDef chosenArcanaSlot2Skill;

        public void SaveConfiguration(SkillDef newBombPowder = null, SkillDef newBombCasing = null, SkillDef newBombFuse = null, SkillDef newArcanaOffense = null, SkillDef newArcanaDefense = null)
        {
            if (newBombPowder != chosenBombPowderSkill)
            {
                chosenBombPowderSkill = newBombPowder;
            }
            if (newBombCasing != chosenBombCasingSkill)
            {
                chosenBombCasingSkill = newBombCasing;
            }
            if (newBombFuse != chosenBombFuseSkill)
            {
                chosenBombFuseSkill = newBombFuse;
            }
            if (newArcanaOffense != chosenArcanaSlot1Skill)
            {
                chosenArcanaSlot1Skill = newArcanaOffense;
            }
            if (newArcanaDefense != chosenArcanaSlot2Skill)
            {
                chosenArcanaSlot2Skill = newArcanaDefense;
            }
        }
    }
}