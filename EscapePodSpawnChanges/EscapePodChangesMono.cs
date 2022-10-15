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

namespace EscapePodSpawnChanges
{
    internal class LifePodMapFunction : MonoBehaviour
    {
        public static AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "lifepodspawnmap"));

        public static GameObject canvas;

        GameObject AreaSeceltor;

        GameObject Rightside;
        GameObject Primaryoptions;


        GameObject ModeChoiceText;
        GameObject ModePresetPoint;
        GameObject RandomizePointButton;
        GameObject SelectedPoint;

        int CurrentMode = 1;

        Vector3 vector3 = new Vector3(10000, 10000, 10000);

        ToFile ToFileInstance = new ToFile();
        public void Awake()
        {
            ToFileInstance.CreateJson();

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

            GameObject.Find("SettingModsButton").GetComponent<Button>().onClick.AddListener(OnSettingsModButtonClick);
            GameObject.Find("StartGameButton").GetComponent<Button>().onClick.AddListener(OnStartGameButtonClick);
            GameObject.Find("BackToMenuButton").GetComponent<Button>().onClick.AddListener(OnBackToMenuButtonClick);
            GameObject.Find("ModeChoiceleft").GetComponent<Button>().onClick.AddListener(OnModeChoiceleftClick);
            GameObject.Find("ModeChoiceRight").GetComponent<Button>().onClick.AddListener(OnModeChoiceRightClick);
            GameObject.Find("ModePresetPointChoiceleft").GetComponent<Button>().onClick.AddListener(OnModePresetPointChoiceleftClick);
            GameObject.Find("ModePresetPointChoiceRight").GetComponent<Button>().onClick.AddListener(OnModePresetPointChoiceRightClick);
            GameObject.Find("RandomizePointButton").GetComponent<Button>().onClick.AddListener(OnRandomizePointButtonClick);

            ModePresetPoint = GameObject.Find("ModePresetPoint");
            RandomizePointButton = GameObject.Find("RandomizePointButton");
            ModeChoiceText = GameObject.Find("ModeChoiceText");
            SelectedPoint = GameObject.Find("SelectedPoint");

            AreaSeceltor.SetActive(false);
        }
        public void Update()
        {
            if (Info.showmap)
            {
                AreaSeceltor.SetActive(true);
                Rightside.SetActive(false);
                Primaryoptions.SetActive(false);

                if (CurrentMode == 1)
                {
                    ModeChoiceText.GetComponent<Text>().text = "Specific Point";

                    RandomizePointButton.SetActive(false);
                    ModePresetPoint.SetActive(false);

                    if (Input.GetMouseButtonDown(0) && CheckValidMousePosition(Input.mousePosition) == 1)
                    {
                        vector3 = new Vector3((Input.mousePosition.x - 1280) * 3.33f, 0, (Input.mousePosition.y - 720) * 3.33f);
                        Info.SelectedSpawn = vector3;

                        SelectedPoint.GetComponent<RectTransform>().position = new Vector3(SelectedPoint.transform.position.x, (Input.mousePosition.y - 720f) / 1380f, ((Input.mousePosition.x - 1280f) / 1380f) * -1f);
                    }
                }
                if (CurrentMode == 2)
                {
                    ModeChoiceText.GetComponent<Text>().text = "Preset Point";
                    RandomizePointButton.SetActive(false);
                    ModePresetPoint.SetActive(true);
                }

                if (CurrentMode == 3)
                {
                    ModeChoiceText.GetComponent<Text>().text = "Random Point";
                    RandomizePointButton.SetActive(true);
                    ModePresetPoint.SetActive(false);
                }
            }
            if (!Info.showmap)
            {

            }
        }
        public float CheckValidMousePosition(Vector3 MousePos)
        {
            if ((MousePos.y >= 107.0f && MousePos.y <= 1335.0f) && (MousePos.x >= 664.0f && MousePos.x <= 1895.0f))
            {
                Logger.Log(Logger.Level.Info, "ValidPos1", null, true);
                return 1;
            }
            else
            {
                Logger.Log(Logger.Level.Info, "InvalidPos", null, true);
                return 0;
            }
        }
        void OnSettingsModButtonClick()
        {
            Logger.Log(Logger.Level.Info, "OMG IT WORKED", null, true);
        }
        void OnStartGameButtonClick()
        {
            uGUI_MainMenu.main.StartCoroutine(uGUI_MainMenu.main.StartNewGame(GameMode.Freedom));
            Info.showmap = false;
        }
        void OnBackToMenuButtonClick()
        {
            Info.showmap = false;
            AreaSeceltor.SetActive(false);
            Rightside.SetActive(true);
            Primaryoptions.SetActive(true);
        }
        void OnModeChoiceleftClick()
        {
            if(CurrentMode != 1)
            {
                CurrentMode--;
            }
        }
        void OnModeChoiceRightClick()
        {
            if (CurrentMode != 3)
            {
                CurrentMode++;
            }
        }
        void OnModePresetPointChoiceleftClick()
        {
            Logger.Log(Logger.Level.Info, "OMG IT WORKED", null, true);
        }
        void OnModePresetPointChoiceRightClick()
        {
            Logger.Log(Logger.Level.Info, "OMG IT WORKED", null, true);
        }
        void OnRandomizePointButtonClick()
        {
            Vector3 ranvector3 = new Vector3(Random.Range(664.0f, 1895.0f), 0, Random.Range(107.0f, 1335.0f));

            vector3 = new Vector3((ranvector3.x - 1280) * 3.33f, 0, (ranvector3.z - 720) * 3.33f);

            SelectedPoint.GetComponent<RectTransform>().position = new Vector3(SelectedPoint.transform.position.x, (ranvector3.z - 720f) / 1380f, ((ranvector3.x - 1280f) / 1380f) * -1f);

            Info.SelectedSpawn = vector3;
        }
    }
} 
