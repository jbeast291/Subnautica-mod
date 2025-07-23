using FMOD;
using HarmonyLib;
using LifePodRemastered.Monos;
using LifePodRemastered.objects;
using Nautilus.Assets;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UWE;
using static LifePodRemastered.SaveUtils;
using static UnityEngine.UI.Selectable;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace LifePodRemastered;

public class EscapePodMainMenu : MonoBehaviour, IPointerDownHandler
{
    public static EscapePodMainMenu main;
    public static AssetBundle assetBundle = Info.assetBundle;

    GameMode selectedGameMode = GameMode.None;

    GameObject HoverSound;
    GameObject ClickSound;
    GameObject ButtonHoverSharp;

    Canvas originalGameCanvas;

    GameObject OptionsPannel;
    GameObject ModeChoiceText;

    GameObject mapsBackground;
    GameObject CoordsDisplay;
    GameObject ScalePoint1;
    GameObject ScalePoint2;
    GameObject BoundBottomLeft;
    GameObject BoundTopRight;
    GameObject SelectedPoint;
    GameObject SelectedPointIndicator;

    TextMeshProUGUI RightSideInfoText;

    GameObject ModeChoiceTitleText;
    GameObject BackButton;
    GameObject SettingsButton;
    GameObject StartGameButton;


    int currentModeIndex = 0;

    Button playButton;
    bool hasSelectedPoint = false;

    GameObject cycleMapButton;
    int currentMapSelected = 0;
    List<GameObject> mapVariants;

    List<Mode> modes;
    List<Action<PointerEventData>> registeredMouseDownEvents;

    bool showModSettings = false;

