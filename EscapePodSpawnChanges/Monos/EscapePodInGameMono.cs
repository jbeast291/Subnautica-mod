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
        GameObject parachute2;

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
                //CoroutineHost.StartCoroutine(Parachute());
                CoroutineHost.StartCoroutine(CheckIfLanded());
                CoroutineHost.StartCoroutine(LockCamera());

                SetupParachutes();

                EscapePod.main.GetComponent<Rigidbody>().freezeRotation = true;


            }
        }
        public void SetupParachutes()
        {
            parachute = Instantiate(assetBundle.LoadAsset<GameObject>("ParachuteRight Variant"));
            parachute.transform.parent = EscapePod.main.gameObject.transform;
            parachute.transform.position = EscapePod.main.transform.position;
            parachute.transform.localPosition = new Vector3(-3, 2.5f, 0);
            parachute.transform.localScale = new Vector3(350, 350, 350);
            parachute.SetActive(false);

            parachute2 = Instantiate(assetBundle.LoadAsset<GameObject>("ParachuteLeft Variant"));
            parachute2.transform.parent = EscapePod.main.gameObject.transform;
            parachute2.transform.position = EscapePod.main.transform.position;
            parachute2.transform.localPosition = new Vector3(3, 2.5f, 0);
            parachute2.transform.localScale = new Vector3(350, 350, 350);
            parachute2.SetActive(false);
        }
        public void Update()
        {
            /*
            if (Input.GetKeyDown(KeyCode.O))
            {
                CrashedShipExploder.main.PlayExplosionFX();
                CrashedShipExploder.main.ShakePlayerCamera();
                //Utils.PlayFMODAsset(SaveManager.GetFmodAsset("event:/env/music/aurora_reveal"), Player.main.transform);
                CrashedShipExploder.main.shipExplodeSound.StartEvent();
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                Utils.PlayFMODAsset(CrashedShipExploder.main.rumbleSound, Player.main.transform);
            }
              
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
        IEnumerator LockCamera()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            PlayerBody = GameObject.Find("Player/body");
            PlayerBody.SetActive(false);
            PlayerCam = GameObject.Find("Player/camPivot/camRoot");
            PlayerCam.GetComponent<MainCameraControl>()._cinematicMode = true;

            yield return new WaitForSecondsRealtime(0.1f);
            MainCameraControl.main.ShakeCamera(0.5f, 4f, MainCameraControl.ShakeMode.Quadratic, 1f);

            while (!podlanded)
            {
                Player.main.SetPosition(EscapePod.main.transform.position + new Vector3(0, 0, 20));

                PlayerCam.GetComponent<MainCameraControl>().LookAt(EscapePod.main.transform.position);

                //LifePodRemastered.Logger.LogInfo("Anim");
                yield return null;
            }
            EndCutscene();
        }
        IEnumerator CustomIntro()
        {
            yield return new WaitForSecondsRealtime(10f);
            parachute.SetActive(true);
            parachute2.SetActive(true);
            parachute.GetComponent<Animation>().Play();
            parachute2.GetComponent<Animation>().Play();
            yield return new WaitForSecondsRealtime(3.1f);
            MainCameraControl.main.ShakeCamera(1f, 0.5f, MainCameraControl.ShakeMode.Linear, 1f);
        }

        IEnumerator CheckIfLanded()
        {
            LastPos = EscapePod.main.transform.position;
            yield return new WaitForSeconds(0.1f);

            //Debug.Log(LastPos);
            //Debug.Log(EscapePod.main.transform.position);

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
            Player.main.SetPosition(EscapePod.main.transform.position + new Vector3(0, 3, 0));
            Player.main.escapePod.Update(true);
        }
    }
}
