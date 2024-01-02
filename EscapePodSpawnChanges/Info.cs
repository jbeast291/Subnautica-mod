using System.Reflection;
using System.IO;
using UnityEngine;

namespace LifePodRemastered
{
    public class Info
    {
        public static bool showmap = false; // set to true when any of the start buttons are pressed. used to activate custom GUI
        public static Vector3 SelectedSpawn; // the selected spawn of the pod
        public static Resolution currentRes; // used for math related to the selected point being set in the right area and scaling with resolution.
        public static bool isrepawning; // used for if the player is respawning and to stop the pod from moving while they are doing that.
        public static AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "lifepodspawnmap")); // the game for some reason complaines if you call load from file in 2 different areas so it has to be here so all scripts can use it easly.
        public static string PathToAudioFolder = Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "Audio");
        public static bool newSave; // used to tell weather the save is new or not on startup
        public static bool Showsettings = false; // wether to show the games settings or not 
        public static int GameMode; // 1 survival, 2 creative, 3 freedom, 4 hardcore
        public static bool OverideSpawnHeight; // used if manually inputing coords to overide the set spawn height.
        public static bool loaded = false;
    }
}

