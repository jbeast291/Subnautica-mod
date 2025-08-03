using LifePodRemastered.presetSystem;
using LifePodRemastered.presetSystem.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LifePodRemastered.SaveUtils;

namespace LifePodRemastered;

internal class OptionsMono : MonoBehaviour
{
    public static OptionsMono main;

    GameObject VerticalLayout;
    GameObject Content;
    GameObject ToggleTemplate;
    GameObject SelectorTemplate;

    TextMeshProUGUI OptionsTitle;

    List<BaseOption> Options = new();

    public static readonly Dictionary<string, Vector2> storageSizes = new()
    {
        { "1x1", new Vector2(1, 1) },
        { "2x2", new Vector2(2, 2) },
        { "4x4", new Vector2(4, 4) },
        { "4x8", new Vector2(4, 8) },
        { "6x10", new Vector2(6, 10) },
        { "8x10", new Vector2(8, 10) },
    };

    public void Awake()
    {
        if (main != null)
        {
            UnityEngine.Debug.LogError($"Duplicate {this.GetType().Name} found!");
            Destroy(this);
            return;
        }
        main = this;


        VerticalLayout = GameObject.Find("OptionsVerticalLayout");
        Content = GameObject.Find("OptionsContent");
        OptionsTitle = GameObject.Find("OptionsTitle").GetComponent<TextMeshProUGUI>();

        ToggleTemplate = GameObject.Find("ToggleTemplate");
        ToggleTemplate.SetActive(false);

        SelectorTemplate = GameObject.Find("SelectorTemplate");
        SelectorTemplate.SetActive(false);

        ModePreset modePreset = (ModePreset) EscapePodMainMenu.main.GetRegisteredModeByType(typeof(ModePreset));

        OptionLoadoutSelector loadoutOption = new OptionLoadoutSelector(
            "loadoutOption",
            "LPR.LoadoutTitle",
            "LPR.LoadoutDescription",
            getNewOptionSelector(),
            SaveUtils.settingsCache.selectedLoadoutName,
            modePreset,
            new Action<string, bool>(onLoadoutSelectorChange));
        loadoutOption.init();
        Options.Add(loadoutOption);

        OptionSelector storageSizeOption = new OptionSelector(
            "storageSizeOption",
            "LPR.StorageSizeTitle",
            "LPR.StorageSizeDescription",
            getNewOptionSelector(),
            SaveUtils.settingsCache.storageSize,
            storageSizes.Keys.ToList(),
            new Action<string, bool>(onStorageSizeChange));
        storageSizeOption.init();
        Options.Add(storageSizeOption);

        OptionToggle heavyPodOption = new OptionToggle(
            "heavyPodOption",
            "LPR.HeavyPodTitle", 
            "LPR.HeavyPodDescription",
            getNewOptionToggle().GetComponent<Toggle>(),
            settingsCache.heavyPodToggle,
            new Action<bool, bool>(onHeavyPodToggle));
        Options.Add(heavyPodOption);

        OptionToggle startRepairedOption = new OptionToggle(
            "startRepairedOption",
            "LPR.StartRepairedTitle",
            "LPR.StartRepairedDescription",
            getNewOptionToggle().GetComponent<Toggle>(),
            settingsCache.startRepaired,
            new Action<bool, bool>(onStartRepairedToggle));
        Options.Add(startRepairedOption);

        OptionToggle firstTimeAnimationsOption = new OptionToggle(
            "firstTimeAnimationsOption",
            "LPR.FirstTimeAnimationsTitle", 
            "LPR.FirstTimeAnimationsDescription",
            getNewOptionToggle().GetComponent<Toggle>(),
            settingsCache.FirstTimeToggle,
            new Action<bool, bool>(onFirstTimeAnimToggle));
        Options.Add(firstTimeAnimationsOption);

        OptionToggle cinematicOverlayOption = new OptionToggle(
            "cinematicOverlayOption",
            "LPR.CinematicOverlayTitle", 
            "LPR.CinematicOverlayDescription",
            getNewOptionToggle().GetComponent<Toggle>(),
            settingsCache.CinematicOverlayToggle,
            new Action<bool, bool>(onCinematicOverlayToggle));
        Options.Add(cinematicOverlayOption);

        OptionToggle customIntroOption = new OptionToggle(
            "customIntroOption",
            "LPR.CustomIntroTitle", 
            "LPR.CustomIntroDescription",
            getNewOptionToggle().GetComponent<Toggle>(),
            settingsCache.customIntroToggle,
            new Action<bool, bool>(onCustomIntroToggle));
        Options.Add(customIntroOption);

        OptionToggle autoSkipVinillaIntroOption = new OptionToggle(
            "autoSkipVinillaIntroOption",
            "LPR.AutoSkipVinillaIntroTitle",
            "LPR.AutoSkipVinillaIntroDescription",
            getNewOptionToggle().GetComponent<Toggle>(),
            settingsCache.autoSkipVinillaIntroToggle,
            new Action<bool, bool>(AutoSkipVinillaIntroToggle));
        Options.Add(autoSkipVinillaIntroOption);

        OptionsTitle.text = Language.main.Get("LPR.OptionsTitle");

        updateViewPortHeight();

    }
    public void OnEnable()
    {
        reloadLanguage();
    }
    public void setOptionToValue(string optionID, bool interactable, object value)
    {
        for (int i = 0; i < Options.Count; i++)
        {
            if (Options[i].GetOptionID() == optionID)
            {
                Options[i].SetToggledState(interactable, value);
            }
        }
    }
    public GameObject getNewOptionToggle()
    {
        GameObject toggleOption = Instantiate(ToggleTemplate);
        toggleOption.transform.SetParent(VerticalLayout.transform, false);
        toggleOption.SetActive(true);
        return toggleOption;
    }
    public GameObject getNewOptionSelector()
    {
        GameObject toggleOption = Instantiate(SelectorTemplate);
        toggleOption.transform.SetParent(VerticalLayout.transform, false);
        toggleOption.SetActive(true);
        return toggleOption;
    }
    public void updateViewPortHeight()
    {
        float gapSize = VerticalLayout.GetComponent<VerticalLayoutGroup>().spacing;
        float totalHeight = 0;
        for (int i = 0; i < Options.Count; i++)
        {
            totalHeight += Options[i].GetHeight() + gapSize;
        }

        float extraHeight = 5;
        totalHeight+= extraHeight;

        RectTransform ContentRT = Content.GetComponent<RectTransform>();
        ContentRT.sizeDelta = new Vector2(ContentRT.sizeDelta.x, totalHeight);

        RectTransform VerticalLayoutRT = VerticalLayout.GetComponent<RectTransform>();
        VerticalLayoutRT.sizeDelta = new Vector2(VerticalLayoutRT.sizeDelta.x, totalHeight);

    }
    public void reloadLanguage()
    {
        for (int i = 0; i < Options.Count; i++)
        {
            Options[i].ReloadLanguage();
        }
        OptionsTitle.text = Language.main.Get("OptionsTitle");
    }

