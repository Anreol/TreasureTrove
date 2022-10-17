using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace TreasureTrove.Components
{
    public class ResourcefulSkillController : NetworkBehaviour
    {
        private float _resource;

		[Header("Cached Components")]
		public CharacterBody characterBody;

		[Header("Resource Values")]
		public float maxResource;
		public float minResource;
		public float resource
		{
			get
			{
				return this._resource;
			}
		}
		public float resourceFraction
		{
			get
			{
				return resource / this.maxResource;
			}
		}
		public float resourceAsPercentage
		{
			get
			{
				return resourceFraction * 100f;
			}
		}

		public bool isFullResource
		{
			get
			{
				return resource >= maxResource;
			}
		}
		public bool willExhaust
		{
			get
			{
                if (characterBody && characterBody.skillLocator)
                {
					//characterBody.skillLocator.primary;
                }
				return this.characterBody;
			}
		}
		public bool isResourceAlwaysAvailable
		{
			get
			{
				return minResource >= maxResource;
			}
		}

		private HealthComponent bodyHealthComponent
		{
			get
			{
				return this.characterBody.healthComponent;
			}
		}
	}
}
