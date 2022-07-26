﻿using HarmonyLib;
using Logger = QModManager.Utility.Logger;
using RecipeData = SMLHelper.V2.Crafting.TechData;
using QModManager.API.ModLoading;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
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
            if (QMod.Config.ToggleAirSpawn && !Info.OverideSpawnHeight)
            {
                __result = new Vector3(Info.SelectedSpawn.x, QMod.Config.AirSpawnHeight, Info.SelectedSpawn.z);
            }
            else
            {
                __result = Info.SelectedSpawn;
            }
            return false;
        }
    }


    [HarmonyPatch(typeof(uGUI_MainMenu))] // 1 survival, 2 creative, 3, freedom, 4 hardcore
    internal class PatchuGUI_MainMenu
    {
        [HarmonyPatch(typeof(uGUI_MainMenu), "OnButtonSurvival")]
        [HarmonyPrefix]
        public static bool OnButtonSurvival(uGUI_MainMenu __instance)
        {
            Info.showmap = true;
            Info.GameMode = 1;
            return false;
        }
        [HarmonyPatch(typeof(uGUI_MainMenu), "OnButtonCreative")]
        [HarmonyPrefix]
        public static bool OnButtonCreative(uGUI_MainMenu __instance)
        {
            Info.showmap = true;
            Info.GameMode = 2;
            return false;
        }
        [HarmonyPatch(typeof(uGUI_MainMenu), "OnButtonFreedom")]
        [HarmonyPrefix]
        public static bool OnButtonFreedom(uGUI_MainMenu __instance)
        {
            Info.showmap = true;
            Info.GameMode = 3;
            return false;
        }
        [HarmonyPatch(typeof(uGUI_MainMenu), "OnButtonHardcore")]
        [HarmonyPrefix]
        public static bool OnButtonHardcore(uGUI_MainMenu __instance)
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
            __instance.gameObject.EnsureComponent<LifePodMapFunction>();
        }
    }

    [HarmonyPatch(typeof(EscapePod))]
    [HarmonyPatch("FixedUpdate")]
    internal class EscapePod_FixedUpdate_Patch
    {
        static bool tempbool = false;
        [HarmonyPrefix]
        public static bool Prefix(EscapePod __instance)
        {
            WorldForces wf = __instance.GetComponent<WorldForces>();

            wf.aboveWaterGravity = 100f;
            if (QMod.Config.ToggleHeavyPod)
            {
                wf.underwaterGravity = QMod.Config.HeavyPodIntensity;
            }
            if (!QMod.Config.ToggleHeavyPod)
            {
                wf.underwaterGravity = -30f;
            }

            return true;
        }
        [HarmonyPostfix]
        public static void Postfix(EscapePod __instance)
        {
            float distance = Vector3.Distance(Player.main.transform.position, EscapePod.main.transform.position);

            if (!Info.newSave || !QMod.Config.ToggleAirSpawn)
            {
                if ((distance > 50 || Info.isrepawning) && !__instance.rigidbodyComponent.isKinematic)
                {
                    __instance.rigidbodyComponent.isKinematic = true;
                }
                if (distance < 50 && !Info.isrepawning)
                {
                    __instance.rigidbodyComponent.isKinematic = false;
                }
                if (Info.isrepawning && !tempbool)
                {
                    CoroutineHost.StartCoroutine(WaitTillSpawned());
                    tempbool = true;
                    __instance.rigidbodyComponent.isKinematic = true;
                }
            }

            IEnumerator WaitTillSpawned()
            {
                yield return new WaitForSeconds(20);
                Info.isrepawning = false;
                tempbool = false;
            }
        }
    }
    [HarmonyPatch(typeof(uGUI_PlayerDeath))]
    internal class OnResetPlayerOnDeath
    {
         [HarmonyPatch(nameof(uGUI_PlayerDeath.TriggerDeathVignette))]
         [HarmonyPrefix]
         public static bool OnTriggerDeathVignettePreFix(uGUI_PlayerDeath __instance)
         {
             Info.isrepawning = true;
             return true;
         }
    }
    [HarmonyPatch(typeof(EscapePod))]
    internal class OnStartPatch
    {
        [HarmonyPatch(nameof(EscapePod.Start))]
        [HarmonyPostfix]
        public static void OnStartPostFix(EscapePod __instance)
        {
            __instance.gameObject.EnsureComponent<EscapePodInGameMono>();

            __instance.bottomHatchUsed = QMod.Config.DisableFirstTimeAnims;
            __instance.topHatchUsed = QMod.Config.DisableFirstTimeAnims;
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

    [HarmonyPatch(typeof(uGUI_SceneIntro))]
    internal class OnuGUI_SceneIntroPatch
    {
        [HarmonyPatch(nameof(uGUI_SceneIntro.IntroSequence))]
        [HarmonyPrefix]
        public static bool OnEscapeHoldPreFix(uGUI_SceneIntro __instance)
        {
            if(QMod.Config.SkipInto)
            {
                __instance.menuStillDown = true;
                return false;
            }
            else
            {
                return true;
            }

        }
    }
}
