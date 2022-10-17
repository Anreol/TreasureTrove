using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace TreasureTrove.Components
{
    [RequireComponent(typeof(CharacterBody))]
    public class PlagueWeaponryController : NetworkBehaviour
    {
        [Header("Generic Skill slots")]
        public GenericSkill bombPowderSkill;
        public GenericSkill bombCasingSkill;
        public GenericSkill bombFuseSkill;
        public GenericSkill arcanaSlot1Skill;
        public GenericSkill arcanaSlot2Skill;

        [Header("Skill Families")]
        public SkillFamily bombPowderSkillDefs;
        public SkillFamily bombCasingSkillDefs;
        public SkillFamily bombFuseSkillDefs;
        public SkillFamily arcanaSkillDefs;

        [SyncVar]
        private int netEnabledSkillsMask;

        private CharacterBody characterBody;
        private PlagueMasterWeaponryStorage plagueMasterWeaponryStorage;
        private bool hasDoneInitialReset;
        private SkillDef currentlyOverridingPrimarySkill;
        private SkillDef currentlyOverridingSecondarySkill;
        private SkillDef currentlyOverridingUtilitySkill;
        private int authoritySelectedSkillsMask;

        private bool hasEffectiveAuthority
        {
            get
            {
                return this.characterBody.hasEffectiveAuthority;
            }
        }

        private void Awake()
        {
            this.characterBody = base.GetComponent<CharacterBody>();
            this.hasDoneInitialReset = false;
        }

        private void FixedUpdate()
        {
            this.UpdateSkillOverrides();
            if (this.hasEffectiveAuthority && !this.hasDoneInitialReset)
            {
                if(characterBody.master)
                {
                    plagueMasterWeaponryStorage = characterBody.master.gameObject.GetComponent<PlagueMasterWeaponryStorage>();
                    //It's null, lets save current config (from loadout) into it.
                    if (plagueMasterWeaponryStorage == null)
                    {
                        plagueMasterWeaponryStorage = characterBody.master.gameObject.AddComponent<PlagueMasterWeaponryStorage>();
                        plagueMasterWeaponryStorage.SaveConfiguration(bombPowderSkill.skillDef, bombCasingSkill.skillDef, bombFuseSkill.skillDef, arcanaSlot1Skill.skillDef, arcanaSlot2Skill.skillDef);
                    }

                }
                this.hasDoneInitialReset = true;
                if (this.bombPowderSkill)
                {
                    this.bombPowderSkill.Reset();
                }
                if (this.arcanaSlot1Skill)
                {
                    this.arcanaSlot1Skill.Reset();
                }
                if (this.arcanaSlot2Skill)
                {
                    this.arcanaSlot2Skill.Reset();
                }
            }
        }

        private void OnDisable()
        {
            if (bombPowderSkill)
            {
                SetSkillOverride(ref currentlyOverridingPrimarySkill, null, bombPowderSkill);
            }
            if (arcanaSlot1Skill)
            {
                SetSkillOverride(ref currentlyOverridingSecondarySkill, null, arcanaSlot1Skill);
            }
            if (arcanaSlot2Skill)
            {
                SetSkillOverride(ref currentlyOverridingUtilitySkill, null, arcanaSlot2Skill);
            }
        }

        private void SetSkillOverride([CanBeNull] ref SkillDef currentSkillDef, [CanBeNull] SkillDef newSkillDef, [NotNull] GenericSkill component)
        {
            if (currentSkillDef == newSkillDef)
            {
                return;
            }
            if (currentSkillDef != null)
            {
                component.UnsetSkillOverride(this, currentSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            }
            currentSkillDef = newSkillDef;
            if (currentSkillDef != null)
            {
                component.SetSkillOverride(this, currentSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            }
        }

        private void UpdateSkillOverrides()
        {
            if (!base.enabled)
            {
                return;
            }
            int newServerMask;
            if (this.hasEffectiveAuthority)
            {
                int newAuthorityMask = 0;
                if (this.currentlyOverridingPrimarySkill)
                {
                    //000011?
                    newAuthorityMask |= bombPowderSkillDefs.GetVariantIndex(currentlyOverridingPrimarySkill.skillName);
                }
                if (this.currentlyOverridingSecondarySkill)
                {
                    //002200??
                    newAuthorityMask |= arcanaSkillDefs.GetVariantIndex(currentlyOverridingSecondarySkill.skillName) << 8;
                }
                if (this.currentlyOverridingUtilitySkill)
                {
                    //330000??
                    newAuthorityMask |= arcanaSkillDefs.GetVariantIndex(currentlyOverridingUtilitySkill.skillName) << 16;
                }
                if (newAuthorityMask != this.authoritySelectedSkillsMask)
                {
                    this.authoritySelectedSkillsMask = newAuthorityMask;
                    if (NetworkServer.active)
                    {
                        this.netEnabledSkillsMask = this.authoritySelectedSkillsMask;
                    }
                    else
                    {
                        this.CmdSetSkillMask(this.authoritySelectedSkillsMask);
                    }
                }
                newServerMask = this.authoritySelectedSkillsMask;
            }
            else
            {
                newServerMask = this.netEnabledSkillsMask;
            }
            //replaces everything past the first two numbers?
            int bombIndex = newServerMask & 0xff;
            //Starts from the third number, then cuts off at the fourth number?
            int arcana1Index = newServerMask >> 8 & 0xff;
            //takes the last eight
            int arcana2Index = newServerMask >> 16;

            this.SetSkillOverride(ref this.currentlyOverridingPrimarySkill, bombPowderSkillDefs.variants[bombIndex].skillDef, bombPowderSkill);
            this.SetSkillOverride(ref this.currentlyOverridingSecondarySkill, arcanaSkillDefs.variants[arcana1Index].skillDef, arcanaSlot1Skill);
            this.SetSkillOverride(ref this.currentlyOverridingUtilitySkill, arcanaSkillDefs.variants[arcana2Index].skillDef, arcanaSlot2Skill);
        }

        [Command]
        private void CmdSetSkillMask(int newMask)
        {
            netEnabledSkillsMask = newMask;
        }
    }
}