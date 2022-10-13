using RoR2;
using RoR2.UI;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace TreasureTrove.Misc
{
    internal class UIExtras
    {
        internal static GameObject mainMenuObject; //WHAT IS ENCAPSULATION AMIRITE

        [SystemInitializer]
        private static void Init()
        {
            if (RoR2.RoR2Application.isDedicatedServer || Application.isBatchMode) //We dont need graphics
                return;
            CameraRigController.onCameraEnableGlobal += onCameraEnabledGlobal;
            //SceneCatalog.onMostRecentSceneDefChanged += onMostRecentSceneDefChanged;
        }


        //public static GameObject statBarContainer = Assets.mainAssetBundle.LoadAsset<GameObject>("StatBarsContainer");
        //public static GameObject scoreboardLeftSidePanel = Assets.mainAssetBundle.LoadAsset<GameObject>("ScoreboardLeftSidePanel");

        private static void onCameraEnabledGlobal(CameraRigController obj)
        {
            if (obj)
            {
                //TurboUnityPlugin.instance.StartCoroutine(AwaitForHUDCreationAndAppend(obj, statBarContainer, "MainContainer/MainUIArea/SpringCanvas/LeftCluster"));
                //TurboUnityPlugin.instance.StartCoroutine(AwaitForHUDCreationAndAppend(obj, scoreboardLeftSidePanel, "MainContainer/MainUIArea/SpringCanvas"));
                //TurboUnityPlugin.instance.StartCoroutine(AwaitForHUDCreationAndPatch(obj));
            }
        }

        private static void onMostRecentSceneDefChanged(SceneDef obj)
        {
            //if (obj == SceneCatalog.GetSceneDefFromSceneName("title"))
            //{
            //    mainMenuObject = GameObject.Find("MainMenu");
            //    RoR2.UI.MainMenu.MainMenuController mmc = mainMenuObject.GetComponent<RoR2.UI.MainMenu.MainMenuController>();
            //    if (mmc.multiplayerMenuScreen)
            //    {
            //        GameObject itemHolder = mmc.multiplayerMenuScreen.transform.parent.GetChild(0).GetChild(1).GetChild(1).GetChild(26).GetChild(4).gameObject;
            //        LocalUser firstLocalUser = LocalUserManager.GetFirstLocalUser();
            //        if (firstLocalUser.userProfile.HasDiscoveredPickup(PickupCatalog.itemIndexToPickupIndex[(int)TEContent.Items.StandBonus.itemIndex]))
            //        {
            //            GameObject gameObject = UnityEngine.Object.Instantiate(Assets.mainAssetBundle.LoadAsset<GameObject>("PickupSandBag"), new Vector3(-5.4547f, 597.8f, -430.8293f), Quaternion.Euler(0f, 152.6924f, 0), itemHolder.transform);
            //            gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            //            gameObject.SetActive(true); //Just in case.
            //        }
            //        if (firstLocalUser.userProfile.HasDiscoveredPickup(PickupCatalog.itemIndexToPickupIndex[(int)TEContent.Items.DropletDupe.itemIndex]))
            //        {
            //            GameObject gameObject = UnityEngine.Object.Instantiate(Assets.mainAssetBundle.LoadAsset<GameObject>("DisplayDropletDupe"), new Vector3(-5.06f, 598.34f, -433.3199f), Quaternion.Euler(270f, 328f, 0), itemHolder.transform);
            //            gameObject.SetActive(true); //Just in case.
            //        }
            //    }
            //}
        }

        private static void PatchHUD(HUD newHud)
        {
            ChildLocator cl = newHud.GetComponent<ChildLocator>();
            if (cl)
            {
                ChildLocator.NameTransformPair bottomRightCorner = new ChildLocator.NameTransformPair(){
                    name = newHud.skillIcons[0].transform.parent.parent.name,
                    transform = newHud.skillIcons[0].transform.parent.parent
                };
                ChildLocator.NameTransformPair skill1 = new ChildLocator.NameTransformPair()
                {
                    name = newHud.skillIcons[0].gameObject.name,
                    transform = newHud.skillIcons[0].transform
                };
                ChildLocator.NameTransformPair skill2 = new ChildLocator.NameTransformPair()
                {
                    name = newHud.skillIcons[1].gameObject.name,
                    transform = newHud.skillIcons[1].transform
                };
                ChildLocator.NameTransformPair skill3 = new ChildLocator.NameTransformPair()
                {
                    name = newHud.skillIcons[2].gameObject.name,
                    transform = newHud.skillIcons[2].transform
                };
                ChildLocator.NameTransformPair skill4 = new ChildLocator.NameTransformPair()
                {
                    name = newHud.skillIcons[3].gameObject.name,
                    transform = newHud.skillIcons[3].transform
                };
                cl.transformPairs = cl.transformPairs.Concat(new ChildLocator.NameTransformPair[] { bottomRightCorner, skill1, skill2, skill3, skill4 }).ToArray();
            }
        }
        private static void AssignHUDElement(HUD newHud, GameObject panel, string transform)
        {
            if (!newHud.transform.Find(transform))
                return;
            Transform parent = newHud.transform.Find(transform).transform;
            UnityEngine.Object.Instantiate(panel, parent).SetActive(true);
        }


        private void GetGapBetweenPanels(RectTransform leftPanel, RectTransform rightPanel)
        {
            Vector2 panel2UpperLeftCorner = new Vector2((rightPanel.anchorMax.x - rightPanel.rect.width), (rightPanel.anchorMax.y - rightPanel.rect.height));
            Vector2.Distance(leftPanel.anchorMax, panel2UpperLeftCorner);
        }

        private static IEnumerator AwaitForHUDCreationAndPatch(CameraRigController camera) //Me getting trolled by one single line of code
        {
            yield return new WaitForEndOfFrame();
            if (SceneCatalog.mostRecentSceneDef.baseSceneName == "lobby")
                yield break;
            if (camera.hud == null && !SceneCatalog.mostRecentSceneDef.isOfflineScene)
                TTLog.LogW("Something went wrong when awaiting for the Camera's HUD creation on a Non-Offline Scene.");
            else if (!SceneCatalog.mostRecentSceneDef.isOfflineScene)
                PatchHUD(camera.hud);
        }

        private static IEnumerator AwaitForHUDCreationAndAppend(CameraRigController camera, GameObject objectToInstantiate = null, string parent = null) //Me getting trolled by one single line of code
        {
            yield return new WaitForEndOfFrame();
            if (SceneCatalog.mostRecentSceneDef.baseSceneName == "lobby")
                yield break;
            if (camera.hud == null && !SceneCatalog.mostRecentSceneDef.isOfflineScene)
                TTLog.LogW("Something went wrong when awaiting for the Camera's HUD creation on a Non-Offline Scene.");
            else if (!SceneCatalog.mostRecentSceneDef.isOfflineScene)
                AssignHUDElement(camera.hud, objectToInstantiate, parent);
        }
    }
}