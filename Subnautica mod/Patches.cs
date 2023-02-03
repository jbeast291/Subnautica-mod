using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace KnifeMultipliersSN
{
    internal class Patches
    {
        [HarmonyPatch(typeof(PlayerTool))]
        [HarmonyPatch("OnToolActionStart")]
        internal class PatchPlayerOnToolActionStart
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerTool __instance)
            {
                if (__instance.GetType() == typeof(Knife))
                {
                    Knife knife = __instance as Knife;
                    KnifeMultipliers.Logger.LogInfo("Knife Used");
                    knife.damage = 25;
                    knife.attackDist = 2;
                }
                if (__instance.GetType() == typeof(HeatBlade))
                {
                    HeatBlade heatblade = __instance as HeatBlade;
                    KnifeMultipliers.Logger.LogInfo("HeatBlade Used");
                    heatblade.damage = 50f;
                    heatblade.attackDist = 2f;
                }
            }

        }
    }
}
