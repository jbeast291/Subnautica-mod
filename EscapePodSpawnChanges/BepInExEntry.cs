using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using LifePodRemastered.Compatability;
using LifePodRemastered.Monos;
using LifePodRemastered.presetSystem;
using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options;
using Nautilus.Options.Attributes;
using Nautilus.Utility.ModMessages;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UserStorageXSave;

namespace LifePodRemastered;

[BepInDependency("Jbeast291.InterpolationFix", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("com.prototech.prototypesub", BepInDependency.DependencyFlags.SoftDependency)]
[BepInPlugin(myGUID, pluginName, pluginVersion)]
public class BepInExEntry : BaseUnityPlugin
{
    private const string myGUID = "Jbeast291.LifePodRemastered";
    private const string pluginName = "Life Pod Remastered";
    private const string pluginVersion = "2.0.0";

    internal static readonly Harmony harmony = new Harmony(myGUID);

    internal static new ManualLogSource Logger;

    internal static MyModOptions modOptions;
    public void Awake()
    {
        ModMessageSystem.SendGlobal("FindMyUpdates", "https://raw.githubusercontent.com/jbeast291/Subnautica-mod/refs/heads/main/EscapePodSpawnChanges/FMU.json");
        Logger = base.Logger;
        Logger.LogInfo(pluginName + " " + pluginVersion + " " + "has been loaded!");
        harmony.PatchAll();

        SaveUtils.CreateDefaultConfigIfModFolderCacheDoesNotExist();
        SaveUtils.ReadSettingsFromModFolder();
        LPRGlobals.resetInfo();

        LanguageHandler.RegisterLocalizationFolder();

        modOptions = new MyModOptions();
        OptionsPanelHandler.RegisterModOptions(modOptions);

        bool loaded = Chainloader.PluginInfos.ContainsKey("com.prototech.prototypesub");
        if (loaded)
        {
            PrototypeExpansionPatches.loadCompatabilityPatches();
        }
    }
    public static bool isGUIDLoaded(string modGUID)
    {
        return Chainloader.PluginInfos.ContainsKey(modGUID);
    }
}

public class MyModOptions : ModOptions
{
    public MyModOptions() : base("Life Pod Remastered")
    {

        var VerticalMotionRateSlider = ModSliderOption.Create(id: "VerticalMotionRate", label: Language.main.Get("LPR.VerticalMotionRate"), minValue: 0f, maxValue: 50f, value: SaveUtils.settingsCache.VertialMotionRate, defaultValue: 10f, tooltip: Language.main.Get("LPR.VerticalMotionRateToolTip"));
        VerticalMotionRateSlider.OnChanged += VerticalMotionRateSlider_OnChanged;
        AddItem(VerticalMotionRateSlider);

        var ToggleHeavyPodButton = ModButtonOption.Create(id: "ToggleHeavyPod", label: Language.main.Get("LPR.ToggleHeavyPodButtonInGame"), onPressed: new Action<ButtonClickedEventArgs>(ToggleHeavyPodButton_OnChanged), tooltip: Language.main.Get("LPR.HeavyPodDescription"));
        AddItem(ToggleHeavyPodButton);
    }
    private void VerticalMotionRateSlider_OnChanged(object sender, SliderChangedEventArgs e)
    {
        SaveUtils.settingsCache.VertialMotionRate = e.Value;
        SaveUtils.WriteSettingsToModFolder();//might be a bad idea to do this every time its update but idk man, havnt tested its performance
    }
    private void ToggleHeavyPodButton_OnChanged(ButtonClickedEventArgs e)
    {
        if (HeavyPodMono.main != null)
        {
            HeavyPodMono.main.ToggleHeavyPod(false);
        }
    }
}