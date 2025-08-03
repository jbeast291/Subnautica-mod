using FMOD;
using HarmonyLib;
using LifePodRemastered.Monos;
using LifePodRemastered.presetSystem;
using Nautilus.Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UWE;
using static GameAnalytics;
using static LifePodRemastered.SaveUtils;
using static UnityEngine.UI.Selectable;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace LifePodRemastered;

public class EscapePodMainMenu : MonoBehaviour
{
    public static EscapePodMainMenu main;
    public static AssetBundle assetBundle = LPRGlobals.assetBundle;

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
    GameObject ModeChoiceLeft;
    GameObject ModeChoiceRight;

    TextMeshProUGUI RightSideInfoText;

    GameObject ModeChoiceTitleText;
    GameObject BackButton;
    GameObject SettingsButton;
    GameObject MapTypeButton;
    GameObject StartGameButton;


    public int currentModeIndex = 0;

    Button playButton;
    public MapDisplayType activeMapDisplayType = MapDisplayType.ThreeDimensional;

    GameObject cycleMapButton;
    int currentMapSelected = 0;
    List<GameObject> mapVariants;

    public List<Mode> modes;
    public List<Action> registeredMouseDownEvents;

    bool showModSettings = false;

    public void Awake()
    {
        if (main != null)
        {
            UnityEngine.Debug.LogError($"Duplicate {this.GetType().Name} found!");
            Destroy(this);
            return;
        }
        main = this;

        originalGameCanvas = GameObject.Find("Menu canvas").GetComponent<Canvas>();
        registeredMouseDownEvents = new List<Action>();//just init
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
        MapTypeButton = GameObject.Find("MapTypeButton");
        MapTypeButton.GetComponent<Button>().onClick.AddListener(OnMapTypeButtonClick);
        StartGameButton = GameObject.Find("StartGameButton");
        StartGameButton.GetComponent<Button>().onClick.AddListener(OnStartGameButtonClick);
        BackButton = GameObject.Find("BackToMenuButton");
        BackButton.GetComponent<Button>().onClick.AddListener(OnBackToMenuButtonClick);
        ModeChoiceLeft = GameObject.Find("ModeChoiceleft");
        ModeChoiceLeft.GetComponent<Button>().onClick.AddListener(OnModeChoiceleftClick);
        ModeChoiceRight = GameObject.Find("ModeChoiceRight");
        ModeChoiceRight.GetComponent<Button>().onClick.AddListener(OnModeChoiceRightClick);

        //Modes
        modes = new List<Mode>();
        GameObject YLevelNoticeBackground = GameObject.Find("YLevelNoticeBackground");
        ModeSpecific ModeSpecificPoint = new ModeSpecific(this, "LPR.ModeSpecificName", "OVERRIDEN, Check ModeSpecific.cs", YLevelNoticeBackground);
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

        MiniWorldController.instantiateMiniWorldMainMenu();

        UpdateCurrentMode(currentModeIndex);
        CycleMaps();
        reloadLanguage();
    }
    public void reloadLanguage()
    {
        ModeChoiceTitleText.GetComponent<TextMeshProUGUI>().text = Language.main.Get("LPR.ModeTitle");
        SettingsButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Language.main.Get("LPR.ConfigButton");
        MapTypeButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Language.main.Get("LPR.MapTypeToggleButton");
        BackButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Language.main.Get("LPR.BackButton");
        StartGameButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Language.main.Get("LPR.PlayButton");
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
        showModSettings = false;
        ResetInteractables();
        reloadLanguage();
        selectedGameMode = gameMode;
        gameObject.SetActive(true);
        mapsBackground.SetActive(false);
        OptionsPannel.SetActive(false);
        originalGameCanvas.enabled = false;
        UpdateCoordsDisplay();
        if (activeMapDisplayType == MapDisplayType.ThreeDimensional)
        {
            MiniWorldController.main.ShowMiniworld();
        }
    }
    public void UpdateCurrentMode(int modeIndex)
    {
        for (int i = 0; i < modes.Count; i++)
        {
            Mode mode = modes[i];

            bool isEnabled = (i == modeIndex);

            mode.ToggleMode(isEnabled);

            if (isEnabled)
            {
                ModeChoiceText.GetComponent<TextMeshProUGUI>().text = mode.Name();
                RightSideInfoText.text = mode.Description();
            }
        }
    }
    public void ToggleCurrentModeInteraction(int modeIndex, bool toggleVal)
    {
        for (int i = 0; i < modes.Count; i++)
        {
            if (i == modeIndex)
            {
                Mode mode = modes[i];
                mode.EnableInteraction(toggleVal);
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
    public Mode GetCurrentMode()
    {
        return modes[currentModeIndex];
    }

    public Mode GetRegisteredModeByType(Type type)
    {
        for(int i = 0; i < modes.Count; i++)
        {
            if (modes[i].GetType() == type)
            {
                return modes[i];
            }
        }
        return null;
    }

    public bool CheckValidMousePosition(Vector3 MousePos)
    {
        //check if it is in the bounds
        Vector3 vectorOfRelativePositionToMap = WorldPointToMousePosition(MousePositionToWorldPoint(MousePos));

        RectTransform cycleMapButtonRectTransform = cycleMapButton.GetComponent<RectTransform>();

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
        Vector3 calculatedWorldPoint = new Vector3((MousePos.x - LPRGlobals.currentRes.width / 2) * (1500 / (LPRGlobals.currentRes.width / 2 - ScalePoint1.transform.position.x)), 0,
            (MousePos.y - LPRGlobals.currentRes.height / 2) * (1500 / (LPRGlobals.currentRes.height / 2 - ScalePoint2.transform.position.y)));

        return calculatedWorldPoint;
    }

    public void MoveSelecedPointFromWorldPoint(Vector3 WorldPoint)
    {
        Vector3 calculatedSelectorLocalPosition = WorldPointToMousePosition(WorldPoint);

        SelectedPoint.GetComponent<RectTransform>().localPosition = calculatedSelectorLocalPosition;
        LPRGlobals.SelectedSpawn = WorldPoint;
        UpdateCoordsDisplay();
    }
    public void UpdateCoordsDisplay()
    {
        CoordsDisplay.GetComponent<TextMeshProUGUI>().text =
            $"({LPRGlobals.SelectedSpawn.x.ToString("F2")}, " +
            $"{LPRGlobals.SelectedSpawn.y.ToString("F2")}, " +
            $"{LPRGlobals.SelectedSpawn.z.ToString("F2")})";
    }
    void ResetInteractables()
    {
        ToggleCurrentModeInteraction(currentModeIndex, true);

        MapTypeButton.GetComponent<Button>().interactable = true;
        ModeChoiceLeft.GetComponent<Button>().interactable = true;
        ModeChoiceRight.GetComponent<Button>().interactable = true;
    }

    void OnSettingsModButtonClick()
    {
        RightSideInfoText.text = Language.main.Get("LPR.OptionsDescription");
        showModSettings = !showModSettings;
        OptionsPannel.SetActive(showModSettings);
        ClickSound.GetComponent<FMOD_StudioEventEmitter>().StartEvent();

        ToggleCurrentModeInteraction(currentModeIndex, !showModSettings);

        MapTypeButton.GetComponent<Button>().interactable = !showModSettings;
        ModeChoiceLeft.GetComponent<Button>().interactable = !showModSettings;
        ModeChoiceRight.GetComponent<Button>().interactable = !showModSettings;

        if (activeMapDisplayType == MapDisplayType.TwoDimensional)
        {
            mapsBackground.SetActive(!showModSettings);
        }
        if (!showModSettings)
        {
            ReloadCurrentModeText(currentModeIndex);
        }
    }
    void OnMapTypeButtonClick()
    {
        if (showModSettings)
        {
            return;
        }
        if (activeMapDisplayType == MapDisplayType.ThreeDimensional)
        {
            MiniWorldController.main.HideMiniworld();
            mapsBackground.SetActive(true);
            activeMapDisplayType = MapDisplayType.TwoDimensional;
        }
        else if (activeMapDisplayType == MapDisplayType.TwoDimensional)
        {
            MiniWorldController.main.ShowMiniworld();
            mapsBackground.SetActive(false);
            activeMapDisplayType = MapDisplayType.ThreeDimensional;
        }
        ReloadCurrentModeText(currentModeIndex);
        MoveSelecedPointFromWorldPoint(LPRGlobals.SelectedSpawn);//update the red reticle as the 3d move system wont
    }

    void OnStartGameButtonClick()
    {
        ClickSound.GetComponent<FMOD_StudioEventEmitter>().StartEvent();
        LPRGlobals.newSave = true;
        SaveUtils.LoadChachedSettingsToSlotSave();
        CoroutineHost.StartCoroutine(uGUI_MainMenu.main.StartNewGame(selectedGameMode));

        main = null;

        gameObject.SetActive(false);
    }
    void OnBackToMenuButtonClick()
    {
        ClickSound.GetComponent<FMOD_StudioEventEmitter>().StartEvent();
        OptionsPannel.SetActive(false);
        gameObject.SetActive(false);
        MiniWorldController.main.HideMiniworld();
        originalGameCanvas.enabled = true;
    }
    void OnModeChoiceleftClick()
    {
        if (currentModeIndex != 0)
        {
            currentModeIndex--;

        }
        else
        {
            currentModeIndex = modes.Count - 1;
        }
        PlayButtonPressSound();
        UpdateCurrentMode(currentModeIndex);
    }
    void OnModeChoiceRightClick()
    {
        if (currentModeIndex != modes.Count - 1)
        {
            currentModeIndex++;
        }
        else
        {
            currentModeIndex = 0;
        }
        PlayButtonPressSound();
        UpdateCurrentMode(currentModeIndex);
    }
    void OnMapButtonClick()
    {
        ButtonHoverSharp.GetComponent<FMOD_StudioEventEmitter>().StartEvent();
        if (currentMapSelected < mapVariants.Count - 1)
        {
            currentMapSelected++;
        }
        else
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

    public void registerPointerDownEvent(Action action)
    {
        registeredMouseDownEvents.Add(action);
    }

    public void Update()
    {
        if (GameInput.GetButtonDown(GameInput.Button.LeftHand) || GameInput.GetButtonDown(GameInput.Button.RightHand))
        {
            if (showModSettings)
            {
                return;
            }
            for (int i = 0; i < registeredMouseDownEvents.Count; i++)
            {
                registeredMouseDownEvents[i].Invoke();
            }
        }
    }
    public void ResetCurrentModeToDefault()
    {
        currentModeIndex = 0;
        UpdateCurrentMode(currentModeIndex);
    }
}

public enum MapDisplayType
{
    TwoDimensional,
    ThreeDimensional//,
    //FiveDChess
}