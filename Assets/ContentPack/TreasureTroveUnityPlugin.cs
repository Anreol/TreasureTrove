using BepInEx;
using RoR2;
using RoR2.ContentManagement;
using UnityEngine;


namespace TreasureTrove
{
    [BepInPlugin(ModGuid, ModIdentifier, ModVer)]
    public class TreasureTroveUnityPlugin : BaseUnityPlugin
    {
        internal const string ModVer =
#if DEBUG
            "9999." +
#endif
            "0.0.1";

        internal const string ModIdentifier = "TreasureTrove";
        internal const string ModGuid = "com.Anreol." + ModIdentifier;

        public static TreasureTroveUnityPlugin instance;
        public static PluginInfo pluginInfo;
        public static uint playMusicSystemID;
        public void Awake()
        {
            TTLog.logger = Logger;
            TTLog.LogI("Running Turbo Edition for PLAYTESTING!", true);
            TTLog.LogI("Whenever a run ends, a log message will appear with a link to a form for feedback. Fill it if you want!", true);
#if DEBUG
            TTLog.outputAlways = true;
            TTLog.LogW("Running TurboEdition DEBUG build. PANIC!");
#endif
            pluginInfo = Info;
            instance = this;
            ContentManager.collectContentPackProviders += (addContentPackProvider) => addContentPackProvider(new TTContent());
        }
    }
}