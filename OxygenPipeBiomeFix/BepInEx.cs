using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Utility.ModMessages;

namespace OxygenPipeBiomeFix;

[BepInPlugin(myGUID, pluginName, pluginVersion)]
public class BepInEx : BaseUnityPlugin
{
    private const string myGUID = "Jbeast291.OxygenPipeBiomeFix";
    private const string pluginName = "Oxygen Pipe Biome Fix";
    private const string pluginVersion = "1.0.0";

    private static readonly Harmony harmony = new Harmony(myGUID);

    internal static new ManualLogSource Logger;
    public void Awake()
    {
        ModMessageSystem.SendGlobal("FindMyUpdates", "https://raw.githubusercontent.com/jbeast291/Subnautica-mod/refs/heads/main/OxygenPipeBiomeFix/FMU.json");
        Logger = base.Logger;
        Logger.LogInfo(pluginName + " " + pluginVersion + " " + "has been loaded!");
        Logger.LogInfo("Patching AtmosphereVolume.Start()...");
        harmony.PatchAll();
        Logger.LogInfo("Patched!");
    }
}
