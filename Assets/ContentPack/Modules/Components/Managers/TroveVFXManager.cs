using RoR2;
using System.Linq;
using UnityEngine;
using TemporaryVFX = TreasureTrove.TempVFX.TemporaryVFX;

namespace TreasureTrove
{
    internal class TroveVFXManager : MonoBehaviour
    {
        private TemporaryVisualEffect[] tempVisualEffects = new TemporaryVisualEffect[InitVFX.temporaryVfx.Count];
        private CharacterBody body;
        private CharacterModel model;

        private void Awake()
        {
            body = gameObject.GetComponent<CharacterBody>();
            model = gameObject.GetComponent<ModelLocator>().modelTransform.gameObject.GetComponent<CharacterModel>();
            //tempVisualEffects = new TemporaryVisualEffect[InitVFX.temporaryVfx.Count];
            /*TemporaryVFX[] tempVfx = InitVFX.temporaryVfx.Keys.ToArray();
            foreach (var item in tempVfx)
            {
                HG.ArrayUtils.ArrayAppend(ref tempVisualEffects, item.tempVfxRootGO.GetComponent<TemporaryVisualEffect>());
            }*/
        }

        private void OnEnable()
        {
            InstanceTracker.Add<TroveVFXManager>(this);
        }

        private void Update()
        {
            if (body)
            {
                UpdateAllTemporaryVFX();
            }
        }

        private void OnDisable()
        {
            InstanceTracker.Remove<TroveVFXManager>(this);
        }

        public void UpdateForCamera(CameraRigController cameraRigController)
        {
            UpdateOverlays();
        }

        //Updates all overlays, meant to be used with AddOverlay() or to add Temporary Overlays
        private void UpdateOverlays()
        {
        }

        //Updates all VFX, check each class for the attributes
        private void UpdateAllTemporaryVFX()
        {
            for (int i = 0; i < tempVisualEffects.Length; i++)
            {
                TemporaryVFX vFX = InitVFX.temporaryVfx.Keys.ElementAt(i);
                TTLog.LogW("Updating " + vFX);
                UpdateSingleTemporaryVisualEffect(ref tempVisualEffects[i], InitVFX.temporaryVfx.Values.ElementAt(i), vFX.GetEffectRadius(ref body), vFX.IsEnabled(ref body), vFX.GetChildOverride(ref body));
                TTLog.LogW("Updated " + tempVisualEffects[i] + " " + InitVFX.temporaryVfx.Values.ElementAt(i) + " " + vFX.GetEffectRadius(ref body) + " " + vFX.IsEnabled(ref body) + " " + vFX.GetChildOverride(ref body) + " ");
            }
        }

        //Simple material overlay adder. Must have the same update rate as CharacterModel's UpdateOverlay
        private void AddOverlay(Material overlayMaterial, bool active)
        {
            if (model.activeOverlayCount >= CharacterModel.maxOverlays || !overlayMaterial)
                return;
            if (active)
            {
                Material[] array = model.currentOverlays;
                int num = model.activeOverlayCount++;
                model.activeOverlayCount = num + 1;
                array[num] = overlayMaterial;
            }
        }

        //Temporary VFX Updater, gets the state of the VFX (That's why is passed by ref + stored per component) and if it has to be active, instantiates the gameobject prefab.
        private void UpdateSingleTemporaryVisualEffect(ref TemporaryVisualEffect tempEffect, GameObject prefab, float effectRadius, bool active, string childLocatorOverride = "")
        {
            TTLog.LogW(tempEffect + " = tempEffect; " + prefab + " = Prefab; " + effectRadius + " = Radius; " + active + " = active; " + childLocatorOverride);
            bool flag = tempEffect != null;
            if (flag != active)
            {
                TTLog.LogW("Passed check 1");
                if (active)
                {
                    TTLog.LogW("Passed check 2");
                    if (!flag)
                    {
                        TTLog.LogW("Passed check 3");
                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(prefab, body.corePosition, Quaternion.identity);
                        tempEffect = gameObject.GetComponent<TemporaryVisualEffect>();
                        tempEffect.parentTransform = body.coreTransform;
                        tempEffect.visualState = TemporaryVisualEffect.VisualState.Enter;
                        tempEffect.healthComponent = body.healthComponent;
                        tempEffect.radius = effectRadius;
                        LocalCameraEffect component = gameObject.GetComponent<LocalCameraEffect>();
                        if (component)
                        {
                            component.targetCharacter = base.gameObject;
                        }
                        if (!string.IsNullOrEmpty(childLocatorOverride))
                        {
                            ModelLocator modelLocator = body.modelLocator;
                            ChildLocator childLocator;
                            if (modelLocator == null)
                            {
                                childLocator = null;
                            }
                            else
                            {
                                Transform modelTransform = modelLocator.modelTransform;
                                childLocator = ((modelTransform != null) ? modelTransform.GetComponent<ChildLocator>() : null);
                            }
                            ChildLocator childLocator2 = childLocator;
                            if (childLocator2)
                            {
                                Transform transform = childLocator2.FindChild(childLocatorOverride);
                                if (transform)
                                {
                                    tempEffect.parentTransform = transform;
                                    return;
                                }
                            }
                        }
                    }
                }
                else if (tempEffect)
                {
                    tempEffect.visualState = TemporaryVisualEffect.VisualState.Exit;
                }
            }
        }
    }
}