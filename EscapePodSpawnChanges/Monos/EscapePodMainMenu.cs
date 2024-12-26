using HarmonyLib;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UWE;
using TMPro;
using static LifePodRemastered.SaveUtils;
using FMOD;
using UnityEngine.SceneManagement;

namespace LifePodRemastered
{
    internal class EscapePodMainMenu : MonoBehaviour
    {
        GameObject HoverSound;
        GameObject ClickSound;
        GameObject ButtonHoverSharp;

        public static AssetBundle assetBundle = Info.assetBundle;

        Canvas originalGameCanvas;

        GameObject AreaSeceltor;

        GameObject Rightside;
        GameObject Primaryoptions;

        GameObject OptionsPannel;

        GameObject ModeChoiceText;
        GameObject ModePresetPoint;
        GameObject RandomizePointButton;
        GameObject PresetText;
        GameObject SpecificCoordsInput;

        GameObject mapsBackground;
        GameObject CoordsDisplay;
        GameObject ScalePoint1;
        GameObject ScalePoint2;
        GameObject BoundBottomLeft;
        GameObject BoundTopRight;
        GameObject SelectedPoint;
        GameObject SelectedPointIndicator;

        TextMeshProUGUI RightSideInfoText;

        int CurrentMode = 1;
        int CurrentPreset = 1;

        //Used in map cycle
        int currentMapSelected = 1;
        GameObject mapText;
        GameObject mapNoText;
        GameObject mapDepth;
        GameObject mapXRay;

        GameObject cycleMapButton;

        bool AnimationActive = false;

        List<string[]> presetList;

        bool hasSelectedPoint = false;
        Button playButton;

        public void Awake()
        {
            presetList = Util.ReadPresetsFromModFolder();

            Rightside = GameObject.Find("RightSide");
            Primaryoptions = GameObject.Find("PrimaryOptions");

            originalGameCanvas = GameObject.Find("Menu canvas").GetComponent<Canvas>();


            AreaSeceltor = Instantiate(assetBundle.LoadAsset<GameObject>("LifePodRemasteredCanvas"));
            SceneManager.MoveGameObjectToScene(AreaSeceltor, SceneManager.GetSceneByName("XMenu"));
            AreaSeceltor.transform.localPosition = new Vector3(0, 0, 7500);
            AreaSeceltor.SetActive(true);
        }
        public void Start()
        {

            HoverSound = GameObject.Find("ButtonHover");
            ClickSound = GameObject.Find("ButtonClick");
            ButtonHoverSharp = GameObject.Find("ButtonHoverSharp");

            GameObject.Find("SettingModsButton").GetComponent<Button>().onClick.AddListener(OnSettingsModButtonClick);
            GameObject.Find("StartGameButton").GetComponent<Button>().onClick.AddListener(OnStartGameButtonClick);
            GameObject.Find("BackToMenuButton").GetComponent<Button>().onClick.AddListener(OnBackToMenuButtonClick);
            GameObject.Find("ModeChoiceleft").GetComponent<Button>().onClick.AddListener(OnModeChoiceleftClick);
            GameObject.Find("ModeChoiceRight").GetComponent<Button>().onClick.AddListener(OnModeChoiceRightClick);
            GameObject.Find("ModePresetPointChoiceleft").GetComponent<Button>().onClick.AddListener(OnModePresetPointChoiceleftClick);
            GameObject.Find("ModePresetPointChoiceRight").GetComponent<Button>().onClick.AddListener(OnModePresetPointChoiceRightClick);
            GameObject.Find("RandomizePointButton").GetComponent<Button>().onClick.AddListener(OnRandomizePointButtonClick);
            GameObject.Find("SpecificCoordsInput").GetComponent<TMP_InputField>().onEndEdit.AddListener(OnEndInputFieldEdit);

            cycleMapButton = GameObject.Find("CycleMapButton");
            cycleMapButton.GetComponent<Button>().onClick.AddListener(OnCycleButtonClick);


            playButton = GameObject.Find("StartGameButton").GetComponent<Button>();

            ModePresetPoint = GameObject.Find("ModePresetPoint");
            RandomizePointButton = GameObject.Find("RandomizePointButton");
            ModeChoiceText = GameObject.Find("ModeChoiceText");
            SelectedPoint = GameObject.Find("SelectedPoint");
            SelectedPointIndicator = GameObject.Find("SelectedPointIndicator");
            PresetText = GameObject.Find("PresetText");
            SpecificCoordsInput = GameObject.Find("SpecificCoordsInput");

            mapsBackground = GameObject.Find("MapBackground");
            CoordsDisplay = GameObject.Find("CoordsDisplay");
            ScalePoint1 = GameObject.Find("ScalePoint1");
            ScalePoint2 = GameObject.Find("ScalePoint2");
            BoundBottomLeft = GameObject.Find("BoundBottomLeft");
            BoundTopRight = GameObject.Find("BoundTopRight");
            OptionsPannel = GameObject.Find("OptionsBackGround");

            RightSideInfoText = GameObject.Find("RightSideInfoText").GetComponent<TextMeshProUGUI>();

            //cycle
            mapText = GameObject.Find("MapText");
            mapNoText = GameObject.Find("MapNoText");
            mapDepth = GameObject.Find("MapDepth");
            mapXRay = GameObject.Find("MapXRay");

            AreaSeceltor.SetActive(false);
            OptionsPannel.SetActive(false);

            OptionsPannel.gameObject.EnsureComponent<OptionsMono>();
            SelectedPointIndicator.GetComponent<Animation>().Play();//start loop

            cycleMaps();
        }

