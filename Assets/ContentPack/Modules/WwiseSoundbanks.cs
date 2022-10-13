using RoR2;
using UnityEngine;

namespace TreasureTrove
{
    public static class WwiseSoundbanks
    {
        public static string soundBankDirectory => System.IO.Path.Combine(Assets.assemblyDir, "soundbanks");

        //[SystemInitializer]
        private static void Init()
        {
            uint akBankID;  // Not used. These banks can be unloaded with their file name.
            AkSoundEngine.AddBasePath(soundBankDirectory);
            AkSoundEngine.LoadBank("TroveInit", out akBankID);
            AkSoundEngine.LoadBank("TroveBank", out akBankID);

            //Music
            AkBankManager.LoadBank("TroveMusicBank", false, false);
        }
    }
}