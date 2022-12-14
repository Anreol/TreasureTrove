using AK.Wwise;
using RoR2;
using UnityEngine;

namespace TreasureTrove.ScriptableObjects
{
    [CreateAssetMenu(menuName = "TreasureTrove/MusicTrackSafeDef")]
    public class MusicTrackSafeDef : MusicTrackDef
    {
        public override void Preload()
        {
            AK.Wwise.Bank bank = this.soundBank;
            if (bank == null)
            {
                TTLog.LogE($"Failed to load soundbank! Tried to load: {soundBank} in {this.cachedName}", true);
                return;
            }

            bank.Load(false, false);
            if (TreasureTroveUnityPlugin.playMusicSystemID == 0U)
            {
                TreasureTroveUnityPlugin.playMusicSystemID = AkSoundEngine.PostEvent("Play_Music_System_TreasureTrove", TreasureTroveUnityPlugin.instance.gameObject); //MUST be done, else the music won't play
            }
            //TELog.LogI($"Got a PostEvent id of {id} after trying to play Play_Music_System_TurboEditon {this.cachedName}", true);
        }

        public override void Play()
        {
            this.Preload();
            foreach (State state in this.states)
            {
                AKRESULT result = AkSoundEngine.SetState(state.GroupId, state.Id);
                if (result > AKRESULT.AK_Success)
                {
                    TTLog.LogE($"ERROR setting state: {state.Name} with State GroupId {state.GroupId} and State Id {state.Id} in {this.cachedName}", true);
                }
                else
                {
                    TTLog.LogI($"Successfully set: {state.Name} with State GroupId {state.GroupId} and State Id {state.Id} in {this.cachedName}", true);
                }
            }
        }
    }
}