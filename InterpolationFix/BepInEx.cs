using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Utility.ModMessages;

namespace InterpolationFix;

[BepInPlugin(myGUID, pluginName, pluginVersion)]
public class BepInEx : BaseUnityPlugin
{
    private const string myGUID = "Jbeast291.InterpolationFix";
    private const string pluginName = "Interpolation Fix";
    private const string pluginVersion = "1.0.1";

    private static readonly Harmony harmony = new Harmony(myGUID);

    internal static new ManualLogSource Logger;
    public void Awake()
    {
        ModMessageSystem.SendGlobal("FindMyUpdates", "https://raw.githubusercontent.com/jbeast291/Subnautica-mod/refs/heads/main/InterpolationFix/FMU.json");
        Logger = base.Logger;
        Logger.LogInfo(pluginName + " " + pluginVersion + " " + "has been loaded!");
        Logger.LogInfo("Patching...");
        harmony.PatchAll();
        Logger.LogInfo("Patched!");
    }
}