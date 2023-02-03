using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace KnifeMultipliersSN
{
    [BepInPlugin(myGUID, pluginName, pluginVersion)]
    public class KnifeMultipliers : BaseUnityPlugin
    {
        private const string myGUID = "Jbeast291.KnifeMultipliers";
        private const string pluginName = "Knife Multipliers";
        private const string pluginVersion = "1.0.0";

        private static readonly Harmony harmony = new Harmony(myGUID);

        internal static new ManualLogSource Logger;

        public void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo(pluginName + " " + pluginVersion + " " + "has been loaded (awesome!)");
            harmony.PatchAll();
        }
    }
}
