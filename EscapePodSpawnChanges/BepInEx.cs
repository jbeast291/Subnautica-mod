using HarmonyLib;
using BepInEx;
using BepInEx.Logging;
using UnityEngine.UI;

namespace LifePodRemastered
{
    [BepInPlugin(myGUID, pluginName, pluginVersion)]
    public class BepInEx : BaseUnityPlugin
    {
        private const string myGUID = "Jbeast291.LifePodRemastered";
        private const string pluginName = "Life Pod Remastered";
        private const string pluginVersion = "1.0.0";

        private static readonly Harmony harmony = new Harmony(myGUID);

        internal static new ManualLogSource Logger;

        public void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo(pluginName + " " + pluginVersion + " " + "has been loaded!");
            harmony.PatchAll();
        }
    }
}