    public void Awake()
    {
        if (main != null)
        {
            Debug.LogError("Duplicate EscapePodMainMenu found!");
            Destroy(this);
            return;
        }
        main = this;

        originalGameCanvas = GameObject.Find("Menu canvas").GetComponent<Canvas>();
        registeredMouseDownEvents = new List<Action<PointerEventData>>();//just init
    }
    public void Start()
    {

        //Sounds
        HoverSound = GameObject.Find("ButtonHover");
        ClickSound = GameObject.Find("ButtonClick");
        ButtonHoverSharp = GameObject.Find("ButtonHoverSharp");

        //Butons
        SettingsButton = GameObject.Find("SettingModsButton");
        SettingsButton.GetComponent<Button>().onClick.AddListener(OnSettingsModButtonClick);
        StartGameButton = GameObject.Find("StartGameButton");
        StartGameButton.GetComponent<Button>().onClick.AddListener(OnStartGameButtonClick);
        BackButton = GameObject.Find("BackToMenuButton");
        BackButton.GetComponent<Button>().onClick.AddListener(OnBackToMenuButtonClick);
        GameObject.Find("ModeChoiceleft").GetComponent<Button>().onClick.AddListener(OnModeChoiceleftClick);
        GameObject.Find("ModeChoiceRight").GetComponent<Button>().onClick.AddListener(OnModeChoiceRightClick);

        //Modes
        modes = new List<Mode>();
        ModeSpecific ModeSpecificPoint = new ModeSpecific(this, "LPR.ModeSpecificName", "LPR.ModeSpecificDescription");
        modes.Add(ModeSpecificPoint);

        GameObject ModePresetPointRoot = GameObject.Find("ModePresetPoint");
        ModePreset ModePresetPoint = new ModePreset(this, "LPR.ModePresetName", "LPR.ModePresetDescription", ModePresetPointRoot);
        modes.Add(ModePresetPoint);

        GameObject ModeRandomPointRoot = GameObject.Find("RandomizePointButton");
        ModeRandom ModeRandomPoint = new ModeRandom(this, "LPR.ModeRandomName", "LPR.ModeRandomDescription", ModeRandomPointRoot);
        modes.Add(ModeRandomPoint);

        GameObject ModeInputTextRoot = GameObject.Find("SpecificCoordsInput");
        ModeInputText ModeInputTextPoint = new ModeInputText(this, "LPR.ModeInputTextName", "LPR.ModeInputTextDescription", ModeInputTextRoot);
        modes.Add(ModeInputTextPoint);

        //Map
        cycleMapButton = GameObject.Find("CycleMapButton");
        cycleMapButton.GetComponent<Button>().onClick.AddListener(OnMapButtonClick);
        mapVariants = new List<GameObject> {
            GameObject.Find("MapText"),
            GameObject.Find("MapNoText"),
            GameObject.Find("MapDepth"),
            GameObject.Find("MapXRay")
        };

        //Dynamic General Gameobjects
        playButton = GameObject.Find("StartGameButton").GetComponent<Button>();
        ModeChoiceText = GameObject.Find("ModeChoiceText");
        SelectedPoint = GameObject.Find("SelectedPoint");
        SelectedPointIndicator = GameObject.Find("SelectedPointIndicator");
        mapsBackground = GameObject.Find("MapBackground");
        CoordsDisplay = GameObject.Find("CoordsDisplay");
        ScalePoint1 = GameObject.Find("ScalePoint1");
        ScalePoint2 = GameObject.Find("ScalePoint2");
        BoundBottomLeft = GameObject.Find("BoundBottomLeft");
        BoundTopRight = GameObject.Find("BoundTopRight");
        OptionsPannel = GameObject.Find("OptionsBackGround");
        RightSideInfoText = GameObject.Find("RightSideInfoText").GetComponent<TextMeshProUGUI>();


        //For Language Changes Only
        ModeChoiceTitleText = GameObject.Find("ModeChoice");


        gameObject.SetActive(false);
        OptionsPannel.gameObject.EnsureComponent<OptionsMono>();
        SelectedPointIndicator.GetComponent<Animation>().Play();//start loop

        UpdateCurrentMode(currentModeIndex);
        CycleMaps();
        reloadLanguage();
    }
    public void reloadLanguage()
    {
        ModeChoiceTitleText.GetComponent<TextMeshProUGUI>().text = Language.main.Get("LPR.ModeTitle");
        SettingsButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Language.main.Get("LPR.ConfigButton");
        BackButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Language.main.Get("LPR.BackButton");
        StartGameButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Language.main.Get("LPR.PlayButton");
        CoordsDisplay.GetComponent<TextMeshProUGUI>().text = Language.main.Get("LPR.CoordsDisplay");
        ReloadCurrentModeText(currentModeIndex);
        ReloadAllModesLanguage();
    }
    public void ReloadAllModesLanguage()
    {
        for (int i = 0; i < modes.Count; i++)
        {
            modes[i].ReloadLanguage();
        }
    }
    public void enableUI(GameMode gameMode)
    {
        reloadLanguage();
        selectedGameMode = gameMode;
        gameObject.SetActive(true);
        mapsBackground.SetActive(true);
        OptionsPannel.SetActive(false);
        originalGameCanvas.enabled = false;
    }
    public void UpdateCurrentMode(int modeIndex)
    {
        for(int i = 0; i < modes.Count; i++)
        {
            Mode mode = modes[i];

            bool toggleVal = (i == modeIndex);

            mode.ToggleMode(toggleVal);

            if (toggleVal)
            {
                ModeChoiceText.GetComponent<TextMeshProUGUI>().text = mode.Name();
                RightSideInfoText.text = mode.Description();
            }
        }
    }
    public void ReloadCurrentModeText(int modeIndex)
    {
        for (int i = 0; i < modes.Count; i++)
        {
            if (i == modeIndex)
            {
                Mode mode = modes[i];
                ModeChoiceText.GetComponent<TextMeshProUGUI>().text = mode.Name();
                RightSideInfoText.text = mode.Description();
            }
        }
    }

