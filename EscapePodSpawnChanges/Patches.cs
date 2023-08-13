using HarmonyLib;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UWE;

namespace LifePodRemastered
{
    [HarmonyPatch(typeof(RandomStart))]
    internal class PatchGetRandomStartPoint
    {
        [HarmonyPatch(typeof(RandomStart), "GetRandomStartPoint")]
        [HarmonyPrefix]
        public static bool NewGetRandomStartPoint(ref Vector3 __result)
        {
            __result = new Vector3(Info.SelectedSpawn.x, 2000f, Info.SelectedSpawn.z);
            return false;
        }
    }


    [HarmonyPatch(typeof(uGUI_MainMenu))] // 1 survival, 2 creative, 3, freedom, 4 hardcore
    internal class PatchuGUI_MainMenu
    {
        [HarmonyPatch(typeof(uGUI_MainMenu), "OnButtonSurvival")]
        [HarmonyPrefix]
        public static bool OnButtonSurvival()
        {
            Info.showmap = true;
            Info.GameMode = 1;
            return false;
        }
        [HarmonyPatch(typeof(uGUI_MainMenu), "OnButtonCreative")]
        [HarmonyPrefix]
        public static bool OnButtonCreative()
        {
            Info.showmap = true;
            Info.GameMode = 2;
            return false;
        }
        [HarmonyPatch(typeof(uGUI_MainMenu), "OnButtonFreedom")]
        [HarmonyPrefix]
        public static bool OnButtonFreedom()
        {
            Info.showmap = true;
            Info.GameMode = 3;
            return false;
        }
        [HarmonyPatch(typeof(uGUI_MainMenu), "OnButtonHardcore")]
        [HarmonyPrefix]
        public static bool OnButtonHardcore()
        {
            Info.showmap = true;
            Info.GameMode = 4;
            return false;
        }
    }

    [HarmonyPatch(typeof(uGUI_MainMenu))]
    internal class PatchuGUI_MainMenuStart
    {
        [HarmonyPatch(typeof(uGUI_MainMenu), "Start")]
        [HarmonyPostfix]
        public static void StartPostPatch(uGUI_MainMenu __instance)
        {
            __instance.gameObject.EnsureComponent<EscapePodChangesMono>();
        }
    }
    /*
    [HarmonyPatch(typeof(EscapePod))]
    [HarmonyPatch("FixedUpdate")]
    internal class EscapePod_FixedUpdate_Patch
    {
        static bool tempbool = false;
        //static bool tempbool2 = false;
        [HarmonyPrefix]
        public static bool Prefix(EscapePod __instance)
        {
            WorldForces wf = __instance.GetComponent<WorldForces>();

            wf.aboveWaterGravity = 500;
            if (true)
            {
                wf.underwaterGravity = 250;
            }

            return true;
        }
    }*/
    [HarmonyPatch(typeof(EscapePod))]
    internal class OnStartPatch
    {
        [HarmonyPatch(nameof(EscapePod.Start))]
        [HarmonyPostfix]
        public static void OnStartPostFix(EscapePod __instance)
        {
            __instance.gameObject.EnsureComponent<EscapePodInGameMono>();

            __instance.bottomHatchUsed = true;
            __instance.topHatchUsed = true;
        }
    }
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
    [HarmonyPatch(typeof(Player))]
    internal class OnLandPreFixPatch
    {
        [HarmonyPatch(nameof(Player.OnLand))]
        [HarmonyPrefix]
        public static bool OnLandPreFix()
        {
            return false;
        }
    }
    [HarmonyPatch(typeof(uGUI_SceneIntro))]
    internal class OnuGUI_SceneIntroPatch
    {
        [HarmonyPatch(nameof(uGUI_SceneIntro.OnUpdate))]
        [HarmonyPrefix]
        public static void OnEscapeHoldPreFix(uGUI_SceneIntro __instance)
        {
            __instance.Stop(true);
        }
    }
}

