using HarmonyLib;
using LifePodRemastered.Monos;
using System;
using System.Collections;
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

        [HarmonyPatch(nameof(Player.ResetPlayerOnDeath))]
        [HarmonyPrefix]
        public static void ResetPlayerOnDeathPreFix(Player __instance)
        {
            if (HeavyPodMono.main != null)
            {
                HeavyPodMono.main.playerRespawning = true;
            }
        }

        [HarmonyPatch(nameof(Player.ResetPlayerOnDeath))]
        [HarmonyPostfix]
        public static void ResetPlayerOnDeathPostfix(IEnumerator __result)
        {
            HeavyPodMono.main.StartCoroutine(WaitForFinish(__result));
        }

        private static IEnumerator WaitForFinish(IEnumerator target)
        {
            // Let the target run to completion
            while (target.MoveNext())
            {
                yield return target.Current;
            }
            HeavyPodMono.main.playerRespawning = false;
            //sometimes the player doesnt respawn at the right place, like outside the pod. So call this after and it seems to fix it :/
            Player.main.MovePlayerToRespawnPoint();
        }
    }
}
