using HarmonyLib;
using BepInEx;
using BepInEx.Logging;
using UnityEngine.UI;
using Nautilus.Options.Attributes;
using UnityEngine.SceneManagement;
using Nautilus.Json;
using Nautilus.Options;
using Nautilus.Handlers;
using UnityEngine;
using LifePodRemastered.Monos;
using static UserStorageXSave;
using Nautilus.Utility.ModMessages;

namespace LifePodRemastered;

[BepInDependency("Jbeast291.InterpolationFix", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
[BepInPlugin(myGUID, pluginName, pluginVersion)]
public class BepInEx : BaseUnityPlugin
{
    private const string myGUID = "Jbeast291.LifePodRemastered";
    private const string pluginName = "Life Pod Remastered";
    private const string pluginVersion = "2.0.0";

    private static readonly Harmony harmony = new Harmony(myGUID);

    internal static new ManualLogSource Logger;

    internal static MyConfig MyConfig { get; } = OptionsPanelHandler.RegisterModOptions<MyConfig>();
    public void Awake()
    {
        ModMessageSystem.SendGlobal("FindMyUpdates", "https://raw.githubusercontent.com/RamuneNeptune/SubnauticaMods/refs/heads/main/Source/FindMyUpdates/Version.json");
        Logger = base.Logger;
        Logger.LogInfo(pluginName + " " + pluginVersion + " " + "has been loaded!");
        harmony.PatchAll();

        LanguageHandler.RegisterLocalizationFolder();
    }
}

[Menu("Life Pod Remastered")]
public class MyConfig : ConfigFile
{
    [Button("Toggle Heavy Pod", Tooltip = "WARNING: Will cause camera jitter if you are in/on the pod. The pod will only move when you are within 15m(so it doesnt fall thru the ground)")]
    public void MyButtonClickEvent(ButtonClickedEventArgs e)
    {
        if (HeavyPodMono.main != null)
        {
            SaveUtils.inGameSave.HeavyPodToggle = !SaveUtils.inGameSave.HeavyPodToggle;
            HeavyPodMono.main.SheduleAnimation();
        }
    }

    [Slider("Vertical Motion Rate", 1f, 50f, DefaultValue = 10f, Tooltip = "When toggling the Heavy Pod on/off, this is the rate it will move up/down")]
    public float verticalMotionRate = 10f;
}
