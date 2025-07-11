using HarmonyLib;
using LifePodRemastered.Monos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LifePodRemastered.patches;

internal class EscapePodPatch
{
    /**
     * patch in custom intro and hatch used state
     */
    [HarmonyPatch(typeof(EscapePod))]
    internal class OnStartPatch
    {
        [HarmonyPatch(nameof(EscapePod.Start))]
        [HarmonyPostfix]
        public static void OnStartPostFix(EscapePod __instance)
        {
            //always have heavy pod, some functions needed in intro
            __instance.gameObject.EnsureComponent<HeavyPodMono>();

            if (SaveUtils.settingsCache.CustomIntroToggle && Info.newSave)
            {
                __instance.gameObject.EnsureComponent<EscapePodCustomIntro>();
            }

            if (Info.newSave && !SaveUtils.settingsCache.FirstTimeToggle)
            {
                __instance.bottomHatchUsed = true;
                __instance.topHatchUsed = true;
            }
        }
        [HarmonyPatch(nameof(EscapePod.UpdateDamagedEffects))]
        [HarmonyPostfix]
        public static void OnUpdateDamagedEffectsPostFix(EscapePod __instance)
        {
            if (SaveUtils.inGameSave.HeavyPodToggle)
            {
                uGUI_EscapePod.main.content.text = uGUI_EscapePod.main.content.text.Replace("DEPLOYED", "FAILED ");
                uGUI_EscapePod.main.content.text = uGUI_EscapePod.main.content.text.Replace("DEPLOYED ", "FAILED ");
            }
            else
            {
                uGUI_EscapePod.main.content.text = uGUI_EscapePod.main.content.text.Replace("FAILED ", "DEPLOYED ");
            }

        }
    }
}
