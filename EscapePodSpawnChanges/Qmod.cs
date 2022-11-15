using HarmonyLib;
using Logger = QModManager.Utility.Logger;
using RecipeData = SMLHelper.V2.Crafting.TechData;
using QModManager.API.ModLoading;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LifePodRemastered
{
    [QModCore]
    public static class QMod
    {
        internal static MyConfig Config { get; private set; }

        [QModPatch]
        public static void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var modName = ($"Jbeast291_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {modName}", null, true);
            Harmony harmony = new Harmony(modName);
            harmony.PatchAll(assembly);

            Config = OptionsPanelHandler.Main.RegisterModOptions<MyConfig>();
            Logger.Log(Logger.Level.Info, "Patched successfully!", null, true);

        }
    }
    [Menu("Life Pod Remastered")]
    public class MyConfig : ConfigFile
    {
        [Toggle("Heavy Pod", Tooltip = "Escape Pod will sink in the water")]
        public bool ToggleHeavyPod = true;
        [Slider("Heavy Pod Intensity", 1f, 60, DefaultValue = 30f, Tooltip = "How fast the pod sinks in the water")]
        public float HeavyPodIntensity = 30f;
        [Toggle("Air Spawn", Tooltip = "Spawn the pod high in the air")]
        public bool ToggleAirSpawn = true;
        [Slider("Air Spawn Height", 300, 500, DefaultValue = 500, Tooltip = "How high the pod will spawn")]
        public float AirSpawnHeight = 500f;
        [Toggle("Toggle the Parachute", Tooltip = "Turn the parachute ontop of your pod on or off")]
        public bool ToggleParachute = true;
        [Toggle("Disable First Time Animations", Tooltip = "Disable the Cinematic Animations when you first leave the pod")]
        public bool DisableFirstTimeAnims = false;
        [Toggle("Skip Intro Automatically", Tooltip = "Skip The into to the game automatically")]
        public bool SkipInto = false;
        [Toggle("Show Text On Map", Tooltip = "Show text on the Map or not")]
        public bool ShowTextOnMap = true;
        
    }
}
