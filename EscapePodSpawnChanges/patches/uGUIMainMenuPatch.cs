using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            EscapePodMainMenu.main.enableUI(GameMode.Survival);
            return false;
        }
        [HarmonyPatch(typeof(uGUI_MainMenu), "OnButtonCreative")]
        [HarmonyPrefix]
        public static bool OnButtonCreative()
        {
            Info.showmap = true;
            EscapePodMainMenu.main.enableUI(GameMode.Creative);
            return false;
        }
        [HarmonyPatch(typeof(uGUI_MainMenu), "OnButtonFreedom")]
        [HarmonyPrefix]
        public static bool OnButtonFreedom()
        {
            Info.showmap = true;
            EscapePodMainMenu.main.enableUI(GameMode.Freedom);
            return false;
        }
        [HarmonyPatch(typeof(uGUI_MainMenu), "OnButtonHardcore")]
        [HarmonyPrefix]
        public static bool OnButtonHardcore()
        {
            Info.showmap = true;
            EscapePodMainMenu.main.enableUI(GameMode.Hardcore);
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

            GameObject AreaSeceltor = GameObject.Instantiate(EscapePodMainMenu.assetBundle.LoadAsset<GameObject>("LifePodRemasteredCanvas"));
            SceneManager.MoveGameObjectToScene(AreaSeceltor, SceneManager.GetSceneByName("XMenu"));
            AreaSeceltor.transform.localPosition = new Vector3(0, 0, 7500);
            AreaSeceltor.SetActive(true);

            AreaSeceltor.gameObject.EnsureComponent<EscapePodMainMenu>();
        }
    }
}