        public void Update()
        {
            if (Info.showmap && !Info.Showsettings)
            {
                ManageRightSide(CurrentMode, Info.Showsettings);
                ManageLeftSide(CurrentMode, CurrentPreset);

                AreaSeceltor.SetActive(true);
                mapsBackground.SetActive(true);
                OptionsPannel.SetActive(false);
                Rightside.SetActive(false);
                Primaryoptions.SetActive(false);

                originalGameCanvas.enabled = false;
            }
        }
        public void ManageLeftSide(int Mode, int presetNumber)
        {
            switch (Mode)
            {
                case 1:
                    {
                        ModeChoiceText.GetComponent<TextMeshProUGUI>().text = "Specific Point";

                        RandomizePointButton.SetActive(false);
                        ModePresetPoint.SetActive(false);
                        SpecificCoordsInput.SetActive(false);

                        if (Input.GetMouseButtonDown(0) && CheckValidMousePosition(Input.mousePosition))
                        {
                            MoveSelecedPointFromWorldPoint(MousePositionToWorldPoint(Input.mousePosition));
                            ButtonHoverSharp.GetComponent<FMOD_StudioEventEmitter>().StartEvent();
                        }
                        break;
                    }
                case 2:
                    {
                        ModeChoiceText.GetComponent<TextMeshProUGUI>().text = "Preset Point";
                        RandomizePointButton.SetActive(false);
                        ModePresetPoint.SetActive(true);

                        TextMeshProUGUI PresetTextTEXT = PresetText.GetComponent<TextMeshProUGUI>();

                        string[] selectedPreset = presetList[presetNumber - 1];

                        PresetTextTEXT.text = selectedPreset[0];
                        MoveSelecedPointFromWorldPoint(StringToVector3(selectedPreset[1]));

                        break;
                    }
                case 3:
                    {
                        ModeChoiceText.GetComponent<TextMeshProUGUI>().text = "Random Point";
                        RandomizePointButton.SetActive(true);
                        ModePresetPoint.SetActive(false);
                        SpecificCoordsInput.SetActive(false);
                        break;
                    }
                case 4:
                    {
                        ModeChoiceText.GetComponent<TextMeshProUGUI>().text = "Type Coords";
                        RandomizePointButton.SetActive(false);
                        SpecificCoordsInput.SetActive(true);
                        break;
                    }
            }
        }
        public void ManageRightSide(int Mode, bool ShowSettings)
        {

            if (!ShowSettings)//map 
            {
                switch (CurrentMode)
                {
                    case 1: { RightSideInfoText.text = "Click a position on the map to set were the pod spawns.\r\n\r\n\r\nPress play to \r\nstart!"; break; }
                    case 2: { RightSideInfoText.text = "Choose from one of the preset points on the left\r\n\r\n\r\nPress play to \r\nstart!"; break; }
                    case 3: { RightSideInfoText.text = "Press the button \"Randomize Point\" for a randompoint (can be in void so be carefull)\r\n\r\nPress play to \r\nstart!"; break; }
                    case 4: { RightSideInfoText.text = "For more advanced users type your desired coords in manually\r\n\r\n\r\nPress play to \r\nstart!"; break; }
                }
            }
            else//settings
            {
                RightSideInfoText.text = "Modify experimental spawning properties or post-intro settings!\r\n\r\nPress play to \r\nstart!";
            }

        }

