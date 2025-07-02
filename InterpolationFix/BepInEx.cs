using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace InterpolationFix
{
    [BepInPlugin(myGUID, pluginName, pluginVersion)]
    public class BepInEx : BaseUnityPlugin
    {
        private const string myGUID = "Jbeast291.InterpolationFix";
        private const string pluginName = "Interpolaion Fix";
        private const string pluginVersion = "1.0.0";

        private static readonly Harmony harmony = new Harmony(myGUID);

        internal static new ManualLogSource Logger;
        public void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo(pluginName + " " + pluginVersion + " " + "has been loaded!");
            Logger.LogInfo("Patching...");
            harmony.PatchAll();
        }
    }
}