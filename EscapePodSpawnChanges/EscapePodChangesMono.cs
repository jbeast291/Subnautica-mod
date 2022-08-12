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

        public Canvas canvas;

        public Text text;

        public SpriteState sprState = new SpriteState();

        public Image image;
        public Image image2;
        public Image buttonimage;

        public GameObject buttonPrefab;

        public Selectable.Transition transition;


        RectTransform rectTransform;
        RectTransform rectTransform2;
        RectTransform rectTransform3;
        RectTransform rectTransform4;

        GameObject Rightside;
        GameObject Primaryoptions;

        Vector3 vector3 = new Vector3(10000, 10000, 10000);

        public void Awake()
        {
            Font arial;
            arial = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

            Rightside = GameObject.Find("RightSide");
            Primaryoptions = GameObject.Find("PrimaryOptions");


            canvas = uGUI.main.screenCanvas.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            //create image

            GameObject imageGO = new GameObject();
            imageGO.transform.parent = canvas.transform;
            imageGO.AddComponent<Image>();

            image = imageGO.GetComponent<Image>();
            image.gameObject.SetActive(false);
            image.sprite = assetBundle.LoadAsset<Sprite>("SubnauticaMap1");

            rectTransform = image.GetComponent<RectTransform>();
            rectTransform.position = new Vector3(1280, 720, 0);
            rectTransform.sizeDelta = new Vector2(2100 * 0.60f, 2100 * 0.60f);

            //create second image

            GameObject imageGO2 = new GameObject();
            imageGO2.transform.parent = canvas.transform;
            imageGO2.AddComponent<Image>();

            image2 = imageGO2.GetComponent<Image>();
            image2.gameObject.SetActive(false);
            image2.sprite = assetBundle.LoadAsset<Sprite>("SelectedSpot");

            rectTransform2 = image2.GetComponent<RectTransform>();
            rectTransform2.sizeDelta = new Vector2(442 * 0.60f, 130 * 0.60f);

            //Create text for background

            GameObject textgo = new GameObject();
            textgo.transform.parent = canvas.transform;
            textgo.AddComponent<Text>();

            text = textgo.GetComponent<Text>();
            text.font = arial;
            text.alignment = TextAnchor.MiddleCenter;
            text.GetComponent<Transform>();

            rectTransform3 = text.GetComponent<RectTransform>();
            rectTransform3.localPosition = new Vector3(0, 0, 0);
            rectTransform3.sizeDelta = new Vector2(600, 200);

        }
        public void Update()
        {
            if (Info.showmap)
            {
                Logger.Log(Logger.Level.Info, $"pre show", null, true);
                text.color = Color.white;
                text.transform.position = new Vector3(0, 0, 0); // set the position of the text based on the config and the resolution
                text.text = $"TEST"; // set the text to have the day count from daynightcycle
                text.fontSize = 48;
                

                image.gameObject.SetActive(true);
                Rightside.SetActive(false);
                Primaryoptions.SetActive(false);
                Logger.Log(Logger.Level.Info, $"showed map", null, true);

                if (Input.GetMouseButtonDown(0))
                {

                    if (CheckValidMousePosition(Input.mousePosition))
                    {
                        vector3 = new Vector3((Input.mousePosition.x - 1280) * 3.33f, (Input.mousePosition.y - 720) * 3.33f, 0);
                        Logger.Log(Logger.Level.Info, $"{vector3}", null, true);
                        image2.rectTransform.position = new Vector3(Input.mousePosition.x + (210 * 0.60f), Input.mousePosition.y + (52 * 0.60f), 0);
                        image2.gameObject.SetActive(true);

                    }
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (vector3 != new Vector3(10000, 10000, 10000))
                    {
                        Info.SelectedSpawn = new Vector3((Input.mousePosition.x - 1280) * 3.33f, 300, (Input.mousePosition.y - 720) * 3.33f);
                        Info.showmap = false;
                        uGUI_MainMenu.main.StartCoroutine(uGUI_MainMenu.main.StartNewGame(GameMode.Freedom));
                    }
                }
            }
            if (!Info.showmap)
            {
                image2.gameObject.SetActive(false);
                image.gameObject.SetActive(false);
            }
        }
        void startgame()
        {
            Logger.Log(Logger.Level.Info, "PRESSSSSSSSSSSSSS", null, true);
        }
        public bool CheckValidMousePosition(Vector3 MousePos)
        {
            if ((MousePos.y >= 107.0f && MousePos.y <= 1335.0f) && (MousePos.x >= 664.0f && MousePos.x <= 1895.0f))
            {
                Logger.Log(Logger.Level.Info, "ValidPos", null, true);
                return true;
            }
            else
            {
                Logger.Log(Logger.Level.Info, "InvalidPos", null, true);
                return false;
            }
        }
    }
} 
