using HarmonyLib;
using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LifePodRemastered.patches
{
    internal class PdaMutePatch
    {
        [HarmonyPatch(typeof(SoundQueue))]
        internal class PatchGetRandomStartPoint
        {
            [HarmonyPatch(typeof(SoundQueue), "Play")]
            [HarmonyPrefix]
            public static bool SoundQueuePreFix()
            {
                if (Info.mutePdaEvents)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
