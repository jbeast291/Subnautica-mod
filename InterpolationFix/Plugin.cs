using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using InterpolationFix.Mono;
using Nautilus.Handlers;
using Nautilus.Utility.ModMessages;

namespace InterpolationFix;

[BepInPlugin(MyGuid, PluginName, PluginVersion)]
[BepInDependency("com.snmodding.nautilus")]
public class Plugin : BaseUnityPlugin
{
    private const string MyGuid = "Jbeast291.InterpolationFix";
    private const string PluginName = "Interpolation Fix";
    private const string PluginVersion = "1.0.1";

    private static readonly Harmony Harmony = new Harmony(MyGuid);

    internal new static ManualLogSource Logger;
    
    private void Awake()
    {
        Logger = base.Logger;
        
        ModMessageSystem.SendGlobal("FindMyUpdates", "https://raw.githubusercontent.com/jbeast291/Subnautica-mod/refs/heads/main/InterpolationFix/FMU.json");
        
        Logger.LogInfo(PluginName + " " + PluginVersion + " " + "has been loaded!");
        Harmony.PatchAll();
        Logger.LogInfo("Patched Methods!");
        
        WaitScreenHandler.RegisterLateLoadTask(PluginName, AttachInterpolationManager, "SceneLoadListener");
    }

    private void AttachInterpolationManager(WaitScreenHandler.WaitScreenTask waitScreenTask)
    {
        waitScreenTask.Status = "Adding Interpolation Manager...";
        Logger.LogInfo("Adding Interpolation Manager...");
        Player.main.gameObject.EnsureComponent<PlayerInterpolationManager>();
    }
}