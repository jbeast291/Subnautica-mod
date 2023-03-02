using HarmonyLib;
using BepInEx;
using BepInEx.Logging;
using UnityEngine.UI;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Json;

namespace LifePodRemastered
{
    [BepInPlugin(myGUID, pluginName, pluginVersion)]
    public class LifePodRemastered : BaseUnityPlugin
    {
        private const string myGUID = "Jbeast291.LifePodRemastered";
        private const string pluginName = "Life Pod Remastered";
        private const string pluginVersion = "1.0.0";

        private static readonly Harmony harmony = new Harmony(myGUID);

        internal static new ManualLogSource Logger;

        internal static MyConfig Config { get; } = OptionsPanelHandler.RegisterModOptions<MyConfig>();
        public void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo(pluginName + " " + pluginVersion + " " + "has been loaded (awesome!)");
            harmony.PatchAll();
        }
    }
        [Menu("Life Pod Remastered")]
        public class MyConfig : ConfigFile
        {
            [Toggle("Heavy Pod", Tooltip = "Escape Pod will sink in the water")]
            public bool ToggleHeavyPod = true;
            [Slider("Heavy Pod Intensity", 1f, 300, DefaultValue = 30f, Tooltip = "How fast the pod sinks in the water")]
            public float HeavyPodIntensity = 30f;
            [Toggle("Air Spawn", Tooltip = "Spawn the pod high in the air")]
            public bool ToggleAirSpawn = true;
            [Slider("Air Spawn Height", 300, 500, DefaultValue = 500, Tooltip = "How high the pod will spawn")]
            public float AirSpawnHeight = 500f;
            [Toggle("Disable First Time Animations", Tooltip = "Disable the Cinematic Animations when you first leave the pod")]
            public bool DisableFirstTimeAnims = false;
            [Toggle("Skip Intro Automatically", Tooltip = "Skip The into to the game automatically")]
            public bool SkipInto = false;
            [Toggle("Show Text On Map", Tooltip = "Show text on the Map or not")]
            public bool ShowTextOnMap = true;
        }
}
