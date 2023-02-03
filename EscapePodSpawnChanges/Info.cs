using System.Reflection;
using System.IO;
using UnityEngine;

namespace LifePodRemastered
{
    public class Info
    {
        public static bool showmap = false;
        public static Vector3 SelectedSpawn;
        public static Resolution currentRes;
        public static bool isrepawning;
        public static AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "lifepodspawnmap"));
        public static bool newSave;
        public static bool Showsettings = false;
        public static int GameMode; // 1 survival, 2 creative, 3, freedom, 4 hardcore

    }
}
