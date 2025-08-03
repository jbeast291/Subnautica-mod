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
            if (SaveUtils.settingsCache.customIntroToggle || SaveUtils.settingsCache.autoSkipVinillaIntroToggle)
            {
                __instance.Stop(true);
            }
        }
    }
}
