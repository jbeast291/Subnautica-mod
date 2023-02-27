using HarmonyLib;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UWE;
using SMLHelper.V2.Utility;


namespace LifePodRemastered
{
    internal class EscapePodInGameMono : MonoBehaviour
    {
        static AssetBundle assetBundle = Info.assetBundle;

        GameObject parachute;
        GameObject PlayerBody;
        GameObject PlayerCam;

        float tempfloat = 0f;
        Vector3 LastPos;
        bool podlanded;

        public void Start()
        {
            if (Info.newSave && LifePodRemastered.Config.ToggleAirSpawn)
            {
                CoroutineHost.StartCoroutine(CustomIntro());
                CoroutineHost.StartCoroutine(Parachute());
                CoroutineHost.StartCoroutine(CheckIfLanded());

                parachute = Instantiate(assetBundle.LoadAsset<GameObject>("Parachute"));
                parachute.transform.parent = EscapePod.main.gameObject.transform;
                parachute.transform.position = EscapePod.main.transform.position;
                parachute.transform.localPosition = new Vector3(0, 6.9f, 0);

                EscapePod.main.GetComponent<Rigidbody>().freezeRotation = true;
            }
        }
        public void Update()
        {
            /*  
              if(Input.GetMouseButtonDown(0))
              {
                  SaveManager.WriteSettingsToCurrentSlot();
                  Debug.Log(SaveUtils.GetCurrentSaveDataDir());
              }
              if (Input.GetMouseButtonDown(1))
              {
                  SaveManager.ReadSettingsToCurrectSlot();
                  Debug.Log(SaveUtils.GetCurrentSaveDataDir());
              }*/
            
        }
        IEnumerator Parachute()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            while (parachute.transform.position.y > -40.9f)
            {
                yield return null;
            }
            parachute.GetComponent<Animation>().Play();
            yield return new WaitForSecondsRealtime(5.1f);
            Debug.Log("destroy parachute");
            Destroy(parachute);
        }
        
        IEnumerator CustomIntro()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            PlayerBody = GameObject.Find("Player/body");
            PlayerBody.SetActive(false);
            PlayerCam = GameObject.Find("Player/camPivot/camRoot");
            PlayerCam.GetComponent<MainCameraControl>()._cinematicMode = true;

            yield return new WaitForSecondsRealtime(0.1f);
            while (Player.main.transform.position.z <= (EscapePod.main.transform.position.z + 20))
            {
                Player.main.SetPosition(EscapePod.main.transform.position + new Vector3(0, 20 , 0 + tempfloat));
                tempfloat += 2f * Time.deltaTime;
                PlayerCam.GetComponent<MainCameraControl>().LookAt(EscapePod.main.transform.position);
                yield return null;
            }
            tempfloat = 0f;
            while (Player.main.transform.position.y >= (EscapePod.main.transform.position.y))
            {
                Player.main.SetPosition(EscapePod.main.transform.position + new Vector3(0, 20 - tempfloat, 20));
                tempfloat += 2f * Time.deltaTime;
                PlayerCam.GetComponent<MainCameraControl>().LookAt(EscapePod.main.transform.position);
                yield return null;
            }
            while(!podlanded)
            {
                Player.main.SetPosition(EscapePod.main.transform.position + new Vector3(0, 0, 20));
                PlayerCam.GetComponent<MainCameraControl>().LookAt(EscapePod.main.transform.position);
                //LifePodRemastered.Logger.LogInfo("Anim");
                yield return null;
            }
            /*while (true)
            {
                Player.main.SetPosition(EscapePod.main.transform.position + new Vector3(0, 0, 20));
               PlayerCam.GetComponent<MainCameraControl>().LookAt(EscapePod.main.transform.position);
                //LifePodRemastered.Logger.LogInfo("Anim");
                yield return null;
            }*/
            EndCutscene();
        }
        IEnumerator CheckIfLanded()
        {
            LastPos = EscapePod.main.transform.position;
            yield return new WaitForSeconds(0.1f);

            Debug.Log(LastPos);
            Debug.Log(EscapePod.main.transform.position);

            if (LastPos == EscapePod.main.transform.position && Time.deltaTime != 0)
            {
                podlanded = true;
                yield break;
            }
            CoroutineHost.StartCoroutine(CheckIfLanded());
        }
        void EndCutscene()
        {
            PlayerBody.SetActive(true);
            PlayerCam.GetComponent<MainCameraControl>()._cinematicMode = false;
            Player.main.SetPosition(EscapePod.main.transform.position);
        }
    }
}
