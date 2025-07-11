using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LifePodRemastered.patches;

internal class uGUISceneIntroPatch
{
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
