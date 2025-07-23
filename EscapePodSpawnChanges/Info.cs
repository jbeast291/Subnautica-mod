using System.Reflection;
using System.IO;
using UnityEngine;
using JetBrains.Annotations;

namespace LifePodRemastered;

public class Info
{
    public static bool showmap = false; // set to true when any of the start buttons are pressed. used to activate custom GUI

    public static AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "lifepodspawnmap")); // the game for some reason complaines if you call load from file in 2 different areas so it has to be here so all scripts can use it easly.
    public static string PathToAudioFolder = Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "Audio");
    public static bool newSave = false; // used to tell weather the save is new or not on startup
    public static bool OverideSpawnHeight = false; // used if manually inputing coords to overide the set spawn height.

    public static bool CinematicActive = false; // set to true when the custom intro is active
    public static bool mutePdaEvents = false; //used to mute the pda durring cinematic intro
    public static Vector3 SelectedSpawn; // the selected spawn of the pod
    public static Resolution currentRes;

    public static void resetInfo()
    {
        showmap = false;
        newSave = false;
        OverideSpawnHeight = false;
        CinematicActive = false;
        mutePdaEvents = false;
    }
}


