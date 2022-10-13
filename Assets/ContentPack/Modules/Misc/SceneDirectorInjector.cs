﻿using RoR2;
using System;
using TreasureTrove.ScriptableObjects;
using UnityEngine;

namespace TreasureTrove.Misc
{
    internal class SceneDirectorInjector
    {
        private static SerializableDirectorCard[] cards;
        private static bool logCards = false;

        [SystemInitializer(new Type[]
        {
            typeof(SceneCatalog),
        })]
        private static void Init()
        {
            cards = new SerializableDirectorCard[]
            {
                Assets.mainAssetBundle.LoadAsset<SerializableDirectorCard>("sdcShrineOvercharger"),
                Assets.mainAssetBundle.LoadAsset<SerializableDirectorCard>("sdcShrineOverchargerCommon")
            };
            //TODO: AS OF JULY 4, LEAVE IT FOR NEXT UPDATE
            //SceneDirector.onGenerateInteractableCardSelection += AddDirectorCards;
            SceneDirector.onPrePopulateMonstersSceneServer += ExplicitInteracteableGeneration;

            SceneCollection.SceneEntry observatoryEntry = new SceneCollection.SceneEntry()
            {
                sceneDef = TTContent.Scenes.observatory,
                weightMinusOne = 0
            };
            HG.ArrayUtils.ArrayAppend<SceneCollection.SceneEntry>(ref SceneCatalog.GetSceneDefFromSceneName("dampcavesimple").destinationsGroup._sceneEntries, observatoryEntry);
        }

        private static void ExplicitInteracteableGeneration(SceneDirector obj)
        {
            Xoroshiro128Plus xoroshiro128Plus = new Xoroshiro128Plus(obj.rng.nextUlong);
            if (Util.GetItemCountForTeam(TeamIndex.Player, TTContent.Items.MoneyBank.itemIndex, false, true) > 0)
            {
                Transform moneyBankTarget;
                if (SceneInfo.instance.countsAsStage)
                {
                    moneyBankTarget = TeleporterInteraction.instance ? TeleporterInteraction.instance.transform : SpawnPoint.readOnlyInstancesList[xoroshiro128Plus.RangeInt(0, SpawnPoint.readOnlyInstancesList.Count)].transform;
                }
                else
                {
                    moneyBankTarget = SpawnPoint.readOnlyInstancesList[xoroshiro128Plus.RangeInt(0, SpawnPoint.readOnlyInstancesList.Count)].transform;
                }
                if (moneyBankTarget)
                {
                    DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(Assets.mainAssetBundle.LoadAsset<SpawnCard>("iscMoneyBank"), new DirectorPlacementRule
                    {
                        placementMode = DirectorPlacementRule.PlacementMode.Approximate, //Let's me spawn in the target, and specify min and max distances,
                        maxDistance = 80f,
                        minDistance = 6f,
                        spawnOnTarget = moneyBankTarget
                    }, xoroshiro128Plus));
                }
            }
        }

        private static void LogDirectorCards(SceneDirector arg1, DirectorCardCategorySelection arg2)
        {
            if (logCards)
            {
                TTLog.LogE("\n\nLogging cards\n\n");
                foreach (var item in arg2.categories)
                {
                    TTLog.LogW("Director category: " + item.name);
                    foreach (var card in item.cards)
                    {
                        TTLog.LogD("Spawn Card: " + card.spawnCard + " Prefab: " + card.spawnCard.prefab + "Is valid: " + card.IsAvailable());
                    }
                }
            }
        }

        private static void AddDirectorCards(SceneDirector arg1, DirectorCardCategorySelection arg2)
        {
            foreach (SerializableDirectorCard sdc in cards)
            {
                for (int i = 0; i < sdc.sceneNamesToBeUsedIn.Length; i++)
                {
                    if (SceneCatalog.mostRecentSceneDef == SceneCatalog.GetSceneDefFromSceneName(sdc.sceneNamesToBeUsedIn[i]))
                    {
                        int index = FixedFindCategoryIndexByName(ref arg2, sdc.categoryName);
                        if (index != -1)
                        {
                            arg2.AddCard(index, sdc.CreateDirectorCard());
                        }
                        continue;
                    }
                }
            }
        }

        public static int FixedFindCategoryIndexByName(ref DirectorCardCategorySelection dccs, string categoryName)
        {
            for (int i = 0; i < dccs.categories.Length; i++)
            {
                if (string.CompareOrdinal(dccs.categories[i].name, categoryName) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        [ConCommand(commandName = "te_list_DirectorInteractableCardsOnStageChange", flags = ConVarFlags.None, helpText = "Lists all ListDirectorInteractable cards whenever a stage changes.")]
        private static void ListDirectorInteractable(ConCommandArgs args)
        {
            logCards = args.TryGetArgBool(0) ?? false;
        }
    }
}