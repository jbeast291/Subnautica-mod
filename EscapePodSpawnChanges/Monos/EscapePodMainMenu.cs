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

namespace LifePodRemastered
{
    internal class EscapePodMainMenu : MonoBehaviour
    {
        GameObject HoverSound;
        GameObject ClickSound;
        GameObject ButtonHoverSharp;

        public static AssetBundle assetBundle = Info.assetBundle;

        public static GameObject canvas;

        GameObject AreaSeceltor;

        GameObject Rightside;
        GameObject Primaryoptions;

        GameObject OptionsPannel;

        GameObject ModeChoiceText;
        GameObject ModePresetPoint;
        GameObject RandomizePointButton;
        GameObject PresetText;
        GameObject SpecificCoordsInput;

        GameObject Map;
        GameObject CoordsDisplay;
        GameObject ScalePoint1;
        GameObject ScalePoint2;
        GameObject BoundBottomLeft;
        GameObject BoundTopRight;
        GameObject SelectedPoint;
        GameObject SelectedPointIndicator;

        GameObject SpecificPointInfo;
        GameObject PresetPointInfo;
        GameObject RandomPointInfo;
        GameObject InputCoordsInfo;
        GameObject SettingsInfo;

        Camera cam;

        int CurrentMode = 1;
        int CurrentPreset = 1;

        bool AnimationActive = false;

        List<string[]> presetList;

        Vector3 vector3 = new Vector3(10000, 10000, 10000);

        public void Awake()
        {
            presetList = Util.ReadPresetsFromModFolder();

            Rightside = GameObject.Find("RightSide");
            Primaryoptions = GameObject.Find("PrimaryOptions");

            canvas = GameObject.Find("Menu canvas");

            AreaSeceltor = Instantiate(assetBundle.LoadAsset<GameObject>("AreaSelector"));
            AreaSeceltor.transform.parent = canvas.transform;
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

            

            ModePresetPoint = GameObject.Find("ModePresetPoint");
            RandomizePointButton = GameObject.Find("RandomizePointButton");
            ModeChoiceText = GameObject.Find("ModeChoiceText");
            SelectedPoint = GameObject.Find("SelectedPoint");
            SelectedPointIndicator = GameObject.Find("SelectedPointIndicator");
            PresetText = GameObject.Find("PresetText");
            SpecificCoordsInput = GameObject.Find("SpecificCoordsInput");

            Map = GameObject.Find("Map");
            CoordsDisplay = GameObject.Find("CoordsDisplay");
            ScalePoint1 = GameObject.Find("ScalePoint1");
            ScalePoint2 = GameObject.Find("ScalePoint2");
            BoundBottomLeft = GameObject.Find("BoundBottomLeft");
            BoundTopRight = GameObject.Find("BoundTopRight");
            OptionsPannel = GameObject.Find("OptionsBackGround");

            SpecificPointInfo = GameObject.Find("SpecificPointInfo");
            PresetPointInfo = GameObject.Find("PresetPointInfo");
            RandomPointInfo = GameObject.Find("RandomPointInfo");
            InputCoordsInfo = GameObject.Find("InputCoordsInfo");
            SettingsInfo = GameObject.Find("SettingsInfo");

            cam = GameObject.Find("UI Camera").GetComponent<Camera>();

            AreaSeceltor.SetActive(false);
            OptionsPannel.SetActive(false);
            SpecificPointInfo.SetActive(false);
            PresetPointInfo.SetActive(false);
            RandomPointInfo.SetActive(false);
            InputCoordsInfo.SetActive(false);
            SettingsInfo.SetActive(false);

            OptionsPannel.gameObject.EnsureComponent<OptionsMono>();
            SelectedPointIndicator.GetComponent<Animation>().Play();//start loop
        }

