using HarmonyLib;
using Logger = QModManager.Utility.Logger;
using RecipeData = SMLHelper.V2.Crafting.TechData;
using QModManager.API.ModLoading;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UWE;

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
        GameObject OptionsPannelBackButton;

        GameObject ModeChoiceText;
        GameObject ModePresetPoint;
        GameObject RandomizePointButton;
        GameObject PresetText;
        GameObject Map;
        GameObject SpecificCoordsInput;

        GameObject SelectedPoint;
        GameObject SelectedPointIndicator;

        GameObject SpecificPointInfo;
        GameObject PresetPointInfo;
        GameObject RandomPointInfo;
        GameObject InputCoordsInfo;
        GameObject SettingsInfo;

        int CurrentMode = 1;
        int CurrentPreset = 1;

        bool PointChanged = false;
        bool startsequence = false;
        bool Scaling = false;
        bool makebig = false;
        bool makesmall = false;


        Vector3 vector3 = new Vector3(10000, 10000, 10000);

        //ToFile ToFileInstance = new ToFile();
        public void Awake()
        {
            //ToFileInstance.CreateJson();

            Font arial;
            arial = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

            Rightside = GameObject.Find("RightSide");
            Primaryoptions = GameObject.Find("PrimaryOptions");

            canvas = GameObject.Find("Menu canvas");

            AreaSeceltor = Instantiate(assetBundle.LoadAsset<GameObject>("AreaSelector"));
            AreaSeceltor.transform.parent = canvas.transform;
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
            GameObject.Find("SpecificCoordsInput").GetComponent<InputField>().onEndEdit.AddListener(OnEndInputFieldEdit);

            OptionsPannel = GameObject.Find("Menu canvas/Panel/Options");
            OptionsPannelBackButton = GameObject.Find("Menu canvas/Panel/Options/ButtonBack");

            Map = GameObject.Find("Map");
            ModePresetPoint = GameObject.Find("ModePresetPoint");
            RandomizePointButton = GameObject.Find("RandomizePointButton");
            ModeChoiceText = GameObject.Find("ModeChoiceText");
            SelectedPoint = GameObject.Find("SelectedPoint");
            SelectedPointIndicator = GameObject.Find("SelectedPointIndicator");
            PresetText = GameObject.Find("PresetText");
            SpecificCoordsInput = GameObject.Find("SpecificCoordsInput");

            SpecificPointInfo = GameObject.Find("SpecificPointInfo");
            PresetPointInfo = GameObject.Find("PresetPointInfo");
            RandomPointInfo = GameObject.Find("RandomPointInfo");
            InputCoordsInfo = GameObject.Find("InputCoordsInfo");
            SettingsInfo = GameObject.Find("SettingsInfo");

            AreaSeceltor.SetActive(false);
            SpecificPointInfo.SetActive(false);
            PresetPointInfo.SetActive(false);
            RandomPointInfo.SetActive(false);
            InputCoordsInfo.SetActive(false);
            SettingsInfo.SetActive(false);
        }

        public void Update()
        {
            

            if (Info.Showsettings)
            {
                ManageRightSide(CurrentMode, Info.Showsettings);

                OptionsPannel.SetActive(true);
                OptionsPannelBackButton.SetActive(false);
                Map.SetActive(false);
            }
            if (Info.showmap && !Info.Showsettings)
            {
                ManageRightSide(CurrentMode, Info.Showsettings);
                ManageLeftSide(CurrentMode, CurrentPreset);

                ManageMapText();
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
            float diff = Info.currentRes.width / 2560f;

            if (Mode == 1)
            {
                ModeChoiceText.GetComponent<Text>().text = "Specific Point";

                RandomizePointButton.SetActive(false);
                ModePresetPoint.SetActive(false);
                SpecificCoordsInput.SetActive(false);

                if (Input.GetMouseButtonDown(0) && CheckValidMousePosition(Input.mousePosition) == 1)
                {
                    MousePositionToSelectedPoint(Input.mousePosition);
                }
            }
            if (Mode == 2)
            {
                ModeChoiceText.GetComponent<Text>().text = "Preset Point";
                RandomizePointButton.SetActive(false);
                ModePresetPoint.SetActive(true);

                Text PresetTextTEXT = PresetText.GetComponent<Text>();

                if (preset == 1)
                {
                    PresetTextTEXT.text = "UnderWater Island";
                    PresetTextTEXT.fontSize = 80;

                    Vector3 PresetPoint = new Vector3(-110, 0, 952);
                    WorldPointtoMoveSelecedPoint(PresetPoint);
                }
                if (preset == 2)
                {
                    PresetTextTEXT.text = "Dunes Vent";
                    PresetTextTEXT.fontSize = 100;

                    Vector3 PresetPoint = new Vector3(-1532, 0, 468);
                    WorldPointtoMoveSelecedPoint(PresetPoint);
                }
                if (preset == 3)
                {
                    PresetTextTEXT.text = "Mushroom Forest";
                    PresetTextTEXT.fontSize = 90;

                    Vector3 PresetPoint = new Vector3(-928, 0, 735);
                    WorldPointtoMoveSelecedPoint(PresetPoint);
                }
                if (preset == 4)
                {
                    PresetTextTEXT.text = "Blood Kelp Trench";
                    PresetTextTEXT.fontSize = 80;

                    Vector3 PresetPoint = new Vector3(-959, 0, -565);
                    WorldPointtoMoveSelecedPoint(PresetPoint);
                }
                if (preset == 5)
                {
                    PresetTextTEXT.text = "Blood Kelp Lost River";
                    PresetTextTEXT.fontSize = 75;

                    Vector3 PresetPoint = new Vector3(-623, 0, 1122);
                    WorldPointtoMoveSelecedPoint(PresetPoint);
                }
                if (preset == 6)
                {
                    PresetTextTEXT.text = "Mountains Wreckage";
                    PresetTextTEXT.fontSize = 80;

                    Vector3 PresetPoint = new Vector3(607, 0, 1217);
                    WorldPointtoMoveSelecedPoint(PresetPoint);
                }
                if (preset == 7)
                {
                    PresetTextTEXT.text = "Bulb Lost River Entrance";
                    PresetTextTEXT.fontSize = 65;

                    Vector3 PresetPoint = new Vector3(1123, 0, 908);
                    WorldPointtoMoveSelecedPoint(PresetPoint);
                }
                if (preset == 8)
                {
                    PresetTextTEXT.text = "Island Degasi";
                    PresetTextTEXT.fontSize = 100;

                    Vector3 PresetPoint = new Vector3(-773, 0, -1110);
                    WorldPointtoMoveSelecedPoint(PresetPoint);
                }
                if (preset == 9)
                {
                    PresetTextTEXT.text = "Island Oasis";
                    PresetTextTEXT.fontSize = 100;

                    Vector3 PresetPoint = new Vector3(-711, 0, -1079);
                    WorldPointtoMoveSelecedPoint(PresetPoint);
                }
                if (preset == 10)
                {
                    PresetTextTEXT.text = "Gun Island";
                    PresetTextTEXT.fontSize = 100;

                    Vector3 PresetPoint = new Vector3(297, 0, 1064);
                    WorldPointtoMoveSelecedPoint(PresetPoint);
                }
                if (preset == 11)
                {
                    PresetTextTEXT.text = "Jellyshroom Cave Entrance";
                    PresetTextTEXT.fontSize = 65;

                    Vector3 PresetPoint = new Vector3(131, 0, -389);
                    WorldPointtoMoveSelecedPoint(PresetPoint);
                }
                if (preset == 12)
                {
                    PresetTextTEXT.text = "Crash Zone ;)";
                    PresetTextTEXT.fontSize = 100;

                    Vector3 PresetPoint = new Vector3(1136, 0, -1547);
                    WorldPointtoMoveSelecedPoint(PresetPoint);
                }
            }

            if (Mode == 3)
            {
                ModeChoiceText.GetComponent<Text>().text = "Random Point";
                RandomizePointButton.SetActive(true);
                ModePresetPoint.SetActive(false);
                SpecificCoordsInput.SetActive(false);
            }

            if (Mode == 4)
            {
                ModeChoiceText.GetComponent<Text>().text = "Type Coords";
                RandomizePointButton.SetActive(false);
                SpecificCoordsInput.SetActive(true);
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
            if(!QMod.Config.ShowTextOnMap)
            {
                Map.GetComponent<Image>().sprite = Info.assetBundle.LoadAsset<Sprite>("Map2");
            }
            if(QMod.Config.ShowTextOnMap)
            {
                Map.GetComponent<Image>().sprite = Info.assetBundle.LoadAsset<Sprite>("Map1");
            }
        }
        public void ManageSelectedPointIndicator()
        {
            if (PointChanged == true)
            {
                PointChanged = false;
                startsequence = true;
            }
            if(startsequence == true)
            {
                if(SelectedPointIndicator.transform.localScale.x > 7 && !Scaling)
                {
                    Scaling = true;
                    makesmall = true;
                    CoroutineHost.StartCoroutine(TimeScaler());
                }
                if (SelectedPointIndicator.transform.localScale.x < 7 && !Scaling)
                {
                    Scaling = true;
                    makebig = true;
                    CoroutineHost.StartCoroutine(TimeScaler());
                }

                if(Scaling == true && makebig)
                {
                    SelectedPointIndicator.transform.localScale += new Vector3(0.05f, 0.05f, 0);
                }
                if (Scaling == true && makesmall)
                {
                    SelectedPointIndicator.transform.localScale -= new Vector3(0.05f, 0.05f, 0);
                }
            }
            IEnumerator TimeScaler()
            {
                yield return new WaitForSeconds(0.5f);
                Scaling = false;
                makebig = false;
                makesmall = false;
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
            SelectedPoint.GetComponent<RectTransform>().position = new Vector3(SelectedPoint.transform.position.x, (vector3.y - (720f * diff)) / (1380f * diff), ((vector3.x - (1280f * diff)) / (1380f * diff) * -1f));
            Info.SelectedSpawn = WorldPoint;
            PointChanged = true;
        }
        public void MousePositionToSelectedPoint(Vector3 MousePos)
        {
            float diff = Info.currentRes.width / 2560f;
            vector3 = new Vector3((MousePos.x - (1280 * diff)) * (3.33f / diff), 0, (MousePos.y - (720 * diff)) * (3.33f / diff));
            Info.SelectedSpawn = vector3;
            SelectedPoint.GetComponent<RectTransform>().position = new Vector3(SelectedPoint.transform.position.x, (MousePos.y - (720f * diff)) / (1380f * diff), ((MousePos.x - (1280f * diff)) / (1380f * diff) * -1f));
            ButtonHoverSharp.GetComponent<FMOD_StudioEventEmitter>().StartEvent();
            PointChanged = true;
        }




        void OnSettingsModButtonClick()
        {
            Info.Showsettings = !Info.Showsettings;
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
                    uGUI_MainMenu.main.StartCoroutine(uGUI_MainMenu.main.StartNewGame(GameMode.Survival));
                }
                if (Info.GameMode == 2)
                {
                    uGUI_MainMenu.main.StartCoroutine(uGUI_MainMenu.main.StartNewGame(GameMode.Creative));
                }
                if (Info.GameMode == 3)
                {
                    uGUI_MainMenu.main.StartCoroutine(uGUI_MainMenu.main.StartNewGame(GameMode.Freedom));
                }
                if (Info.GameMode == 4)
                {
                    uGUI_MainMenu.main.StartCoroutine(uGUI_MainMenu.main.StartNewGame(GameMode.Hardcore));
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
        }
        void OnModeChoiceRightClick()
        {
            if (CurrentMode != 4)
            {
                CurrentMode++;
                HoverSound.GetComponent<FMOD_StudioEventEmitter>().StartEvent();
            }
        }
        void OnModePresetPointChoiceleftClick()
        {
            if (CurrentPreset != 1)
            {
                CurrentPreset--;
                ButtonHoverSharp.GetComponent<FMOD_StudioEventEmitter>().StartEvent();
            }
        }
        void OnModePresetPointChoiceRightClick()
        {
            if (CurrentPreset != 12)
            {
                CurrentPreset++;
                ButtonHoverSharp.GetComponent<FMOD_StudioEventEmitter>().StartEvent();
            }
        }
        void OnEndInputFieldEdit(string s)
        {
            Info.SelectedSpawn = StringToVector3(s);
            WorldPointtoMoveSelecedPoint(Info.SelectedSpawn);
            ButtonHoverSharp.GetComponent<FMOD_StudioEventEmitter>().StartEvent();
        }
        void OnRandomizePointButtonClick()
        {
            float diff = Info.currentRes.width / 2560f;
            Vector3 ranvector3 = new Vector3(Random.Range(664.0f * diff, 1895.0f * diff), Random.Range(107.0f * diff, 1335.0f * diff), 0);
            MousePositionToSelectedPoint(ranvector3);
            ButtonHoverSharp.GetComponent<FMOD_StudioEventEmitter>().StartEvent();
        }
    }
}