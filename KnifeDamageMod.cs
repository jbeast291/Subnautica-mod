using HarmonyLib;
using Logger = QModManager.Utility.Logger;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using System.Reflection;
using QModManager.API.ModLoading;

namespace KnifeDamageMod_SN
{
    [Menu("Knife Damage Multipliers")]
    public class MyConfig : ConfigFile
    {
        public float CorrectDistSlider;

        [Slider("Knife Damage", 1, 400, DefaultValue = 20)]
        public float KnifeDamageSlider = 20; // defualt value of knife damage

        [Slider("Knife Attack Distance", 1, 100, DefaultValue = 2), OnChange(nameof(CorrectDistVal))]
        public float KnifeDistSlider = 2; // defualt value of knife damage

        [Toggle("Show Debug Logs"), OnChange(nameof(DebugNotification))]
        public bool showDebugLogs = false; //Shows debug logs if true

        public void CorrectDistVal()
        {
            CorrectDistSlider = (KnifeDistSlider * 0.6f);
        }
        public void DebugNotification()
        {
            if (showDebugLogs)
                Logger.Log(Logger.Level.Info, "Show Debug Logs Enabled", null, true);

            if (!showDebugLogs)
                Logger.Log(Logger.Level.Info, "Show Debug Logs Disabled", null, true);
        }

        class KnifeDamageMod
        {
            public static MyConfig Config { get; } = new MyConfig();


            [HarmonyPatch(typeof(PlayerTool))]
            [HarmonyPatch("OnToolActionStart")]
            internal class PatchPlayerToolAwake
            {
                [HarmonyPostfix]
                public static void Postfix(PlayerTool __instance)
                {
                    // Check to see if this is the knife
                    if (__instance.GetType() == typeof(Knife))
                    {
                        Config.Load();
                        Knife knife = __instance as Knife;
                        float knifeDamage = knife.damage;
                        float newKnifeDamage = Config.KnifeDamageSlider;

                        float KnifeDist = knife.attackDist;
                        float NewKnifeDist = Config.CorrectDistSlider;

                        if (knifeDamage != Config.KnifeDamageSlider)
                            knife.damage = newKnifeDamage; // Change the knife damage to the value set in the config

                        if (knifeDamage != Config.CorrectDistSlider)
                            knife.attackDist = NewKnifeDist; // Change the knife damage to the value set in the config

                        if (Config.showDebugLogs && (knifeDamage != Config.KnifeDamageSlider))
                            Logger.Log(Logger.Level.Debug, $"Knife damage was: {knifeDamage}," + $" is now: {newKnifeDamage}", null, true);

                        if (Config.showDebugLogs && (KnifeDist != Config.CorrectDistSlider))
                            Logger.Log(Logger.Level.Debug, $"Knife attack distance was: {KnifeDist}," + $" is now: {NewKnifeDist}", null, true);
                    }
                }
            }
        }

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
    }
}
