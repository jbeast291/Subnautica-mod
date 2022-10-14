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

        GameObject imageGO;
        GameObject imageGO2;
        GameObject imageGO3;
        GameObject imageGO4;
        GameObject imageGO5;

        GameObject textgo;
        GameObject textgo2;
        GameObject textgo3;

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

            imageGO = new GameObject();
            imageGO.transform.parent = canvas.transform;
            imageGO.AddComponent<Image>();

            imageGO.GetComponent<Image>().gameObject.SetActive(false);
            imageGO.GetComponent<Image>().sprite = assetBundle.LoadAsset<Sprite>("SubnauticaMap1");

            imageGO.GetComponent<Image>().GetComponent<RectTransform>().position = new Vector3(1280, 720, 0);
            imageGO.GetComponent<Image>().GetComponent<RectTransform>().sizeDelta = new Vector2(2100 * 0.60f, 2100 * 0.60f);

            //create second image

            imageGO2 = new GameObject();
            imageGO2.transform.parent = canvas.transform;
            imageGO2.AddComponent<Image>();

            imageGO2.GetComponent<Image>().gameObject.SetActive(false);
            imageGO2.GetComponent<Image>().sprite = assetBundle.LoadAsset<Sprite>("SelectedSpot");

            imageGO2.GetComponent<Image>().GetComponent<RectTransform>().sizeDelta = new Vector2(442 * 0.60f, 130 * 0.60f);

            //create third image

            imageGO3 = new GameObject();
            imageGO3.transform.parent = canvas.transform;
            imageGO3.AddComponent<Image>();

            imageGO3.GetComponent<Image>().gameObject.SetActive(false);
            imageGO3.GetComponent<Image>().sprite = assetBundle.LoadAsset<Sprite>("scannerroomui_listbgmouseover.png-CAB_f5d5c899328b296cd841c68fa14b2706--8424868254826263282");


            imageGO3.GetComponent<Image>().GetComponent<RectTransform>().position = new Vector3(320, 720, 0);
            imageGO3.GetComponent<Image>().GetComponent<RectTransform>().sizeDelta = new Vector2(918 * 0.6f, 500);

            //create fourth image

            imageGO4 = new GameObject();
            imageGO4.transform.parent = canvas.transform;
            imageGO4.AddComponent<Image>();


            imageGO4.GetComponent<Image>().gameObject.SetActive(false);
            imageGO4.GetComponent<Image>().sprite = assetBundle.LoadAsset<Sprite>("scannerroomui_listbgmouseover.png-CAB_f5d5c899328b296cd841c68fa14b2706--8424868254826263282");

            imageGO4.GetComponent<Image>().GetComponent<RectTransform>().position = new Vector3(2240, 720, 0);
            imageGO4.GetComponent<Image>().GetComponent<RectTransform>().sizeDelta = new Vector2(918 * 0.6f, 500);

            //create fourth image

            imageGO5 = new GameObject();
            imageGO5.transform.parent = canvas.transform;
            imageGO5.AddComponent<Image>();


            imageGO5.GetComponent<Image>().gameObject.SetActive(false);
            imageGO5.GetComponent<Image>().sprite = assetBundle.LoadAsset<Sprite>("scannerroomui_listbgmouseover.png-CAB_f5d5c899328b296cd841c68fa14b2706--8424868254826263282");

            imageGO5.GetComponent<Image>().GetComponent<RectTransform>().position = new Vector3(2240, 220, 0);
            imageGO5.GetComponent<Image>().GetComponent<RectTransform>().sizeDelta = new Vector2(918 * 0.35f, 500 * 0.15f);


            //Create text for background

            textgo = new GameObject();
            textgo.transform.parent = canvas.transform;
            textgo.AddComponent<Text>();

            textgo.GetComponent<Text>().font = arial;
            textgo.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            textgo.GetComponent<Text>().color = Color.white;
            textgo.GetComponent<Text>().transform.position = new Vector3(2240, 820, 0);
            textgo.GetComponent<Text>().text = "Click a position on the map to set were the pod spawns.";
            textgo.GetComponent<Text>().fontSize = 36;
            textgo.GetComponent<Text>().GetComponent<RectTransform>().sizeDelta = new Vector2(400, 500);
            textgo.SetActive(false);

            //create second text

            textgo2 = new GameObject();
            textgo2.transform.parent = canvas.transform;
            textgo2.AddComponent<Text>();

            textgo2.GetComponent<Text>().font = arial;
            textgo2.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            textgo2.GetComponent<Text>().color = Color.white;
            textgo2.GetComponent<Text>().transform.position = new Vector3(2240, 620, 0);
            textgo2.GetComponent<Text>().text = "Press play to start!";
            textgo2.GetComponent<Text>().fontSize = 36;
            textgo2.GetComponent<Text>().GetComponent<RectTransform>().sizeDelta = new Vector2(400, 500);
            textgo2.SetActive(false);

            //Create button text

            textgo3 = new GameObject();
            textgo3.transform.parent = canvas.transform;
            textgo3.AddComponent<Text>();

            textgo3.GetComponent<Text>().font = arial;
            textgo3.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            textgo3.GetComponent<Text>().color = Color.white;
            textgo3.GetComponent<Text>().transform.position = new Vector3(2240, 220, 0);
            textgo3.GetComponent<Text>().text = "Play";
            textgo3.GetComponent<Text>().fontSize = 36;
            textgo3.GetComponent<Text>().GetComponent<RectTransform>().sizeDelta = new Vector2(400, 500);
            textgo3.SetActive(false);

        }
        public void Update()
        {
            Logger.Log(Logger.Level.Info, "yes", null, true);
            if (Info.showmap)
            {

                textgo.SetActive(true);
                textgo2.SetActive(true);
                textgo3.SetActive(true);

                imageGO.SetActive(true);
                imageGO3.SetActive(true);
                imageGO4.SetActive(true);
                imageGO5.SetActive(true);

                Rightside.SetActive(false);
                Primaryoptions.SetActive(false);

                if (Input.GetMouseButtonDown(0))
                {
                    Logger.Log(Logger.Level.Info, Input.mousePosition.ToString(), null, true);

                    if (CheckValidMousePosition(Input.mousePosition) == 1)
                    {
                        vector3 = new Vector3((Input.mousePosition.x - 1280) * 3.33f, (Input.mousePosition.y - 720) * 3.33f, 0);
                        //Logger.Log(Logger.Level.Info, $"{vector3}", null, true);
                        imageGO2.GetComponent<Image>().GetComponent<RectTransform>().position = new Vector3(Input.mousePosition.x + (210 * 0.60f), Input.mousePosition.y + (52 * 0.60f), 0);
                        imageGO2.gameObject.SetActive(true);

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
                textgo.SetActive(false);
                textgo2.SetActive(false);

                imageGO2.SetActive(false);
                imageGO3.SetActive(false);
                imageGO4.SetActive(false); 
                imageGO.gameObject.SetActive(false);
            }
        }
        public float CheckValidMousePosition(Vector3 MousePos)
        {
            if ((MousePos.y >= 107.0f && MousePos.y <= 1335.0f) && (MousePos.x >= 664.0f && MousePos.x <= 1895.0f))
            {
                Logger.Log(Logger.Level.Info, "ValidPos", null, true);
                return 1;
            }
            else
            {
                Logger.Log(Logger.Level.Info, "InvalidPos", null, true);
                return 0;
            }
        }
    }
} 
