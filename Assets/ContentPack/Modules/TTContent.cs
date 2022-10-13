using EntityStates;
using RoR2;
using RoR2.ContentManagement;
using RoR2.ExpansionManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using TreasureTrove.Misc;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TreasureTrove
{
    public class TTContent : IContentPackProvider
    {
        private static bool alreadyLoadedBaseGame = false;

        public delegate IEnumerator LoadStaticContentAsyncDelegate(LoadStaticContentAsyncArgs args);

        public delegate IEnumerator GenerateContentPackAsyncDelegate(GetContentPackAsyncArgs args);

        public delegate IEnumerator FinalizeAsyncDelegate(FinalizeAsyncArgs args);

        public static LoadStaticContentAsyncDelegate onLoadStaticContent { get; set; }
        public static GenerateContentPackAsyncDelegate onGenerateContentPack { get; set; }
        public static FinalizeAsyncDelegate onFinalizeAsync { get; set; }

        public string identifier => TreasureTroveUnityPlugin.ModIdentifier;
        public SerializableContentPack serializableContentPack; //Registration
        public ContentPack tempPackFromSerializablePack = new ContentPack(); //One step away from finalization

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            //Assetbundle fuckery, unity stuff.
            List<AssetBundle> loadedBundles = new List<AssetBundle>();
            var bundlePaths = Assets.GetAssetBundlePaths();
            int num;
            for (int i = 0; i < bundlePaths.Length; i = num)
            {
                var bundleLoadRequest = AssetBundle.LoadFromFileAsync(bundlePaths[i]);
                while (!bundleLoadRequest.isDone)
                {
                    args.ReportProgress(Util.Remap(bundleLoadRequest.progress + i, 0f, bundlePaths.Length, 0f, 0.8f));
                    yield return null;
                }
                num = i + 1;
                loadedBundles.Add(bundleLoadRequest.assetBundle);
            }

            //Content pack things, RoR2 systems.
            Assets.loadedAssetBundles = new ReadOnlyCollection<AssetBundle>(loadedBundles);
            serializableContentPack = Assets.mainAssetBundle.LoadAsset<SerializableContentPack>("TreasureTroveContentPack");
            
            tempPackFromSerializablePack = serializableContentPack.CreateContentPack();
            tempPackFromSerializablePack.identifier = identifier;

            //ContentLoadHelper.PopulateTypeFields<ArtifactDef>(typeof(TTContent.Artifacts), tempPackFromSerializablePack.artifactDefs);
            //ContentLoadHelper.PopulateTypeFields<ItemTierDef>(typeof(TTContent.ItemTiers), tempPackFromSerializablePack.itemTierDefs);
            //ContentLoadHelper.PopulateTypeFields<ItemDef>(typeof(TTContent.Items), tempPackFromSerializablePack.itemDefs);
            //ContentLoadHelper.PopulateTypeFields<EquipmentDef>(typeof(TTContent.Equipment), tempPackFromSerializablePack.equipmentDefs);
            //ContentLoadHelper.PopulateTypeFields<BuffDef>(typeof(TTContent.Buffs), tempPackFromSerializablePack.buffDefs, (string fieldName) => "bd" + fieldName);
            //ContentLoadHelper.PopulateTypeFields<EliteDef>(typeof(TEContent.Elites), contentPackFromSerializableContentPack.eliteDefs, (string fieldName) => "ed" + fieldName);
            ContentLoadHelper.PopulateTypeFields<SurvivorDef>(typeof(TTContent.Survivors), tempPackFromSerializablePack.survivorDefs);
            //ContentLoadHelper.PopulateTypeFields<ExpansionDef>(typeof(TTContent.Expansions), tempPackFromSerializablePack.expansionDefs);
            //ContentLoadHelper.PopulateTypeFields<SceneDef>(typeof(TTContent.Scenes), tempPackFromSerializablePack.sceneDefs);

            //This shouldn't go earlier than the type field population!
            //InitBuffs.Init();
            //InitVFX.Init();

            if (onLoadStaticContent != null)
                yield return onLoadStaticContent;

            args.ReportProgress(1f);
            yield break;
        }

        /// <summary>
        /// NOTE: Every instruction here will be done as many times as the current number of content packs the game has. This will cause serious errors if you do not watch out.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(tempPackFromSerializablePack, args.output);

            if (onGenerateContentPack != null)
                yield return onGenerateContentPack;

            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            if (Directory.Exists(Assets.languageRoot))
            {
                Language.collectLanguageRootFolders += (List<string> stringList) => stringList.Add(Assets.languageRoot);
                Misc.MiscLanguage.AddDeathMessages();
            }
            CostAndStatExtras.Init();

            //Extra stuff for hopoo
            RoR2Application.isModded = true;
            //Gets resolved to a hash, adding mod guid makes it unique, and adding modVer changes the hash generated with different versions. It can actually be literally any string desired. This is what appears in the mod mismatch error message when connecting to a remote server.
            NetworkModCompatibilityHelper.networkModList = NetworkModCompatibilityHelper.networkModList.Append(TreasureTroveUnityPlugin.ModGuid + ";" + TreasureTroveUnityPlugin.ModVer);

            args.ReportProgress(1f);
            yield break;
        }
        public static class Survivors
        {
            public static SurvivorDef Plague;
            public static SurvivorDef Specter;
        }
    }
}