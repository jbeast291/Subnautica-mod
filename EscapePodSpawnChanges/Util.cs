using FMOD;
using Nautilus.Handlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LifePodRemastered
{
    internal class Util
    {
        public static FMODAsset GetFmodAsset(string audioPath)
        {
            FMODAsset asset = ScriptableObject.CreateInstance<FMODAsset>();
            asset.path = audioPath;
            return asset;
        }
        IEnumerator AudioLooper(//Must have already initialized the audio with fmod before use
        string StartAudioName, float StartAudioDuration,
        string CoreAudioName, float CoreAudioDuration,
        string EndAudioName, int LoopCount, float volumeMulti) {
            //Play the fade in audio for the loop
            if (CustomSoundHandler.TryPlayCustomSound(StartAudioName, out Channel StartChannel))
            {
                StartChannel.setVolume(1f * volumeMulti);
            }
            yield return new WaitForSeconds(StartAudioDuration);
            while (LoopCount > 0)
            {
                if (CustomSoundHandler.TryPlayCustomSound(CoreAudioName, out Channel CoreChannel))
                {
                    CoreChannel.setVolume(1f * volumeMulti);
                }
                LoopCount--;
                yield return new WaitForSeconds(CoreAudioDuration);
            }
            if (CustomSoundHandler.TryPlayCustomSound(EndAudioName, out Channel EndChannel))
            {
                EndChannel.setVolume(1f * volumeMulti);
            }
            //start looping the main segment

        }

        public static List<string[]> ReadPresetsFromModFolder()
        {
            string raw =  File.ReadAllText(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Presets"), "presets.txt"));
            List<string[]> presetlist = new List<string[]>();
            foreach (string pair in raw.Split(new char[] { '\n' }))
            {
                string[] splitPair = pair.Split(new char[] { ':' });
                //remove comments from parsing and blankimproperformated lines
                if (splitPair.Length != 2 || (splitPair[0].Length >= 2 && (splitPair[0].Substring(0, 2) == "//")))
                {
                    continue;
                }
                presetlist.Add(splitPair);
            }
            return presetlist;
            
        }
    }
}
