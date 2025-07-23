using LifePodRemastered.objects;
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
    GameObject VerticalLayout;
    GameObject Content;
    GameObject ToggleTemplate;

    TextMeshProUGUI OptionsTitle;

    //Toggle heavyPodToggle;
    //Toggle firstTimeAnimToggle;
    //Toggle experimentalSettingToggle;
    //Toggle customIntroToggle;
    //Toggle heightReqToggle;
    //Toggle cinematicOverlayToggle;

    //GameObject experimentalSettingsGroup;

    List<ToggleOption> toggleOptions = new();

    public void Awake()
    {
        VerticalLayout = GameObject.Find("OptionsVerticalLayout");
        Content = GameObject.Find("OptionsContent");
        OptionsTitle = GameObject.Find("OptionsTitle").GetComponent<TextMeshProUGUI>();

        ToggleTemplate = GameObject.Find("ToggleTemplate");
        ToggleTemplate.SetActive(false);

        ToggleOption heavyPodOption = new ToggleOption(
            getNewToggleOption().GetComponent<Toggle>(), 
            "LPR.HeavyPodTitle", 
            "LPR.HeavyPodDescription",
            settingsCache.HeavyPodToggle,
            new Action<bool>(onHeavyPodToggle));
        toggleOptions.Add(heavyPodOption);

        ToggleOption firstTimeAnimationsOption = new ToggleOption(
            getNewToggleOption().GetComponent<Toggle>(), 
            "LPR.FirstTimeAnimationsTitle", 
            "LPR.FirstTimeAnimationsDescription",
            settingsCache.FirstTimeToggle,
            new Action<bool>(onFirstTimeAnimToggle));
        toggleOptions.Add(firstTimeAnimationsOption);

        ToggleOption cinematicOverlayOption = new ToggleOption(
            getNewToggleOption().GetComponent<Toggle>(), 
            "LPR.CinematicOverlayTitle", 
            "LPR.CinematicOverlayDescription",
            settingsCache.CinematicOverlayToggle,
            new Action<bool>(onCinematicOverlayToggle));
        toggleOptions.Add(cinematicOverlayOption);

        ToggleOption customIntroOption = new ToggleOption(
            getNewToggleOption().GetComponent<Toggle>(), 
            "LPR.CustomIntroTitle", 
            "LPR.CustomIntroDescription",
            settingsCache.CustomIntroToggle,
            new Action<bool>(onCustomIntroToggle));
        toggleOptions.Add(customIntroOption);

        ToggleOption airSpawnOption = new ToggleOption(
            getNewToggleOption().GetComponent<Toggle>(), 
            "LPR.AirSpawnTitle", 
            "LPR.AirSpawnDescription",
            settingsCache.HeightReqToggle,
            new Action<bool>(HeightReqToggle));
        toggleOptions.Add(airSpawnOption);

        OptionsTitle.text = Language.main.Get("LPR.OptionsTitle");

        updateViewPortHeight();
    }
    public GameObject getNewToggleOption()
    {
        GameObject toggleOption = Instantiate(ToggleTemplate);
        toggleOption.transform.SetParent(VerticalLayout.transform, false);
        toggleOption.SetActive(true);
        return toggleOption;
    }
    public void updateViewPortHeight()
    {
        float totalHeight = 0;
        for (int i = 0; i < toggleOptions.Count; i++)
        {
            totalHeight += toggleOptions[i].GetHeight();
        }

        RectTransform VerticalLayoutRT = VerticalLayout.GetComponent<RectTransform>();
        VerticalLayoutRT.sizeDelta = new Vector2(VerticalLayoutRT.sizeDelta.x, totalHeight);

        RectTransform ContentRT = Content.GetComponent<RectTransform>();
        ContentRT.sizeDelta = new Vector2(ContentRT.sizeDelta.x, totalHeight);
    }
    void OnEnable()
    {
        reloadLanguage();
    }
    public void reloadLanguage()
    {
        for (int i = 0; i < toggleOptions.Count; i++)
        {
            toggleOptions[i].reloadLanguage();
        }
        OptionsTitle.text = Language.main.Get("OptionsTitle");
    }

    public void onHeavyPodToggle(bool value)
    {
        settingsCache.HeavyPodToggle = value;
        WriteSettingsToModFolder();
    }
    
    public void onFirstTimeAnimToggle(bool value)
    {
        settingsCache.FirstTimeToggle = value;
        WriteSettingsToModFolder();
    }
    public void onCinematicOverlayToggle(bool value)
    {
        settingsCache.CinematicOverlayToggle = value;
        WriteSettingsToModFolder();
    }
    public void onCustomIntroToggle(bool value)
    {
        settingsCache.CustomIntroToggle = value;
        WriteSettingsToModFolder();
    }
    public void HeightReqToggle(bool value)
    {
        settingsCache.HeightReqToggle = value;
        WriteSettingsToModFolder();
    }
}
