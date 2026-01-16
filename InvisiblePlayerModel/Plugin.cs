using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Utility.ModMessages;

namespace InvisiblePlayerModel;

[BepInPlugin(MY_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus")]
public class Plugin : BaseUnityPlugin
{
    private const string MY_GUID = "Jbeast291.InvisiblePlayerModel";
    private const string PLUGIN_NAME = "Invisible Player Model";
    private const string PLUGIN_VERSION = "0.0.1";
    
    public static InvisiblePlayerModelConfig config { get; } = OptionsPanelHandler.RegisterModOptions<InvisiblePlayerModelConfig>();
    
    private static readonly Harmony Harmony = new Harmony(MY_GUID);
    internal new static ManualLogSource Logger { get; private set; }

    private void Awake()
    {
        Logger = base.Logger;
        
        ModMessageSystem.SendGlobal("FindMyUpdates", "https://raw.githubusercontent.com/jbeast291/Subnautica-mod/refs/heads/main/InterpolationFix/FMU.json");
        
        Harmony.PatchAll();
        Logger.LogInfo($"Plugin {MY_GUID} ({PLUGIN_VERSION}) is loaded!");
        
        WaitScreenHandler.RegisterLateLoadTask(PLUGIN_NAME, AttachInterpolationManager, "SceneLoadListener");
    }
    private void AttachInterpolationManager(WaitScreenHandler.WaitScreenTask waitScreenTask)
    {
        waitScreenTask.Status = "Modifying Player Model...";
        Logger.LogInfo("Modifying Player Model...");
        PlayerModelModifier.UpdatePlayerModel();
    }
}
