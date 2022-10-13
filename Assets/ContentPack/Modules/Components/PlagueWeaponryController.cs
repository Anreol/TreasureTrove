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

        [Header("Skill Defs")]
        public SkillDef[] bombPowderSkillDefs;
        public SkillDef[] bombCasingSkillDefs;
        public SkillDef[] bombFuseSkillDefs;
        public SkillDef[] arcanaSkillDefs;

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
                    newAuthorityMask |= System.Array.IndexOf(bombPowderSkillDefs, currentlyOverridingPrimarySkill);
                }
                if (this.currentlyOverridingSecondarySkill)
                {
                    newAuthorityMask |= System.Array.IndexOf(arcanaSkillDefs, currentlyOverridingSecondarySkill) << 4;
                }
                if (this.currentlyOverridingUtilitySkill)
                {
                    newAuthorityMask |= System.Array.IndexOf(arcanaSkillDefs, currentlyOverridingUtilitySkill) << 8;
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
            int bombIndex = newServerMask >> 8;
            int arcana1Index = newServerMask >> 4;
            int arcana2Index = newServerMask;
            this.SetSkillOverride(ref this.currentlyOverridingPrimarySkill, bombPowderSkillDefs[bombIndex], bombPowderSkill);
            this.SetSkillOverride(ref this.currentlyOverridingSecondarySkill, arcanaSkillDefs[arcana1Index], arcanaSlot1Skill);
            this.SetSkillOverride(ref this.currentlyOverridingUtilitySkill, arcanaSkillDefs[arcana2Index], arcanaSlot2Skill);
        }

        [Command]
        private void CmdSetSkillMask(int newMask)
        {
            netEnabledSkillsMask = newMask;
        }
    }
}