using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LifePodRemastered.patches;

internal class uGUIMainMenuPatch
{
    /**
     * Patch regular menu buttons to reroute to the map system
     */
    [HarmonyPatch(typeof(uGUI_MainMenu))] // 1 survival, 2 creative, 3, freedom, 4 hardcore
    internal class PatchuGUI_MainMenu
    {
        [HarmonyPatch(typeof(uGUI_MainMenu), "OnButtonSurvival")]
        [HarmonyPrefix]
        public static bool OnButtonSurvival()
        {
            Info.showmap = true;
            Info.GameMode = GameMode.Survival;
            return false;
        }
        [HarmonyPatch(typeof(uGUI_MainMenu), "OnButtonCreative")]
        [HarmonyPrefix]
        public static bool OnButtonCreative()
        {
            Info.showmap = true;
            Info.GameMode = GameMode.Creative;
            return false;
        }
        [HarmonyPatch(typeof(uGUI_MainMenu), "OnButtonFreedom")]
        [HarmonyPrefix]
        public static bool OnButtonFreedom()
        {
            Info.showmap = true;
            Info.GameMode = GameMode.Freedom;
            return false;
        }
        [HarmonyPatch(typeof(uGUI_MainMenu), "OnButtonHardcore")]
        [HarmonyPrefix]
        public static bool OnButtonHardcore()
        {
            Info.showmap = true;
            Info.GameMode = GameMode.Hardcore;
            return false;
        }
        [HarmonyPatch(typeof(uGUI_MainMenu), "Start")]
        [HarmonyPostfix]
        public static void StartPostPatch(uGUI_MainMenu __instance)
        {
            //this needs to be done before anything to prevent breakages on new installs
            SaveUtils.CreateDefaultConfigIfModFolderCacheDoesNotExist();

            //these need to be reset even if the gameobject is never activated, otherwise the intro and other settings get fucked
            SaveUtils.ReadSettingsFromModFolder();//load for menu
            Info.resetInfo();

            __instance.gameObject.EnsureComponent<EscapePodMainMenu>();
        }
    }

}