        public static Vector3 StringToVector3(string sVector)
        {
            //Remove parentheses
            if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            {
                sVector = sVector.Substring(1, sVector.Length - 2);
            }

            //split xyz
            string[] sArray = sVector.Split(',');

            //store as Vector3
            Vector3 result = new Vector3(float.Parse(sArray[0]), float.Parse(sArray[1]), float.Parse(sArray[2]));

            return result;
        }

        public bool CheckValidMousePosition(Vector3 MousePos)
        {
            //check if it is in the bounds
            Vector3 vectorOfRelativePositionToMap = WorldPointToLocalOfMap(MousePositionToWorldPoint(MousePos));
            if ((MousePos.y >= BoundBottomLeft.transform.position.y && MousePos.y <= BoundTopRight.transform.position.y && MousePos.x >= BoundBottomLeft.transform.position.x && MousePos.x <= BoundTopRight.transform.position.x) && (
                //check if the click is on the cycle button
                vectorOfRelativePositionToMap.x <= (cycleMapButton.GetComponent<RectTransform>().localPosition.x - (cycleMapButton.GetComponent<RectTransform>().rect.width / 2)) ||
                vectorOfRelativePositionToMap.x >= (cycleMapButton.GetComponent<RectTransform>().localPosition.x + (cycleMapButton.GetComponent<RectTransform>().rect.width / 2)) ||
                vectorOfRelativePositionToMap.y <= (cycleMapButton.GetComponent<RectTransform>().localPosition.y - (cycleMapButton.GetComponent<RectTransform>().rect.height / 2)) ||
                vectorOfRelativePositionToMap.y >= (cycleMapButton.GetComponent<RectTransform>().localPosition.y + (cycleMapButton.GetComponent<RectTransform>().rect.height / 2))))
            {
                return true;
            }
            else
            {
                return false;
            }
           
        }


        public Vector3 WorldPointToLocalOfMap(Vector3 WorldPoint)
        {
            Vector3 calculatedSelectorLocalPosition = new Vector3(WorldPoint.x * System.Math.Abs(ScalePoint1.transform.localPosition.x) / 1500, 
                WorldPoint.z * System.Math.Abs(ScalePoint2.transform.localPosition.y) / 1500, 0);
            return calculatedSelectorLocalPosition;
        }
        public Vector3 MousePositionToWorldPoint(Vector3 MousePos)
        {
            //calculate the world point based on the mouse point
            Vector3 calculatedWorldPoint = new Vector3((MousePos.x - Info.currentRes.width / 2) * (1500 / (Info.currentRes.width / 2 - ScalePoint1.transform.position.x)), 0,
                (MousePos.y - Info.currentRes.height / 2) * (1500 / (Info.currentRes.height / 2 - ScalePoint2.transform.position.y)));

            return calculatedWorldPoint;
        }



