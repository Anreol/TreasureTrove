using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreasureTrove.Components;
using UnityEngine;

namespace TreasureTrove.ScriptableObjects
{
    [CreateAssetMenu(menuName = "TreasureTrove/SkillDef/ResourcefulSkillDef")]
    public class ResourcefulSkillDef : SkillDef
    {
		public override SkillDef.BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
		{
			return new ResourcefulSkillDef.InstanceData
			{
				resourcefulSkillController = skillSlot.GetComponent<ResourcefulSkillController>()
			};
		}
		public override bool IsReady([NotNull] GenericSkill skillSlot)
		{
			return base.IsReady(skillSlot) && this.HasRequiredResource(skillSlot);
		}

		public bool HasRequiredResource([NotNull] GenericSkill skillSlot)
		{
			ResourcefulSkillDef.InstanceData instanceData = (ResourcefulSkillDef.InstanceData)skillSlot.skillInstanceData;
			return instanceData.resourcefulSkillController && (instanceData.resourcefulSkillController.resource >= this.minimumResource || canUseBelowMinimum) && instanceData.resourcefulSkillController.resource < this.maximumResource;
		}

		[Tooltip("Should it disregard the minimum Resource value and still let be used. Used in Resources that let themselves be exhausted.")]
		public bool canUseBelowMinimum = false;
		[Tooltip("Minimum Resource that it needs to have before it sets itself as available. Also the estimated amount to use.")]
		public float minimumResource;
		[Tooltip("Maximum amount of resource that it needs to be below of to be considered available.")]
		public float maximumResource;

		internal class InstanceData : SkillDef.BaseSkillInstanceData
		{
			public ResourcefulSkillController resourcefulSkillController;
		}
	}
}