        public void Update()
        {
            if (Info.showmap && !Info.Showsettings)
            {
                ManageRightSide(CurrentMode, Info.Showsettings);
                ManageLeftSide(CurrentMode, CurrentPreset);

                AreaSeceltor.SetActive(true);
                Map.SetActive(true);
                OptionsPannel.SetActive(false);
                Rightside.SetActive(false);
                Primaryoptions.SetActive(false);
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

                        if (Input.GetMouseButtonDown(0) && CheckValidMousePosition(Input.mousePosition) == 1)
                        {
                            MousePositionToSelectedPoint(Input.mousePosition);
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
                        WorldPointtoMoveSelecedPoint(StringToVector3(selectedPreset[1]));

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

            if (!ShowSettings)//settings info off
            {
                switch (CurrentMode)
                {
                    case 1:
                        {
                            SpecificPointInfo.SetActive(true);
                            PresetPointInfo.SetActive(false);
                            SettingsInfo.SetActive(false);
                            break;
                        }
                    case 2:
                        {
                            SpecificPointInfo.SetActive(false);
                            PresetPointInfo.SetActive(true);
                            RandomPointInfo.SetActive(false);
                            SettingsInfo.SetActive(false);
                            break;
                        }
                    case 3:
                        {
                            PresetPointInfo.SetActive(false);
                            RandomPointInfo.SetActive(true);
                            InputCoordsInfo.SetActive(false);
                            SettingsInfo.SetActive(false);
                            break;
                        }
                    case 4:
                        {
                            RandomPointInfo.SetActive(false);
                            InputCoordsInfo.SetActive(true);
                            SettingsInfo.SetActive(false);
                            break;
                        }
                }
            }
            else//settings
            {
                SpecificPointInfo.SetActive(false);
                PresetPointInfo.SetActive(false);
                RandomPointInfo.SetActive(false);
                InputCoordsInfo.SetActive(false);
                SettingsInfo.SetActive(true);
            }

        }

        public static Vector3 StringToVector3(string sVector) // I may or may not have "borrrowed" this code
        {
            // Remove the parentheses
            if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            {
                sVector = sVector.Substring(1, sVector.Length - 2);
            }

            // split the items
            string[] sArray = sVector.Split(',');

            // store as a Vector3
            Vector3 result = new Vector3(float.Parse(sArray[0]), float.Parse(sArray[1]), float.Parse(sArray[2]));

            return result;
        }

        public float CheckValidMousePosition(Vector3 MousePos)
        {
            if ((MousePos.y >= cam.WorldToScreenPoint(BoundBottomLeft.transform.position).y && MousePos.y <= cam.WorldToScreenPoint(BoundTopRight.transform.position).y && (MousePos.x >= cam.WorldToScreenPoint(BoundBottomLeft.transform.position).x && MousePos.x <= cam.WorldToScreenPoint(BoundTopRight.transform.position).x)))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        public void WorldPointtoMoveSelecedPoint(Vector3 WorldPoint)
        {
            float diff = Info.currentRes.width / 2560f;
            vector3 = new Vector3((WorldPoint.x / (3.37f / diff)) + (1280 * diff), (WorldPoint.z / (3.37f / diff)) + (720 * diff));
            SelectedPoint.GetComponent<RectTransform>().localPosition = new Vector3((vector3.x - (1280f * diff)) / (1250f * diff), (vector3.y - (720f * diff)) / (1250f * diff), 0);
            Info.SelectedSpawn = WorldPoint;
            CoordsDisplay.GetComponent<TextMeshProUGUI>().text = "Coords: " + WorldPoint;
        }
        public void MousePositionToSelectedPoint(Vector3 MousePos)
        {
            vector3 = new Vector3((MousePos.x - Info.currentRes.width / 2) * (1500 / (Info.currentRes.width / 2 - cam.WorldToScreenPoint(ScalePoint1.transform.position).x)), 0, (MousePos.y - Info.currentRes.height / 2) * (1500 / (Info.currentRes.height / 2 - cam.WorldToScreenPoint(ScalePoint2.transform.position).y)));
            Info.SelectedSpawn = vector3;
            CoordsDisplay.GetComponent<TextMeshProUGUI>().text = "Coords: " + vector3;

            float Scaler = 0.000238f;
            Vector3 SELECTOR = new Vector3(vector3.x * Scaler, vector3.z * Scaler, 0);
            SelectedPoint.GetComponent<RectTransform>().localPosition = SELECTOR;
            ButtonHoverSharp.GetComponent<FMOD_StudioEventEmitter>().StartEvent();
        }



        void OnSettingsModButtonClick()
        {
            Info.Showsettings = !Info.Showsettings;
            OptionsPannel.SetActive(true);
            Map.SetActive(false);
            ManageRightSide(CurrentMode, Info.Showsettings);
            ClickSound.GetComponent<FMOD_StudioEventEmitter>().StartEvent();
            
        }
        void OnStartGameButtonClick()
        {
            if (vector3 != new Vector3(10000, 10000, 10000))
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
            Info.OverideSpawnHeight = true;
            WorldPointtoMoveSelecedPoint(Info.SelectedSpawn);
            ButtonHoverSharp.GetComponent<FMOD_StudioEventEmitter>().StartEvent();

            ManageRightSide(CurrentMode, Info.Showsettings);
            ManageLeftSide(CurrentMode, CurrentPreset);
        }
        void OnRandomizePointButtonClick()
        {
            Info.OverideSpawnHeight = false;
            Vector3 ranvector3 = new Vector3(Random.Range(cam.WorldToScreenPoint(BoundBottomLeft.transform.position).x, cam.WorldToScreenPoint(BoundTopRight.transform.position).x), Random.Range(cam.WorldToScreenPoint(BoundBottomLeft.transform.position).y, cam.WorldToScreenPoint(BoundTopRight.transform.position).y), 0);

            MousePositionToSelectedPoint(ranvector3);
            ButtonHoverSharp.GetComponent<FMOD_StudioEventEmitter>().StartEvent();

            ManageRightSide(CurrentMode, Info.Showsettings);
            ManageLeftSide(CurrentMode, CurrentPreset);
        }
    }
}