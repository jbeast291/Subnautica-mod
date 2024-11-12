using HarmonyLib;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Nautilus.Utility;
using static LifePodRemastered.SaveUtils;


namespace LifePodRemastered
{
    public class SaveUtils
    {

        [System.Serializable]
        public class InGameSave
        {
            public bool HeavyPodToggle;
        }
        [System.Serializable]
        public class SettingsCache
        {
            //non experimental
            public bool HeavyPodToggle;
            public bool FirstTimeToggle;
            public bool CinematicOverlayToggle;

            //experimental
            public bool ExSettingToggle;
            //by default all should be true
            public bool CustomIntroToggle;
            public bool HeightReqToggle;
        }
        public static InGameSave inGameSave = new InGameSave();
        public static SettingsCache settingsCache = new SettingsCache();

        public static void WriteSettingsToCurrentSlot()
        {
            string StringOutPut = JsonUtility.ToJson(inGameSave);
            File.WriteAllText(Nautilus.Utility.SaveUtils.GetCurrentSaveDataDir() + "/LifePodRemastered.json", StringOutPut);
        }

        public static void ReadSettingsFromCurrectSlot()
        {
            string file = File.ReadAllText(Nautilus.Utility.SaveUtils.GetCurrentSaveDataDir() + "/LifePodRemastered.json");
            inGameSave = JsonUtility.FromJson<InGameSave>(file);
        }
        public static void WriteSettingsToModFolder()
        {
            string StringOutPut = JsonUtility.ToJson(settingsCache);
            File.WriteAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/SettingsChache.json", StringOutPut);
        }

        public static void ReadSettingsFromModFolder()
        {
            string file = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/SettingsChache.json");
            settingsCache = JsonUtility.FromJson<SettingsCache>(file);
        }

        public static void CreateDefaultConfigIfModFolderCacheDoesNotExist()
        {
            if(!File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/SettingsChache.json"))
            {
                ResetSettingsCacheToDefault();
                WriteSettingsToModFolder();
            }
        }
        public static void CreateDefaultSlotSettingsIfNotExist()
        {
            if (!File.Exists(Nautilus.Utility.SaveUtils.GetCurrentSaveDataDir() + "/LifePodRemastered.json"))
            {
                //set values to be conservative just in case, basically so it doesnt fuck up the save
                inGameSave.HeavyPodToggle = false;
                WriteSettingsToCurrentSlot();
            }
        }

        public static void ResetSettingsCacheToDefault()
        {
            settingsCache.HeavyPodToggle = true;
            settingsCache.FirstTimeToggle = true;
            settingsCache.CinematicOverlayToggle = true;

            settingsCache.ExSettingToggle = false;

            settingsCache.CustomIntroToggle = true;
            settingsCache.HeightReqToggle = true;
        }

        public static void LoadChachedSettingsToSlotSave()
        {
            inGameSave.HeavyPodToggle = settingsCache.HeavyPodToggle;
        }
    }
}
