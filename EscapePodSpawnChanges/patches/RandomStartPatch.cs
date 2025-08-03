using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LifePodRemastered.patches;

internal class RandomStartPatch
{
    /**
     * Patch the lifepods spawn location to be whatever is set
     */
    [HarmonyPatch(typeof(RandomStart))]
    internal class PatchGetRandomStartPoint
    {
        [HarmonyPatch(typeof(RandomStart), "GetRandomStartPoint")]
        [HarmonyPrefix]
        public static bool NewGetRandomStartPoint(ref Vector3 __result)
        {
            float y = 2000f;
            if (!SaveUtils.settingsCache.customIntroToggle)
            {
                y = LPRGlobals.SelectedSpawn.y;
            }
            __result = new Vector3(LPRGlobals.SelectedSpawn.x, y, LPRGlobals.SelectedSpawn.z);
            return false;
        }
    }
}