        public void MoveSelecedPointFromWorldPoint(Vector3 WorldPoint)
        {
            Vector3 calculatedSelectorLocalPosition = WorldPointToLocalOfMap(WorldPoint);

            SelectedPoint.GetComponent<RectTransform>().localPosition = calculatedSelectorLocalPosition;
            Info.SelectedSpawn = WorldPoint;
            CoordsDisplay.GetComponent<TextMeshProUGUI>().text = "Coords: " + WorldPoint;

            playButton.interactable = true;//make the start button pressable, once point is selected
            hasSelectedPoint = true;
        }
        void OnSettingsModButtonClick()
        {
            Info.Showsettings = !Info.Showsettings;
            OptionsPannel.SetActive(Info.Showsettings);
            mapsBackground.SetActive(!Info.Showsettings);
            ManageRightSide(CurrentMode, Info.Showsettings);
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

                switch (Info.GameMode) // 1 survival, 2 creative, 3, freedom, 4 hardcore
                {
                    case 1: { CoroutineHost.StartCoroutine(uGUI_MainMenu.main.StartNewGame(GameMode.Survival)); break; }
                    case 2: { CoroutineHost.StartCoroutine(uGUI_MainMenu.main.StartNewGame(GameMode.Creative)); break; }
                    case 3: { CoroutineHost.StartCoroutine(uGUI_MainMenu.main.StartNewGame(GameMode.Freedom));  break; }
                    case 4: { CoroutineHost.StartCoroutine(uGUI_MainMenu.main.StartNewGame(GameMode.Hardcore)); break; }
                }
            }
        }
        void OnBackToMenuButtonClick()
        {
            ClickSound.GetComponent<FMOD_StudioEventEmitter>().StartEvent();
            Info.showmap = false;
            Info.Showsettings = false;
            OptionsPannel.SetActive(false);
            AreaSeceltor.SetActive(false);
            Rightside.SetActive(true);
            Primaryoptions.SetActive(true);

            originalGameCanvas.enabled = true;
        }
        void OnModeChoiceleftClick()
        {
            Info.OverideSpawnHeight = false;
            if (CurrentMode != 1)
            {
                CurrentMode--;
                HoverSound.GetComponent<FMOD_StudioEventEmitter>().StartEvent();
                Info.OverideSpawnHeight = false;
            }
            ManageRightSide(CurrentMode, Info.Showsettings);
            ManageLeftSide(CurrentMode, CurrentPreset);
        }
        void OnModeChoiceRightClick()
        {
            if (CurrentMode != 4)
            {
                CurrentMode++;
                HoverSound.GetComponent<FMOD_StudioEventEmitter>().StartEvent();
                Info.OverideSpawnHeight = false;
            }
            ManageRightSide(CurrentMode, Info.Showsettings);
            ManageLeftSide(CurrentMode, CurrentPreset);
        }
        void OnModePresetPointChoiceleftClick()
        {
            if (CurrentPreset != 1)
            {
                CurrentPreset--;
                ButtonHoverSharp.GetComponent<FMOD_StudioEventEmitter>().StartEvent();
            }
            ManageRightSide(CurrentMode, Info.Showsettings);
            ManageLeftSide(CurrentMode, CurrentPreset);
        }
        void OnModePresetPointChoiceRightClick()
        {
            if (CurrentPreset != presetList.Count) 
            {
                CurrentPreset++;
                ButtonHoverSharp.GetComponent<FMOD_StudioEventEmitter>().StartEvent();
            }
            ManageRightSide(CurrentMode, Info.Showsettings);
            ManageLeftSide(CurrentMode, CurrentPreset);
        }
        void OnEndInputFieldEdit(string s)
        {
            Info.SelectedSpawn = StringToVector3(s);
            playButton.interactable = true;//make the start button pressable, once point is selected
            Info.OverideSpawnHeight = true;
            MoveSelecedPointFromWorldPoint(Info.SelectedSpawn);
            ButtonHoverSharp.GetComponent<FMOD_StudioEventEmitter>().StartEvent();

            ManageRightSide(CurrentMode, Info.Showsettings);
            ManageLeftSide(CurrentMode, CurrentPreset);
        }
        void OnRandomizePointButtonClick()
        {
            Info.OverideSpawnHeight = false;
            Vector3 ranvector3 = new Vector3(Random.Range(BoundBottomLeft.transform.position.x, BoundTopRight.transform.position.x), 
                Random.Range(BoundBottomLeft.transform.position.y, BoundTopRight.transform.position.y), 0);

            MoveSelecedPointFromWorldPoint(MousePositionToWorldPoint(ranvector3));

            ButtonHoverSharp.GetComponent<FMOD_StudioEventEmitter>().StartEvent();

            ManageRightSide(CurrentMode, Info.Showsettings);
            ManageLeftSide(CurrentMode, CurrentPreset);
        }
        void OnCycleButtonClick()
        {
            ButtonHoverSharp.GetComponent<FMOD_StudioEventEmitter>().StartEvent();
            if(currentMapSelected < 4)
            {
                currentMapSelected++;
            } else
            {
                currentMapSelected = 1;
            }
            cycleMaps();
        }
        void cycleMaps()
        {
            switch (currentMapSelected)
            {
                case 1:
                    {
                        mapText.SetActive(true);
                        mapNoText.SetActive(false);
                        mapDepth.SetActive(false);
                        mapXRay.SetActive(false);
                        break;
                    }
                case 2:
                    {
                        mapText.SetActive(false);
                        mapNoText.SetActive(true);
                        mapDepth.SetActive(false);
                        mapXRay.SetActive(false);
                        break;
                    }
                case 3:
                    {
                        mapText.SetActive(false);
                        mapNoText.SetActive(false);
                        mapDepth.SetActive(true);
                        mapXRay.SetActive(false);
                        break;
                    }
                case 4:
                    {
                        mapText.SetActive(false);
                        mapNoText.SetActive(false);
                        mapDepth.SetActive(false);
                        mapXRay.SetActive(true);
                        break;
                    }
            }
        }
    }
}