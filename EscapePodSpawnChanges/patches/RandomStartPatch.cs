using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LifePodRemastered.patches
{
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
                float y;
                if (!SaveUtils.settingsCache.HeightReqToggle)
                {
                    y = 0f;
                }
                if (!SaveUtils.settingsCache.HeightReqToggle && Info.OverideSpawnHeight)
                {
                    y = Info.SelectedSpawn.y;
                }
                else
                {
                    y = 2000f;
                }
                __result = new Vector3(Info.SelectedSpawn.x, y, Info.SelectedSpawn.z);
                return false;
            }
        }
    }
}