    public bool CheckValidMousePosition(Vector3 MousePos)
    {
        //check if it is in the bounds
        Vector3 vectorOfRelativePositionToMap = WorldPointToMousePosition(MousePositionToWorldPoint(MousePos));

        RectTransform cycleMapButtonRectTransform= cycleMapButton.GetComponent<RectTransform>();

        //If more things need to be excluded in the future, just make a dynamic system to check an array 
        if ((MousePos.y >= BoundBottomLeft.transform.position.y && MousePos.y <= BoundTopRight.transform.position.y && MousePos.x >= BoundBottomLeft.transform.position.x && MousePos.x <= BoundTopRight.transform.position.x) && (
            //check if the click is on the cycle button
            vectorOfRelativePositionToMap.x <= (cycleMapButtonRectTransform.localPosition.x - (cycleMapButtonRectTransform.rect.width / 2)) ||
            vectorOfRelativePositionToMap.x >= (cycleMapButtonRectTransform.localPosition.x + (cycleMapButtonRectTransform.rect.width / 2)) ||
            vectorOfRelativePositionToMap.y <= (cycleMapButtonRectTransform.localPosition.y - (cycleMapButtonRectTransform.rect.height / 2)) ||
            vectorOfRelativePositionToMap.y >= (cycleMapButtonRectTransform.localPosition.y + (cycleMapButtonRectTransform.rect.height / 2))))
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    public Vector3 WorldPointToMousePosition(Vector3 WorldPoint)
    {
        Vector3 calculatedSelectorLocalPosition = new Vector3(WorldPoint.x * System.Math.Abs(ScalePoint1.transform.localPosition.x) / 1500, 
            WorldPoint.z * System.Math.Abs(ScalePoint2.transform.localPosition.y) / 1500, 0);
        return calculatedSelectorLocalPosition;
    }
    public Vector3 MousePositionToWorldPoint(Vector3 MousePos)
    {
        Vector3 calculatedWorldPoint = new Vector3((MousePos.x - Info.currentRes.width / 2) * (1500 / (Info.currentRes.width / 2 - ScalePoint1.transform.position.x)), 0,
            (MousePos.y - Info.currentRes.height / 2) * (1500 / (Info.currentRes.height / 2 - ScalePoint2.transform.position.y)));

        return calculatedWorldPoint;
    }

    public void MoveSelecedPointFromWorldPoint(Vector3 WorldPoint)
    {
        Vector3 calculatedSelectorLocalPosition = WorldPointToMousePosition(WorldPoint);

        SelectedPoint.GetComponent<RectTransform>().localPosition = calculatedSelectorLocalPosition;
        Info.SelectedSpawn = WorldPoint;
        CoordsDisplay.GetComponent<TextMeshProUGUI>().text = Language.main.Get("LPR.CoordsDisplay") + " " + WorldPoint;

        playButton.interactable = true;//make the start button pressable, once point is selected
        hasSelectedPoint = true;
    }


    void OnSettingsModButtonClick()
    {
        RightSideInfoText.text = Language.main.Get("LPR.OptionsDescription");
        showModSettings = !showModSettings;
        OptionsPannel.SetActive(showModSettings);
        mapsBackground.SetActive(!showModSettings);
        ClickSound.GetComponent<FMOD_StudioEventEmitter>().StartEvent();
    }
    void OnStartGameButtonClick()
    {
        if (hasSelectedPoint)
        {
            ClickSound.GetComponent<FMOD_StudioEventEmitter>().StartEvent();
            Info.showmap = false;
            Info.newSave = true;
            SaveUtils.LoadChachedSettingsToSlotSave();

            CoroutineHost.StartCoroutine(uGUI_MainMenu.main.StartNewGame(selectedGameMode));

            main = null;

            gameObject.SetActive(false);
        }
    }
    void OnBackToMenuButtonClick()
    {
        ClickSound.GetComponent<FMOD_StudioEventEmitter>().StartEvent();
        Info.showmap = false;
        OptionsPannel.SetActive(false);
        gameObject.SetActive(false);

        originalGameCanvas.enabled = true;
    }
    void OnModeChoiceleftClick()
    {
        if (currentModeIndex != 0)
        {
            currentModeIndex--;
            PlayButtonPressSound();
            UpdateCurrentMode(currentModeIndex);
        }
    }
    void OnModeChoiceRightClick()
    {
        if (currentModeIndex != modes.Count - 1)
        {
            currentModeIndex++;
            PlayButtonPressSound();
            UpdateCurrentMode(currentModeIndex);
        }
    }
    void OnMapButtonClick()
    {
        ButtonHoverSharp.GetComponent<FMOD_StudioEventEmitter>().StartEvent();
        if(currentMapSelected < mapVariants.Count)
        {
            currentMapSelected++;
        } else
        {
            currentMapSelected = 0;
        }
        CycleMaps();
    }
    void CycleMaps()
    {
        for (int i = 0; i < mapVariants.Count; i++)
        {
            mapVariants[i].SetActive(i == currentMapSelected);
        }
    }
    public void PlayButtonPressSound()
    {
        ButtonHoverSharp.GetComponent<FMOD_StudioEventEmitter>().StartEvent();
    }

    public void registerPointerDownEvent(Action<PointerEventData> action)
    {
        registeredMouseDownEvents.Add(action);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if(mapsBackground.activeSelf == false)
        {
            return;
        }
        for(int i = 0;i < registeredMouseDownEvents.Count; i++)
        {
            registeredMouseDownEvents[i].Invoke(eventData);
        }
    }
}