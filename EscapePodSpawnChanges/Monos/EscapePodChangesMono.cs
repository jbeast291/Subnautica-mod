using HarmonyLib;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UWE;
using SMLHelper.V2.Utility;
using TMPro;

namespace LifePodRemastered
{
    internal class LifePodMapFunction : MonoBehaviour
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
        //GameObject OptionsPannelBackButton;

        GameObject ModeChoiceText;
        GameObject ModePresetPoint;
        GameObject RandomizePointButton;
        GameObject PresetText;
        GameObject SpecificCoordsInput;

        GameObject Map;
        GameObject SelectedPoint;
        GameObject SelectedPointIndicator;
        GameObject OptionsBackGround;

        GameObject SpecificPointInfo;
        GameObject PresetPointInfo;
        GameObject RandomPointInfo;
        GameObject InputCoordsInfo;
        GameObject SettingsInfo;

        int CurrentMode = 1;
        int CurrentPreset = 1;

        bool AnimationActive = false;

        //bool PointChanged = false;
        //bool startsequence = false;
        //bool Scaling = false;
        //bool makebig = false;
        //bool makesmall = false;


        Vector3 vector3 = new Vector3(10000, 10000, 10000);

        //ToFile ToFileInstance = new ToFile();
        public void Awake()
        {
            //ToFileInstance.CreateJson();

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
            OptionsPannel = GameObject.Find("OptionsBackGround");

            SpecificPointInfo = GameObject.Find("SpecificPointInfo");
            PresetPointInfo = GameObject.Find("PresetPointInfo");
            RandomPointInfo = GameObject.Find("RandomPointInfo");
            InputCoordsInfo = GameObject.Find("InputCoordsInfo");
            SettingsInfo = GameObject.Find("SettingsInfo");


            AreaSeceltor.SetActive(false);
            OptionsBackGround.SetActive(false);
            SpecificPointInfo.SetActive(false);
            PresetPointInfo.SetActive(false);
            RandomPointInfo.SetActive(false);
            InputCoordsInfo.SetActive(false);
            SettingsInfo.SetActive(false);

        }

