﻿using RoR2;
using RoR2.Items;
using RoR2.UI;
using System;
using System.Linq;
using TreasureTrove.Components;
using UnityEngine;
using UnityEngine.Networking;

namespace TreasureTrove.Items
{
    public class RadioSearchBodyBehavior : BaseItemBodyBehavior
    {
        [BaseItemBodyBehavior.ItemDefAssociationAttribute(useOnServer = false, useOnClient = true)]
        private static ItemDef GetItemDef()
        {
            return TTContent.Items.RadioSearch;
        }

        private GameObject[] alreadyCheckedChests = new GameObject[0]; //Should clean itself every stage change
        private float updateTimer;
        private const float updateInterval = 10f;

        private void FixedUpdate()
        {
            this.updateTimer -= Time.fixedDeltaTime;
            if (updateTimer <= 0f)
            {
                this.updateTimer = updateInterval;
                CheckForNearbyContent();
            }
        }

        private void CheckForNearbyContent()
        {
            foreach (var item in ChestRevealer.RevealedObject.currentlyRevealedObjects)
            {
                if (Vector3.Distance(body.transform.position, item.Key.transform.position) <= 12 && !alreadyCheckedChests.Contains(item.Key))
                {
                    HG.ArrayUtils.ArrayAppend(ref alreadyCheckedChests, item.Key);
                    if (Util.CheckRoll(25 + (stack - 1) * 5, body.master))
                    {
                        SpriteRenderer sprote = item.Value.positionIndicator.insideViewObject.GetComponent<SpriteRenderer>();
                        if (sprote && sprote.sprite == PingIndicator.GetInteractableIcon(item.Key.gameObject))
                        {
                            item.Key.AddComponent<PickupSpriteGetter>().spriteComponent = sprote;
                            item.Value.lifetime += 30;
                        }
                    }
                }
            }
        }

        [SystemInitializer(typeof(PickupCatalog))]
        public static void Initialize()
        {
            RoR2.Stage.onStageStartGlobal += StageStartGlobal; // RadioSearch
            RoR2.Run.onRunStartGlobal += PrepareTypes;
        }

        private static void PrepareTypes(Run obj)
        {
            RoR2.Run.onRunStartGlobal -= PrepareTypes;
            foreach (var item in ChestRevealer.typesToCheck)
            {
                if (!bannedTypes.Contains(item))
                {
                    HG.ArrayUtils.ArrayAppend(ref trimmedTypesToCheck, item);
                }
            }
        }

        private static void StageStartGlobal(Stage obj)
        {
            if (Util.GetItemCountForTeam(TeamIndex.Player, TTContent.Items.RadioSearch.itemIndex, true) > 0)
            {
                GetSignal();
                if (NetworkServer.active)
                    FindRadioTowers();
            }
        }

        private static void GetSignal()
        {
            currentRevealCount = 0;
            numberToReveal = Util.GetItemCountForTeam(TeamIndex.Player, TTContent.Items.RadioSearch.itemIndex, true);
            MonoBehaviour[] things = new MonoBehaviour[0];
            for (int i = 0; i < trimmedTypesToCheck.Length; i++)
            {
                foreach (MonoBehaviour monoBehaviour in InstanceTracker.FindInstancesEnumerable(trimmedTypesToCheck[i]))
                {
                    if (((IInteractable)monoBehaviour).ShouldShowOnScanner())
                    {
                        HG.ArrayUtils.ArrayAppend(ref things, monoBehaviour);
                    }
                }
            }
            numberToReveal += (int)(things.Length * 0.15);
            for (int i = 0; i < things.Length; i++)
            {
                if (currentRevealCount > numberToReveal)
                    continue;
                TryAddRevealer(things[i].gameObject.transform);
                currentRevealCount++;
            }
        }

        private static void FindRadioTowers()
        {
            foreach (NetworkBehaviour item in InstanceTracker.FindInstancesEnumerable(typeof(RadiotowerTerminal)))
            {
                item.gameObject.GetComponent<PurchaseInteraction>().onPurchase.AddListener((interactor) => CheckItem(interactor));
            }
        }

        private static void CheckItem(Interactor interactor)
        {
            if (interactor.GetComponent<CharacterBody>().inventory.GetItemCount(ItemCatalog.FindItemIndex("RadioSearch")) > 0)
            {
                NetworkServer.Spawn(UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/NetworkedObjects/ChestScanner"), interactor.transform.position, Quaternion.identity));
            }
        }

        private static void TryAddRevealer(Transform revealableTransform)
        {
            ChestRevealer.PendingReveal pendingReveal = new ChestRevealer.PendingReveal
            {
                gameObject = revealableTransform.gameObject,
                time = Run.FixedTimeStamp.now,
                duration = revealDuration,
            };
            ChestRevealer.pendingReveals.Add(pendingReveal);
        }

        private const float revealDuration = 60f;
        private static Type[] trimmedTypesToCheck = new Type[0];

        private static Type[] bannedTypes = new Type[]
        {
            typeof(BarrelInteraction),
            typeof(TeleporterInteraction),
            typeof(VehicleSeat),
            typeof(BazaarUpgradeInteraction),
            typeof(ProxyInteraction)
        };

        private static double currentRevealCount;
        private static int numberToReveal;
    }
}