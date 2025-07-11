using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifePodRemastered.patches;

internal class DisplayManagerPatch
{
    [HarmonyPatch(typeof(DisplayManager))]
    internal class OnResolutionChangedPatch
    {
        [HarmonyPatch(nameof(DisplayManager.Update))]
        [HarmonyPostfix]
        public static void OnResolutionChangedPostFix(DisplayManager __instance)
        {
            Info.currentRes = __instance.resolution; // sets the resolution in Info to what is currently set
        }
    }
}
