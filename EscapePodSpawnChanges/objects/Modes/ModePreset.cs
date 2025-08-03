using FMOD;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LifePodRemastered.presetSystem;

internal class ModePreset : Mode
{
    GameObject presetRoot;

    Button ModePresetPointChoiceLeft;
    Button ModePresetPointChoiceRight;

    TextMeshProUGUI presetText;

    TextMeshProUGUI presetSubText;
    TextMeshProUGUI presetSubSubText; // lol

    TextMeshProUGUI notice;

    List<BasePreset> presets;
    List<string> presetFileNames;

    int CurrentPresetIndex = 0;

    bool hasOverridSettings = false;

    public ModePreset(EscapePodMainMenu escapePodMainMenu, String nameLanguageKey, String descriptionLanguageKey, GameObject presetRoot)
        : base(escapePodMainMenu, nameLanguageKey, descriptionLanguageKey, true)
    {
        this.presetRoot = presetRoot;

        presetFileNames = GetPresetNamesFromModFolder();

        presets = new List<BasePreset>();
        for(int i = 0; i < presetFileNames.Count; i++)
        {
            BasePreset preset = new BasePreset(presetFileNames[i]);
            if (preset.Load())
            {
                presets.Add(preset);
            }
        }

        ModePresetPointChoiceLeft = presetRoot.FindChild("ModePresetPointChoiceleft").GetComponent<Button>();
        ModePresetPointChoiceLeft.onClick.AddListener(MovePresetLeft);
        ModePresetPointChoiceRight = presetRoot.FindChild("ModePresetPointChoiceRight").GetComponent<Button>();
        ModePresetPointChoiceRight.onClick.AddListener(MovePresetRight);

        presetText = presetRoot.FindChild("PresetText").GetComponent<TextMeshProUGUI>();

        presetSubText = presetRoot.GetComponent<TextMeshProUGUI>();
        presetSubSubText = presetRoot.transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        notice = presetRoot.FindChild("NoticeBackground").transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        if(presets.Count > 0)
        {
            BasePreset selectedPreset = presets[CurrentPresetIndex];
            presetText.text = selectedPreset.fileName;
        }
        ReloadLanguage();
    }
    public override void ToggleMode(bool toggle)
    {
        presetRoot.SetActive(toggle);
        if (hasOverridSettings)
        {
            SaveUtils.ReadSettingsFromModFolder();//reload the previous saved items, so that what was temporarily overriden by the preset is restored
            UnlockSettings();
        }
        if (toggle)
        {
            UpdateSelectedPresetInUI();
        }
    }
    public override void ReloadLanguage()
    {
        presetSubText.text = Language.main.Get("LPR.ModePresetText");
        presetSubSubText.text = Language.main.Get("LPR.ModePresetSubText");
        UpdateNotice();
    }

    public override void EnableInteraction(bool toggle)
    {
        ModePresetPointChoiceLeft.interactable = toggle;
        ModePresetPointChoiceRight.interactable = toggle;
    }
    public void UpdateNotice()
    {
        if (presets.Count <= 0)
        {
            return;
        }

        if (presets[CurrentPresetIndex].errorsDuringParsing)
        {
            notice.text = Language.main.Get("LPR.ModePresetErrorNotification");
            notice.color = new Color(1, 0, 0, 1);
        } 
        else
        {
            notice.text = Language.main.Get("LPR.ModePresetNotice");
            notice.color = new Color(1, 1, 1, 1);
        }
    }
    public void UpdateSelectedPresetInUI()
    {
        if (presets.Count > 0)
        {
            BasePreset selectedPreset = presets[CurrentPresetIndex];
            presetText.text = selectedPreset.fileName;
            escapePodMainMenu.MoveSelecedPointFromWorldPoint(selectedPreset.location);
            LockSettingsToPreset();
        }
    }
    public void LockSettingsToPreset()
    {
        BasePreset selectedPreset = presets[CurrentPresetIndex];
        OptionsMono.main.setOptionToValue("loadoutOption", false, selectedPreset.fileName);
        OptionsMono.main.setOptionToValue("storageSizeOption", false, OptionsMono.storageSizes.FirstOrDefault(kv => kv.Value == selectedPreset.storageSize).Key);
        OptionsMono.main.setOptionToValue("heavyPodOption", false, selectedPreset.heavyPod);
        OptionsMono.main.setOptionToValue("startRepairedOption", false, selectedPreset.startRepaired);
        OptionsMono.main.setOptionToValue("customIntroOption", false, selectedPreset.customIntro);
        hasOverridSettings = true;
    }
    public void UnlockSettings()
    {
        OptionsMono.main.setOptionToValue("loadoutOption", true, SaveUtils.settingsCache.selectedLoadoutName);
        OptionsMono.main.setOptionToValue("storageSizeOption", true, SaveUtils.settingsCache.storageSize);
        OptionsMono.main.setOptionToValue("heavyPodOption", true, SaveUtils.settingsCache.heavyPodToggle);
        OptionsMono.main.setOptionToValue("startRepairedOption", true, SaveUtils.settingsCache.startRepaired);
        OptionsMono.main.setOptionToValue("customIntroOption", true, SaveUtils.settingsCache.customIntroToggle);
        hasOverridSettings = false;
    }

    public void MovePresetLeft()
    {
        if (CurrentPresetIndex != 0)
        {
            CurrentPresetIndex--;
        } else
        {
            CurrentPresetIndex = presets.Count - 1;
        }
        escapePodMainMenu.PlayButtonPressSound();
        UpdateSelectedPresetInUI();
        UpdateNotice();
    }
    public void MovePresetRight()
    {
        if (CurrentPresetIndex != presets.Count - 1)
        {
            CurrentPresetIndex++;
        }
        else
        {
            CurrentPresetIndex = 0;
        }
        escapePodMainMenu.PlayButtonPressSound();
        UpdateSelectedPresetInUI();
        UpdateNotice();
    }
    public BasePreset getPresetByIndex(int index)
    {
        return presets[index];
    }
    public List<string> getPresetFileNames()
    {
        return presetFileNames;
    }

    private static List<string> GetPresetNamesFromModFolder()
    {
        string presetsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),"Presets");

        // look for .json files only
        var presetNames = new List<string>();
        foreach (string filePath in Directory.GetFiles(presetsFolder, "*.json"))
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            presetNames.Add(fileName);
        }

        return presetNames;
    }
    
}

