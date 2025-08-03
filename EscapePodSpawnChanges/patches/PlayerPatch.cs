using HarmonyLib;
using LifePodRemastered.Monos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace LifePodRemastered.patches;

internal class PlayerPatch
{
    [HarmonyPatch(typeof(Player))]
    internal class OnLandPreFixPatch
    {
        [HarmonyPatch(nameof(Player.OnLand))]
        [HarmonyPrefix]
        public static bool OnLandPreFix()
        {
            if (LPRGlobals.CinematicActive)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(nameof(Player.Start))]
        [HarmonyPostfix]
        public static void StartPostFix(Player __instance)
        {
            __instance.gameObject.EnsureComponent<SaveMono>();
        }
    }
}
