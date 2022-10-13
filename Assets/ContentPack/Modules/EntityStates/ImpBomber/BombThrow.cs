﻿using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TreasureTrove.EntityStates.ImpBomber.Weapon
{
    class BombThrow : GenericProjectileBaseState
    {
        public static string bombBoneChildName;
        [SerializeField]
        public GameObject bombPrefabDefault;
        private ChildLocator childLocator;
        private GameObject bombInstance;

        public override void OnEnter()
        {
            this.childLocator = base.GetModelChildLocator();
            if (this.childLocator)
            {
                Transform transform = this.childLocator.FindChild(bombBoneChildName) ?? base.characterBody.coreTransform;
                if (transform && this.bombPrefabDefault)
                {
                    this.bombInstance = UnityEngine.Object.Instantiate<GameObject>(this.bombPrefabDefault, transform.position, transform.rotation, transform);
                    bombInstance.GetComponent<ChildLocator>().FindChild("Light").gameObject.SetActive(true);
                }
            }
            base.GetModelAnimator().SetBool("BombHolding.active", false);
            base.OnEnter();
        }
        public override void FireProjectile()
        {
            if (bombInstance)
            {
                UnityEngine.Object.Destroy(bombInstance);
            }
            base.FireProjectile();
        }
        public override void PlayAnimation(float duration)
        {
            base.PlayAnimation(duration);
            base.PlayCrossfade("Gesture, Additive", "ThrowBomb", "BombThrow.playbackRate", duration, 0.1f);
        }

    }
}