using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Options.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace DayCounterChip
{
    [BepInPlugin(myGUID, pluginName, pluginVersion)]
    [BepInDependency("com.snmodding.nautilus")]
    public class BepInEx : BaseUnityPlugin
    {
        private const string myGUID = "Jbeast291.DayCounterChip";
        private const string pluginName = "Day Counter Chip";
        private const string pluginVersion = "1.2.0";

        private static readonly Harmony harmony = new Harmony(myGUID);

        internal static new ManualLogSource Logger;

        internal static MyConfig myConfig { get; } = OptionsPanelHandler.RegisterModOptions<MyConfig>();

        public void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo(pluginName + " " + pluginVersion + " " + "has been loaded!");
            harmony.PatchAll();
            DayCounterItem.Register();

        }

    }


    [Menu("Day Counter Chip")]
    public class MyConfig : ConfigFile
    {
        [Slider("Counter X Position on screen", -942f, 942f, DefaultValue = 783, Tooltip = "Move the counter around your screen on the X-axis (unavailable in PDA mode)"), OnChange(nameof(PosUpdate))]
        public float PosX = 783;

        [Slider("Counter Y Position on screen", -540f, 540f, DefaultValue = 479, Tooltip = "Move the counter around your screen on the Y-axis (unavailable in PDA mode)"), OnChange(nameof(PosUpdate))]
        public float PosY = 479;

        [Slider("Counter Scale", 0.1f, 2f, DefaultValue = 0.65f, Tooltip = "Scale up/down the counter (unavailable in PDA mode)"), OnChange(nameof(ScaleUpdate))]
        public float Scale = 0.65f;

        //Deprecated for not being very usefull
        //[Choice("Text Color", new[] { "Blue", "Red", "White", "Green", "Black", "Cyan", "Gray", "Magenta", "Yellow" }, Tooltip = "changes text color")]
        //public string ColorChoice = "White";

        [Choice("BackGround Style", new[] { "BackGround 1", "BackGround 2", "Pda Style", "No BackGround" }, Tooltip = "Changes the style of the image behind the text"), OnChange(nameof(BackgroundUpdate))]
        public string BackGroundChoice = "BackGround 2";

        [Toggle("PDA/VR Mode", Tooltip = "(Recommended if using VR) Moves the Day Counter into the PDA so it can be less intrusive/seen in VR"), OnChange(nameof(PdaModeUpdate))]
        public bool PdaMode = false;

        public void ScaleUpdate()
        {
            if (!SceneManager.GetSceneByName("StartScreen").IsValid())
                DayCounterChipFuntion.UpdateScale();
        }

        void PdaModeUpdate()
        {
            if (!SceneManager.GetSceneByName("StartScreen").IsValid())
                DayCounterChipFuntion.UpdateMode();
        }

        void BackgroundUpdate()
        {
            if (!SceneManager.GetSceneByName("StartScreen").IsValid())
                DayCounterChipFuntion.UpdateImages();
        }
        void PosUpdate()
        {
            if (!SceneManager.GetSceneByName("StartScreen").IsValid())
                DayCounterChipFuntion.UpdatePosition();
        }
    }
}
