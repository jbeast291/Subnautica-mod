using BepInEx;
using HarmonyLib;
using LifePodRemastered.Monos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static VFXParticlesPool;

namespace LifePodRemastered.patches;

internal class EscapePodPatch
{
    /**
     * patch in custom intro and hatch used state
     */
    [HarmonyPatch(typeof(EscapePod))]
    internal class OnStartPatch
    {
        
        [HarmonyPatch(nameof(EscapePod.Awake))]
        [HarmonyPostfix]
        public static void ExpandPodInventory(ref EscapePod __instance)
        {
            ItemsContainer podContainer = __instance.storageContainer.container;
            Vector2 vanilla = new Vector2(4, 8);
            if (podContainer.sizeX != (int)vanilla.x || podContainer.sizeY != (int)vanilla.y)
            {
                BepInExEntry.Logger.LogInfo($"lifepod inventory size already modified! Skipping changing for compatibility!");
                return;
            }
            if (SaveUtils.inGameSave.storageSize.IsNullOrWhiteSpace())
            {
                SaveUtils.inGameSave.storageSize = "4x8";
            }
            OptionsMono.storageSizes.TryGetValue(SaveUtils.inGameSave.storageSize, out Vector2 newSize);
            podContainer.Resize((int)newSize.x, (int)newSize.y);
        }
        

        [HarmonyPatch(nameof(EscapePod.Start))]
        [HarmonyPostfix]
        public static void OnStartPostFix(EscapePod __instance)
        {
            if (LPRGlobals.newSave && SaveUtils.settingsCache.startRepaired)
            {
                __instance.GetComponent<LiveMixin>().ResetHealth();
            }
            //always have heavy pod, some functions needed in intro
            __instance.gameObject.EnsureComponent<HeavyPodMono>();
            //This is enabled regarless of anything
            __instance.gameObject.EnsureComponent<PowerRelayController>();

            if (SaveUtils.settingsCache.customIntroToggle && LPRGlobals.newSave)
            {
                __instance.gameObject.EnsureComponent<EscapePodCustomIntro>();
            }

            if (LPRGlobals.newSave && !SaveUtils.settingsCache.FirstTimeToggle)
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
        [HarmonyPatch(nameof(EscapePod.OnRepair))]
        [HarmonyPostfix]
        public static void OnRepairPostFix()
        {
            HeavyPodMono.main.CreatePodUI();
        }
        [HarmonyPatch(typeof(EscapePod), nameof(EscapePod.DamageRadio))]
        [HarmonyPrefix]
        public static bool OnDamageRadioPostFix()
        {
            return !SaveUtils.settingsCache.startRepaired;
        }

    }
}
