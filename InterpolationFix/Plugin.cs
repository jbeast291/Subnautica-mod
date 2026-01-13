using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Utility.ModMessages;

namespace InterpolationFix;

[BepInPlugin(MyGuid, PluginName, PluginVersion)]
public class Plugin : BaseUnityPlugin
{
    private const string MyGuid = "Jbeast291.InterpolationFix";
    private const string PluginName = "Interpolation Fix";
    private const string PluginVersion = "1.0.1";

    private static readonly Harmony Harmony = new Harmony(MyGuid);

    internal static new ManualLogSource Logger;
    public void Awake()
    {
        Logger = base.Logger;
        
        ModMessageSystem.SendGlobal("FindMyUpdates", "https://raw.githubusercontent.com/jbeast291/Subnautica-mod/refs/heads/main/InterpolationFix/FMU.json");
        
        Logger.LogInfo(PluginName + " " + PluginVersion + " " + "has been loaded!");
        Logger.LogInfo("Patching...");
        Harmony.PatchAll();
        Logger.LogInfo("Patched!");
    }
}