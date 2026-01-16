using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using InterpolationFix.Mono;
using Nautilus.Handlers;
using Nautilus.Utility.ModMessages;

namespace InterpolationFix;

[BepInPlugin(MY_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus")]
public class Plugin : BaseUnityPlugin
{
    private const string MY_GUID = "Jbeast291.InterpolationFix";
    private const string PLUGIN_NAME = "Interpolation Fix";
    private const string PLUGIN_VERSION = "1.1.0";

    private static readonly Harmony Harmony = new Harmony(MY_GUID);

    internal new static ManualLogSource Logger;
    
    private void Awake()
    {
        Logger = base.Logger;
        
        ModMessageSystem.SendGlobal("FindMyUpdates", "https://raw.githubusercontent.com/jbeast291/Subnautica-mod/refs/heads/main/InterpolationFix/FMU.json");
        
        Logger.LogInfo(PLUGIN_NAME + " " + PLUGIN_VERSION + " " + "has been loaded!");
        Harmony.PatchAll();
        Logger.LogInfo("Patched Methods!");
        
        WaitScreenHandler.RegisterLateLoadTask(PLUGIN_NAME, AttachInterpolationManager, "SceneLoadListener");
    }

    private void AttachInterpolationManager(WaitScreenHandler.WaitScreenTask waitScreenTask)
    {
        waitScreenTask.Status = "Adding Interpolation Manager...";
        Logger.LogInfo("Adding Interpolation Manager...");
        Player.main.gameObject.EnsureComponent<PlayerInterpolationManager>();
    }
}