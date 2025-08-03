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


namespace LifePodRemastered;

public class SaveUtils
{

    [System.Serializable]
    public class InGameSave
    {
        public bool HeavyPodToggle;
        public string storageSize;
    }
    public static InGameSave inGameSave = new InGameSave();

    [System.Serializable]
    public class SettingsCache
    {
        public string selectedLoadoutName;
        public string storageSize;
        public bool heavyPodToggle;
        public bool startRepaired;
        public bool FirstTimeToggle;
        public bool CinematicOverlayToggle;

        public bool customIntroToggle;
        public bool autoSkipVinillaIntroToggle;

        public float VertialMotionRate;
    }

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
        File.WriteAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/SettingsCache.json", StringOutPut);
    }

    public static void ReadSettingsFromModFolder()
    {
        string file = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/SettingsCache.json");
        settingsCache = JsonUtility.FromJson<SettingsCache>(file);
    }

    public static void CreateDefaultConfigIfModFolderCacheDoesNotExist()
    {
        if (!File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/SettingsCache.json"))
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
            inGameSave.storageSize = "4x8";
            WriteSettingsToCurrentSlot();
        }
    }

    public static void ResetSettingsCacheToDefault()
    {
        settingsCache.selectedLoadoutName = "Vinilla+";
        settingsCache.storageSize = "4x8";
        settingsCache.heavyPodToggle = true;
        settingsCache.startRepaired = false;
        settingsCache.FirstTimeToggle = true;
        settingsCache.CinematicOverlayToggle = true;

        settingsCache.customIntroToggle = true;
        settingsCache.VertialMotionRate = 10f;
    }

    public static void LoadChachedSettingsToSlotSave()
    {
        inGameSave.HeavyPodToggle = settingsCache.heavyPodToggle;
        inGameSave.storageSize = settingsCache.storageSize;
    }
}

