using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Json;


namespace DayCounterChip
{
    [BepInPlugin(myGUID, pluginName, pluginVersion)]
    public class BepInEx : BaseUnityPlugin
    {
        private const string myGUID = "Jbeast291.DayCounterChip";
        private const string pluginName = "Day Counter Chip";
        private const string pluginVersion = "1.0.0";

        private static readonly Harmony harmony = new Harmony(myGUID);

        internal static new ManualLogSource Logger;

        internal static MyConfig Config { get; } = OptionsPanelHandler.RegisterModOptions<MyConfig>();

        public void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo(pluginName + " " + pluginVersion + " " + "has been loaded (awesome!)");
            harmony.PatchAll();
            DayCounterChip dayCounterChip = new DayCounterChip();
            dayCounterChip.Patch();

        }

    }

    [Menu("Day Counter Chip")]
    public class MyConfig : ConfigFile
    {
        [Slider("Counter Position on screen X", -2.8f, 2.2f, DefaultValue = 1.7783f)]
        public float PosX = 1.7783f;

        [Slider("Counter Position on screen Y", -1.4f, 1.4f, DefaultValue = 1.2482f)]
        public float PosY = 1.2482f;

        //[Choice("Text Color", new[] { "Blue", "Red", "White", "Green", "Black", "Cyan", "Gray", "Magenta", "Yellow" }, Tooltip = "changes text color")]
        //public string ColorChoice = "White";

        [Choice("BackGround Style", new[] { "BackGround 1", "BackGround 2", "No BackGround" }, Tooltip = "Changes the style of the image behind the text")]
        public string BackGroundChoice = "BackGround 2";
    }
}
