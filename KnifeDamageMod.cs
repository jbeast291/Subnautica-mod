using HarmonyLib;
using Logger = QModManager.Utility.Logger;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using System.Reflection;
using QModManager.API.ModLoading;

namespace KnifeDamageMod_SN
{
    [Menu("Knife and HeatBlade Multipliers")]
    public class MyConfig : ConfigFile
    {
        public float CorrectDistSlider;

        [Slider("Knife Damage", 1, 400, DefaultValue = 20)]
        public float KnifeDamageSlider = 20; // defualt value of knife damage

        [Slider("Knife Attack Distance", 1, 200, DefaultValue = 2), OnChange(nameof(CorrectDistVal))]
        public float KnifeDistSlider = 2; // defualt value of knife damage

        [Toggle("Appy To Knife")]
        public bool ToggleKnife = true; //Enables the knife to be 

        [Toggle("Appy To Heat Blade")]
        public bool ToggleHeatBlade = true; //Enables the Heat Blade to be changed

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
            internal class PatchPlayerOnToolActionStart
            {
                [HarmonyPostfix]
                public static void Postfix(PlayerTool __instance)
                {
                    Config.Load();

                    // Check to see if this is the knife
                    if (__instance.GetType() == typeof(Knife) && Config.ToggleKnife)
                    {
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
                    // Check to see if this is the HeatBlade
                    if (__instance.GetType() == typeof(HeatBlade) && Config.ToggleHeatBlade)
                    {
                        HeatBlade heatblade = __instance as HeatBlade;
                        float heatBladeDamage = heatblade.damage;
                        float newHeatBladeDamage = Config.KnifeDamageSlider;

                        float HeatBladeDist = heatblade.attackDist;
                        float NewHeatBladeDist = Config.CorrectDistSlider;

                        if (heatBladeDamage != Config.KnifeDamageSlider)
                            heatblade.damage = newHeatBladeDamage; // Change the knife damage to the value set in the config

                        if (heatBladeDamage != Config.CorrectDistSlider)
                            heatblade.attackDist = NewHeatBladeDist; // Change the knife damage to the value set in the config

                        if (Config.showDebugLogs && (heatBladeDamage != Config.KnifeDamageSlider))
                            Logger.Log(Logger.Level.Debug, $"Heat Blade damage was: {heatBladeDamage}," + $" is now: {newHeatBladeDamage}", null, true);

                        if (Config.showDebugLogs && (HeatBladeDist != Config.CorrectDistSlider))
                            Logger.Log(Logger.Level.Debug, $"Heat Blade attack distance was: {HeatBladeDist}," + $" is now: {NewHeatBladeDist}", null, true);
                    }
                    if (__instance.GetType() == typeof(Knife) && !Config.ToggleKnife)
                    {
                        Knife knife = __instance as Knife;
                        knife.damage = 20;
                        knife.attackDist = 2;
                        if(Config.showDebugLogs)
                        {
                            Logger.Log(Logger.Level.Debug, $"Knife Multipiers has been reset", null, true);
                        }
                    }
                    if (__instance.GetType() == typeof(HeatBlade) && !Config.ToggleKnife)
                    {
                        HeatBlade heatblade = __instance as HeatBlade;
                        heatblade.damage = 20;
                        heatblade.attackDist = 2;
                        if (Config.showDebugLogs)
                        {
                            Logger.Log(Logger.Level.Debug, $"HeatBlade Multipiers has been reset", null, true);
                        }
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
