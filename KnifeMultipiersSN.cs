using HarmonyLib;
using Logger = QModManager.Utility.Logger;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using System.Reflection;
using QModManager.API.ModLoading;

namespace KnifeMultipiersSN
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

        [Toggle("Show Debug Logs Ingame"), OnChange(nameof(DebugNotification))]
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

        class KnifeMultipiers
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
                    float newDamage = Config.KnifeDamageSlider;
                    float NewDist = Config.CorrectDistSlider;

                    // Check to see if this is the knife
                    if (__instance.GetType() == typeof(Knife))
                    {
                        Knife knife = __instance as Knife;
                        float knifeDamage = knife.damage;
                        float KnifeDist = knife.attackDist;
                        if (Config.ToggleKnife)
                        {
                            if (knifeDamage != Config.KnifeDamageSlider)
                            {
                                knife.damage = newDamage; // Change the knife damage to the value set in the config
                                if (Config.showDebugLogs)
                                    Logger.Log(Logger.Level.Debug, $"Knife damage was: {knifeDamage}," + $" is now: {newDamage}", null, true);
                            }

                            if (KnifeDist != Config.CorrectDistSlider)
                            {
                                knife.attackDist = NewDist; // Change the knife damage to the value set in the config
                                if (Config.showDebugLogs)
                                    Logger.Log(Logger.Level.Debug, $"Knife attack distance was: {KnifeDist}," + $" is now: {NewDist}", null, true);
                            }
                        }
                        if(!Config.ToggleKnife && (knife.damage != 20) && (knife.attackDist != 2))//Reset Knife Values if de-togged in options menue 
                        {
                            knife.damage = 20;
                            knife.attackDist = 2;
                            if (Config.showDebugLogs)
                            {
                                Logger.Log(Logger.Level.Debug, $"Knife Multipiers has been reset. Damage: {knife.damage}. Dist: {knife.attackDist}", null, true);
                            }
                        }
                    }
                    // Check to see if this is the HeatBlade
                    if (__instance.GetType() == typeof(HeatBlade))
                    {
                        HeatBlade heatBlade = __instance as HeatBlade;
                        float heatBladeDamage = heatBlade.damage;
                        float heatBladeDist = heatBlade.attackDist;
                        if (Config.ToggleHeatBlade)
                        {
                            if (heatBladeDamage != Config.KnifeDamageSlider)
                            {
                                heatBlade.damage = newDamage; // Change the HeatBlade damage to the value set in the config
                                if (Config.showDebugLogs)
                                    Logger.Log(Logger.Level.Debug, $"HeatBlade damage was: {heatBladeDamage}," + $" is now: {newDamage}", null, true);
                            }

                            if (heatBladeDist != Config.CorrectDistSlider)
                            {
                                heatBlade.attackDist = NewDist; // Change the HeatBlade damage to the value set in the config
                                if (Config.showDebugLogs)
                                    Logger.Log(Logger.Level.Debug, $"HeatBlade attack distance was: {heatBladeDist}," + $" is now: {NewDist}", null, true);
                            }
                        }
                        if (!Config.ToggleHeatBlade && (heatBlade.damage != 40) && (heatBlade.attackDist != 2))//Reset HeatBlade Values if de-togged in options menue 
                        {
                            heatBlade.damage = 40;
                            heatBlade.attackDist = 2;
                            if (Config.showDebugLogs)
                            {
                                Logger.Log(Logger.Level.Debug, $"HeatBlade Multipiers has been reset. Damage: {heatBlade.damage}. Dist: {heatBlade.attackDist}", null, true);
                            }
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
