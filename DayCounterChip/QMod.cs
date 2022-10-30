using HarmonyLib;
using Logger = QModManager.Utility.Logger;
using QModManager.API.ModLoading;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using System.Reflection;
using UnityEngine.UI;

namespace DayCounterChip
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

            DayCounterChip dayCounterChip = new DayCounterChip();
            dayCounterChip.Patch();

            Config = OptionsPanelHandler.Main.RegisterModOptions<MyConfig>();
            Logger.Log(Logger.Level.Info, "Patched successfully!", null, true);

        }
    }
    [Menu("Day Counter Chip")]
    public class MyConfig : ConfigFile
    {
        [Slider("Counter Position on screen X", -2.8f, 2.2f, DefaultValue = 1.7783f)]
        public float PosX = 1.7783f;

        [Slider("Counter Position on screen Y", -1.4f, 1.4f, DefaultValue = 1.2482f)]
        public float PosY = 1.2482f;

        [Choice("Text Color", new[] { "Blue", "Red", "White", "Green", "Black", "Cyan", "Gray", "Magenta", "Yellow" }, Tooltip = "changes text color")]
        public string ColorChoice = "White";

        [Choice("BackGround Style", new[] { "BackGround 1", "BackGround 2", "No BackGround" }, Tooltip = "Changes the style of the image behind the text")]
        public string BackGroundChoice = "BackGround 2";
    }
}
