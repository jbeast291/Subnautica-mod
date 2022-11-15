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
    internal class EscapePodInGameMono : MonoBehaviour
    {
        public static AssetBundle assetBundle = Info.assetBundle;
        GameObject parachute;
        bool DetachParachute = false;
        bool toggleParachute;
        public void Start()
        {
            if(QMod.Config.ToggleParachute)
            {
                
            }
            if (Info.newSave && QMod.Config.ToggleAirSpawn)
            {
                CoroutineHost.StartCoroutine(WaitForNewSave());
                parachute = Instantiate(assetBundle.LoadAsset<GameObject>("Parachute"));
                parachute.transform.parent = EscapePod.main.gameObject.transform;
                parachute.transform.position = EscapePod.main.transform.position;
                parachute.transform.localPosition = new Vector3(0, 6.9f, 0);
            }
        }
        public void Update()
        {
            if (DetachParachute)
            {
                Vector3 target = new Vector3(parachute.transform.position.x, 1000, parachute.transform.position.z);
                float speed = 20f * Time.deltaTime;
                parachute.transform.position = Vector3.MoveTowards(parachute.transform.position, target, speed);

                if (parachute.transform.position.y >= 0f)
                {
                    DetachParachute = false;
                    Destroy(parachute);
                }
            }
        }
        IEnumerator WaitForNewSave()
        {
            yield return new WaitForSeconds(32);//(32 * (QMod.Config.AirSpawnHeight / 100 / 6)
            Info.newSave = false;
            DetachParachute = true;
        }

    }
}
