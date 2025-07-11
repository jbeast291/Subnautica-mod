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
using static VFXSandSharkDune;

namespace LifePodRemastered;

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
        string EndAudioName, int LoopCount, float volumeMulti)
    {
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
        string raw = File.ReadAllText(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Presets"), "presets.txt"));
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

    public static float clipLength(Animator anim, string name)
    {
        RuntimeAnimatorController ac = anim.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if (ac.animationClips[i].name == name)
            {
                return ac.animationClips[i].length;
            }
        }
        return -1;
    }

    public static Vector3 StringToVector3(string sVector)
    {
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        string[] sArray = sVector.Split(',');

        return new Vector3(float.Parse(sArray[0]), float.Parse(sArray[1]), float.Parse(sArray[2]));
    }
}

