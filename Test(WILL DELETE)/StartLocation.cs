using System.Reflection;
using HarmonyLib;
using UnityEngine; //You need UnityEngine here when using Vector3
using QModManager.API.ModLoading;
using Logger = QModManager.Utility.Logger;

namespace StartLocation
{
    [QModCore]
    public static class QMod
    {
        [QModPatch]
        public static void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var modName = ($"craig_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {modName}");
            Harmony harmony = new Harmony(modName);
            harmony.PatchAll(assembly);
            Logger.Log(Logger.Level.Info, "Patched successfully!");
        }
    }
    [HarmonyPatch(typeof(RandomStart))]
    internal class PatchGetRandomStartPoint
    {
        [HarmonyPatch(typeof(RandomStart), "GetRandomStartPoint")]
        [HarmonyPrefix]
        public static bool NewGetRandomStartPoint(ref Vector3 __result)
        {
            __result = new Vector3(45, 0, 0);// you need to specify a new object here when using C#
            return false;// When using a harmony prefix if you have the function be a bool and return false it skips executing the original function and returns __result to whatever called it. Basically it overides the original function and returns your variable.
            /* If you want to learn more about prefixes and Harmony here are some usefull links
            https://raw.githubusercontent.com/pardeike/Harmony/master/Harmony/Documentation/images/patch-logic.svg?sanitize=true
            https://harmony.pardeike.net/articles/intro.html
            */
        }
    }
}