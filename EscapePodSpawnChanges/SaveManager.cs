using HarmonyLib;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Nautilus.Utility;
using static LifePodRemastered.SaveManager;


namespace LifePodRemastered
{
    public class SaveManager
    {

        [System.Serializable]
        public class InGameSave
        {
            public bool HeavyPodToggle;
            public bool FirstTimeToggle;
            public bool CustomIntroToggle;
            public bool HeightReqToggle;
        }
        [System.Serializable]
        public class SettingsCache
        {
            public bool HeavyPodToggle;
            public bool FirstTimeToggle;
            public bool CustomIntroToggle;
            public bool HeightReqToggle;

            public bool ExSettingToggle;
        }
        public static InGameSave inGameSave = new InGameSave();
        public static SettingsCache settingsCache = new SettingsCache();

        public static void WriteSettingsToCurrentSlot()
        {
            string StringOutPut = JsonUtility.ToJson(inGameSave);
            File.WriteAllText(SaveUtils.GetCurrentSaveDataDir() + "/LifePodRemastered.json", StringOutPut);
        }

        public static void ReadSettingsFromCurrectSlot()
        {
            string file = File.ReadAllText(SaveUtils.GetCurrentSaveDataDir() + "/LifePodRemastered.json");
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
        public static void LoadChachedSettingsToSlotSave()
        {
            inGameSave.HeavyPodToggle = settingsCache.HeavyPodToggle;
            inGameSave.FirstTimeToggle = settingsCache.FirstTimeToggle;
            inGameSave.CustomIntroToggle = settingsCache.CustomIntroToggle;
            inGameSave.HeightReqToggle = settingsCache.HeightReqToggle;

        }
    }
}