    public void onLoadoutSelectorChange(string value, bool saveToConfig)
    {
        settingsCache.selectedLoadoutName = value;
        if (saveToConfig)
        {
            WriteSettingsToModFolder();
        }
    }
    public void onStorageSizeChange(string value, bool saveToConfig)
    {
        settingsCache.storageSize = value;
        if (saveToConfig)
        {
            WriteSettingsToModFolder();
        }
    }
    public void onHeavyPodToggle(bool value, bool saveToConfig)
    {
        settingsCache.heavyPodToggle = value;
        if(saveToConfig)
        {
            WriteSettingsToModFolder();
        }
    }
    public void onStartRepairedToggle(bool value, bool saveToConfig)
    {
        settingsCache.startRepaired = value;
        if (saveToConfig)
        {
            WriteSettingsToModFolder();
        }
    }
    public void onFirstTimeAnimToggle(bool value, bool saveToConfig)
    {
        settingsCache.FirstTimeToggle = value;
        if (saveToConfig)
        {
            WriteSettingsToModFolder();
        }
    }
    public void onCinematicOverlayToggle(bool value, bool saveToConfig)
    {
        settingsCache.CinematicOverlayToggle = value;
        if (saveToConfig)
        {
            WriteSettingsToModFolder();
        }
    }
    public void onCustomIntroToggle(bool value, bool saveToConfig)
    {
        settingsCache.customIntroToggle = value;
        if (saveToConfig)
        {
            WriteSettingsToModFolder();
        }
    }
    public void AutoSkipVinillaIntroToggle(bool value, bool saveToConfig)
    {
        settingsCache.autoSkipVinillaIntroToggle = value;
        if (saveToConfig)
        {
            WriteSettingsToModFolder();
        }
    }

}
