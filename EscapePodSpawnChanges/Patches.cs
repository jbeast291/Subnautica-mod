using HarmonyLib;
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

namespace EscapePodSpawnChanges
{
    [HarmonyPatch(typeof(RandomStart))]
    internal class PatchPlayerOnToolActionStart
    {
        [HarmonyPatch(typeof(RandomStart), "GetRandomStartPoint")]
        [HarmonyPrefix]
        public static bool NewGetRandomStartPoint(ref Vector3 __result)
        {
            __result = Info.SelectedSpawn;
            Logger.Log(Logger.Level.Info, "Moved pod 1", null, true);
            return false;
        }
    }

    [HarmonyPatch(typeof(uGUI_MainMenu))]
    internal class PatchuGUI_MainMenu
    {
        [HarmonyPatch(typeof(uGUI_MainMenu), "OnButtonFreedom")]
        [HarmonyPrefix]
        public static bool NewOnButtonFreedom(uGUI_MainMenu __instance)
        {
            Info.showmap = true;
            Logger.Log(Logger.Level.Info, "canceled game launch", null, true);
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

        [HarmonyPrefix]
        public static bool Prefix(EscapePod __instance)
        {
            WorldForces wf = __instance.GetComponent<WorldForces>();

            wf.aboveWaterGravity = 100f;
            if (QMod.Config.ToggleHeavyPod)
            {
                wf.underwaterGravity = 9.81f;
            }
            if (!QMod.Config.ToggleHeavyPod)
            {
                wf.underwaterGravity = 0f;
            }
            return true;
        }
        [HarmonyPostfix]
        public static void Postfix(EscapePod __instance)
        {
            float distance = Vector3.Distance(Player.main.transform.position, EscapePod.main.transform.position);
            if (distance > 15)
            {
                __instance.rigidbodyComponent.isKinematic = true;
            }
            else
            {
                __instance.rigidbodyComponent.isKinematic = false;
            }
        }
    }
}