        public void Update()
        {
            
            if (Info.showmap && !Info.Showsettings)
            {
                ManageMapText();
                ManageRightSide(CurrentMode, Info.Showsettings);
                ManageLeftSide(CurrentMode, CurrentPreset);
                ManageSelectedPointIndicator();

                AreaSeceltor.SetActive(true);
                Map.SetActive(true);
                OptionsPannel.SetActive(false);
                Rightside.SetActive(false);
                Primaryoptions.SetActive(false);
            }
        }
        public void ManageLeftSide(int Mode, int preset)
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
                        switch (preset)
                        {
                            case 1:
                                PresetTextTEXT.text = "UnderWater Island";
                                WorldPointtoMoveSelecedPoint(new Vector3(-110, 0, 952));
                                break;
                            case 2:
                                PresetTextTEXT.text = "Dunes Vent";
                                WorldPointtoMoveSelecedPoint(new Vector3(-1532, 0, 468));
                                break;
                            case 3:
                                PresetTextTEXT.text = "Mushroom Forest";
                                WorldPointtoMoveSelecedPoint(new Vector3(-928, 0, 735));
                                break;
                            case 4:
                                PresetTextTEXT.text = "Blood Kelp Trench";
                                WorldPointtoMoveSelecedPoint(new Vector3(-959, 0, -565));
                                break;
                            case 5:
                                PresetTextTEXT.text = "Blood Kelp Lost River";
                                WorldPointtoMoveSelecedPoint(new Vector3(-623, 0, 1122));
                                break;
                            case 6:
                                PresetTextTEXT.text = "Mountains Wreckage";
                                WorldPointtoMoveSelecedPoint(new Vector3(607, 0, 1217));
                                break;
                            case 7:
                                PresetTextTEXT.text = "Bulb Lost River Entrance";
                                WorldPointtoMoveSelecedPoint(new Vector3(1123, 0, 908));
                                break;
                            case 8:
                                PresetTextTEXT.text = "Island Degasi";
                                WorldPointtoMoveSelecedPoint(new Vector3(-773, 0, -1110));
                                break;
                            case 9:
                                PresetTextTEXT.text = "Island Oasis";
                                WorldPointtoMoveSelecedPoint(new Vector3(-711, 0, -1079));
                                break;
                            case 10:
                                PresetTextTEXT.text = "Gun Island";
                                WorldPointtoMoveSelecedPoint(new Vector3(297, 0, 1064));
                                break;
                            case 11:
                                PresetTextTEXT.text = "Jellyshroom Cave Entrance";
                                WorldPointtoMoveSelecedPoint(new Vector3(131, 0, -389));
                                break;
                            case 12:
                                PresetTextTEXT.text = "Crash Zone ;)";
                                WorldPointtoMoveSelecedPoint(new Vector3(1136, 0, -1547));
                                break;
                            default:
                                PresetTextTEXT.text = "Error please report to dev";
                                WorldPointtoMoveSelecedPoint(new Vector3(0, 0, 0));
                                break;
                        }
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
                if (CurrentMode == 1)//specific
                {
                    SpecificPointInfo.SetActive(true);
                    PresetPointInfo.SetActive(false);
                    SettingsInfo.SetActive(false);
                }
                if (CurrentMode == 2)//Preset
                {
                    SpecificPointInfo.SetActive(false);
                    PresetPointInfo.SetActive(true);
                    RandomPointInfo.SetActive(false);
                    SettingsInfo.SetActive(false);
                }
                if (CurrentMode == 3)//randon
                {
                    PresetPointInfo.SetActive(false);
                    RandomPointInfo.SetActive(true);
                    InputCoordsInfo.SetActive(false);
                    SettingsInfo.SetActive(false);
                }
                if (CurrentMode == 4)//Input Coords
                {
                    RandomPointInfo.SetActive(false);
                    InputCoordsInfo.SetActive(true);
                    SettingsInfo.SetActive(false);
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
        public void ManageMapText()
        {
            if(!LifePodRemastered.Config.ShowTextOnMap)
            {
                Map.GetComponent<Image>().sprite = Info.assetBundle.LoadAsset<Sprite>("Map2");
            }
            if(LifePodRemastered.Config.ShowTextOnMap)
            {
                Map.GetComponent<Image>().sprite = Info.assetBundle.LoadAsset<Sprite>("Map1");
            }
        }
        public void ManageSelectedPointIndicator()
        {
            if (!AnimationActive)
            {
                AnimationActive = false;
                SelectedPoint.GetComponent<Animation>().Play();
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
            float diff = Info.currentRes.width / 2560f;

            if ((MousePos.y >= (107.0f * diff) && MousePos.y <= (1335.0f * diff)) && (MousePos.x >= (664.0f * diff) && MousePos.x <= (1895.0f * diff)))
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
            vector3 = new Vector3((WorldPoint.x / (3.33f / diff)) + (1280 * diff), (WorldPoint.z / (3.33f / diff)) + (720 * diff));
            LifePodRemastered.Logger.LogInfo(vector3.ToString());
            SelectedPoint.GetComponent<RectTransform>().localPosition = new Vector3((vector3.x - (1280f * diff)) / (1250f * diff), (vector3.y - (720f * diff)) / (1250f * diff), 0);
            Info.SelectedSpawn = WorldPoint;
        }
        public void MousePositionToSelectedPoint(Vector3 MousePos)
        {
            float diff = Info.currentRes.width / 2560f;
            vector3 = new Vector3((MousePos.x - (1280 * diff)) * (3.33f / diff), 0, (MousePos.y - (720 * diff)) * (3.33f / diff));
            Info.SelectedSpawn = vector3;
            LifePodRemastered.Logger.LogInfo(vector3.ToString());
            SelectedPoint.GetComponent<RectTransform>().localPosition = new Vector3((MousePos.x - (1280f * diff)) / (1250f * diff), (MousePos.y - (720f * diff)) / (1250f * diff), 0);
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
                Info.newSave = true; // 1 survival, 2 creative, 3, freedom, 4 hardcore
                if (Info.GameMode == 1)
                {
                    CoroutineHost.StartCoroutine(uGUI_MainMenu.main.StartNewGame(GameMode.Survival));
                }
                if (Info.GameMode == 2)
                {
                    CoroutineHost.StartCoroutine(uGUI_MainMenu.main.StartNewGame(GameMode.Creative));
                }
                if (Info.GameMode == 3)
                {
                    CoroutineHost.StartCoroutine(uGUI_MainMenu.main.StartNewGame(GameMode.Freedom));
                }
                if (Info.GameMode == 4)
                {
                    CoroutineHost.StartCoroutine(uGUI_MainMenu.main.StartNewGame(GameMode.Hardcore));
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
            if(CurrentMode != 1)
            {
                CurrentMode--;
                HoverSound.GetComponent<FMOD_StudioEventEmitter>().StartEvent();
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
            if (CurrentPreset != 12)
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
            WorldPointtoMoveSelecedPoint(Info.SelectedSpawn);
            ButtonHoverSharp.GetComponent<FMOD_StudioEventEmitter>().StartEvent();

            ManageRightSide(CurrentMode, Info.Showsettings);
            ManageLeftSide(CurrentMode, CurrentPreset);
        }
        void OnRandomizePointButtonClick()
        {
            float diff = Info.currentRes.width / 2560f;
            Vector3 ranvector3 = new Vector3(Random.Range(664.0f * diff, 1895.0f * diff), Random.Range(107.0f * diff, 1335.0f * diff), 0);
            MousePositionToSelectedPoint(ranvector3);
            ButtonHoverSharp.GetComponent<FMOD_StudioEventEmitter>().StartEvent();

            ManageRightSide(CurrentMode, Info.Showsettings);
            ManageLeftSide(CurrentMode, CurrentPreset);
        }
    }
}