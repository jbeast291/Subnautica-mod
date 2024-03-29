﻿using HarmonyLib;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UWE;
using static LifePodRemastered.SaveManager;
using LifePodRemastered;
using UnityEngine.UI;
using mset;
using System;
using Nautilus.Utility;
using TMPro;
using LifePodRemastered.Fade_Sfxmonos;
using FMOD;
using Nautilus.Handlers;

namespace LifePodRemastered
{
    internal class EscapePodCustomIntro : MonoBehaviour
    {
        static AssetBundle assetBundle = Info.assetBundle;

        WorldForces wf = EscapePod.main.GetComponent<WorldForces>();

        //para
        GameObject parachute;
        GameObject parachute2;

        //orbit
        GameObject FireVfx;
        GameObject FireVfx2;
        GameObject FireVfx3;
        GameObject FakeSpace;


        //restraints
        GameObject PlayerBody;
        GameObject PlayerCam;
        OxygenManager oxygenManager;

        //logic
        bool FirstSequenceDone = false;
        Vector3 LastPos;
        bool podlanded;
        bool podHitWater;
        float WfAboveOffset = 0f;
        float WfUnderOffset = 0f;
        float FirstZoomOffset = 1.001f;
        float FirstMove1 = 0;
        float FirstMove2 = 0;

        //UI
        GameObject CineUiEmpty;
        GameObject PressAnyButtonText;
        GameObject PresentingText;
        GameObject SubnauticaLogoImage;
        GameObject BlackBackground;


        FMOD.Sound FirstAudio;


        public void Awake()
        {
            //register onloaded to be called on game finished loading. waiting for game to load essentially
            Action loaded = Onloaded;
            SaveUtils.RegisterOneTimeUseOnLoadEvent(loaded);

            //freeze escape pod so it doesnt move while the player is looking at the starting ui
            EscapePod.main.GetComponent<Rigidbody>().isKinematic = true;

            EscapePod.main.GetComponent<Rigidbody>().freezeRotation = true;
        }

        public void Start()
        {
            oxygenManager = Player.main.GetComponent<OxygenManager>();
        }

        void Onloaded()//called once the game is loaded
        {
            // start the game intro when the game is loaded
            SetupUi();
            SetupAudio();
        }
        public void SetupAudio()
        {
            FirstAudio = CustomSoundHandler.RegisterCustomSound("FirstAudio", Path.Combine(Info.PathToAudioFolder, "SubnauticaIntro-01.wav"), AudioUtils.BusPaths.PlayerSFXs);
        }

        public void SetupUi()
        {
            CineUiEmpty = Instantiate(assetBundle.LoadAsset<GameObject>("CustomIntroCanvas"));
            CineUiEmpty.transform.parent = GameObject.Find("uGUI(Clone)").transform;
            CineUiEmpty.transform.position = new Vector3(0, 0, 1);
            CineUiEmpty.transform.localScale = new Vector3(1, 1, 1);

            PressAnyButtonText = GameObject.Find("uGUI(Clone)/CustomIntroCanvas(Clone)/BlackBackground/PressAnyButton");
            PressAnyButtonText.AddComponent<TextFader>();
            PressAnyButtonText.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 0);
            //Set text to be transparent. tried seting the object active and setting this in the editor but it caused issues regarless

            PresentingText = GameObject.Find("uGUI(Clone)/CustomIntroCanvas(Clone)/BlackBackground/PresentingText");
            PresentingText.AddComponent<TextFader>();
            PresentingText.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, 0);

            SubnauticaLogoImage = GameObject.Find("uGUI(Clone)/CustomIntroCanvas(Clone)/BlackBackground/SubnauticaLogo");
            SubnauticaLogoImage.AddComponent<ImageFader>();
            SubnauticaLogoImage.GetComponent<Image>().color = new Color(1, 1, 1, 0);

            //get ref to the background
            BlackBackground = GameObject.Find("uGUI(Clone)/CustomIntroCanvas(Clone)/BlackBackground");
            BlackBackground.AddComponent<ImageFader>();


            //start sequence
            CoroutineHost.StartCoroutine(Sequence1BlackBackground());
        }
        IEnumerator Sequence1BlackBackground()//press any button, presenting, subnautica, fade out of black and see pod
        {
            CoroutineHost.StartCoroutine(LockCameraOutsidePod());


            //Set black background fader to be the right color
            BlackBackground.GetComponent<ImageFader>().SetStartingColor(Color.black);

            //Press any button text fading
            PressAnyButtonText.GetComponent<TextFader>().startFadeIn(50);//start fading in text
            yield return new WaitUntil(() => PressAnyButtonText.GetComponent<TextFader>().DoneFadeIn == true); // wait for finished fading
            yield return new WaitUntil(() => Input.anyKeyDown == true); // check input
            PressAnyButtonText.GetComponent<TextFader>().startFadeOut(50);//start fading out text
            yield return new WaitUntil(() => PressAnyButtonText.GetComponent<TextFader>().DoneFadeOut == true); // wait for finished fading
            PressAnyButtonText.SetActive(false);

            //Presenting Text fading
            PresentingText.GetComponent<TextFader>().startFadeIn(50);
            yield return new WaitUntil(() => PresentingText.GetComponent<TextFader>().DoneFadeIn == true); // wait for finished fading
            yield return new WaitForSecondsRealtime(1);//show for a moment

            //start playing audio
            if(CustomSoundHandler.TryPlayCustomSound("FirstAudio", out Channel channel))
            {
                channel.setVolume(2f);
            }

            yield return new WaitForSecondsRealtime(2);
            PresentingText.GetComponent<TextFader>().startFadeOut(50);
            yield return new WaitUntil(() => PresentingText.GetComponent<TextFader>().DoneFadeOut == true); // wait for finished fading
            PressAnyButtonText.SetActive(false);

            //Subnautica Logo fading
            SubnauticaLogoImage.GetComponent<ImageFader>().startFadeIn(50);
            yield return new WaitUntil(() => SubnauticaLogoImage.GetComponent<ImageFader>().DoneFadeIn == true); // wait for finished fading
            yield return new WaitForSecondsRealtime(5.25f);

            //Fade out text
            SubnauticaLogoImage.GetComponent<ImageFader>().startFadeOut(50);
            yield return new WaitUntil(() => SubnauticaLogoImage.GetComponent<ImageFader>().DoneFadeOut == true); // wait for finished fading
            yield return new WaitForSeconds(0.25f);
            
            //startup second sequence
            FirstSequenceDone = true;
            Setup2Sequence();

            //Fade out black background
            BlackBackground.GetComponent<ImageFader>().startFadeOut(50);
            yield return new WaitUntil(() => BlackBackground.GetComponent<ImageFader>().DoneFadeOut == true); // wait for finished fading

        }

        IEnumerator LockCameraOutsidePod()// workaround to remove the inside noises from the pod
        {
            while (!FirstSequenceDone)
            {
                Player.main.SetPosition(EscapePod.main.transform.position + new Vector3(0, 20, 0));
                PlayerCam.GetComponent<MainCameraControl>().LookAt(EscapePod.main.transform.position);
                yield return null;
            }
        }




        // Sequene 2 Below

        void Setup2Sequence()
        {
            GameObject.Find("StaticOverlay").AddComponent<StaticOverlayShaker>();

            //unfreeze pod
            EscapePod.main.GetComponent<Rigidbody>().isKinematic = false;

            SetupParachutes();
            SetupFirevfx();
            setupFakeSpace();
                

            CoroutineHost.StartCoroutine(Sequence2PodFalling());
            //CoroutineHost.StartCoroutine(Parachute());
            CoroutineHost.StartCoroutine(CheckIfLanded());
            CoroutineHost.StartCoroutine(CheckIfUnderwater());
            CoroutineHost.StartCoroutine(LockCamera());

        }


       void SetupFirevfx()
        {
            FireVfx = Instantiate(assetBundle.LoadAsset<GameObject>("FireEmbers"));
            FireVfx.transform.parent = EscapePod.main.gameObject.transform;
            FireVfx.transform.position = EscapePod.main.transform.position;
            FireVfx.transform.localPosition = new Vector3(2.5f, -1.2891f, 0);
            FireVfx.SetActive(false);

            FireVfx2 = Instantiate(assetBundle.LoadAsset<GameObject>("FireEmbers"));
            FireVfx2.transform.parent = EscapePod.main.gameObject.transform;
            FireVfx2.transform.position = EscapePod.main.transform.position;
            FireVfx2.transform.localPosition = new Vector3(-2.5f, -1.2891f, 0);
            FireVfx2.SetActive(false);

            FireVfx3 = Instantiate(assetBundle.LoadAsset<GameObject>("FlameThrower Variant"));
            FireVfx3.transform.parent = EscapePod.main.gameObject.transform;
            FireVfx3.transform.position = EscapePod.main.transform.position;
            FireVfx3.transform.localPosition = new Vector3(0, 0, 0);
            FireVfx3.SetActive(false);
        }
        void SetupParachutes()
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
        void setupFakeSpace()
        {
            FakeSpace = Instantiate(assetBundle.LoadAsset<GameObject>("BlackSPhere1"));
            FakeSpace.transform.parent = EscapePod.main.gameObject.transform;
            FakeSpace.transform.position = EscapePod.main.transform.position;
            FakeSpace.transform.localPosition = new Vector3(0, 0, 0);
            FakeSpace.transform.localScale = new Vector3(20000, 20000, 20000);
            FakeSpace.SetActive(true);
            FakeSpace.gameObject.EnsureComponent<SkyFader>();
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))//event:/player/footstep_metal
            {
                Utils.StopAllFMODEvents(Player.main.gameObject, false);

            }
            if (Input.GetKeyDown(KeyCode.P))//event:/player/footstep_metal
            {
                Utils.PlayFMODAsset(Util.GetFmodAsset("event:/sub/cyclops/docking_doors_close"), Player.main.transform);
                
                
            }
            if (Input.GetKeyDown(KeyCode.I))//event:/player/footstep_metal
            {
                Utils.PlayFMODAsset(Util.GetFmodAsset("event:/sub/cyclops/docking_doors_open"), Player.main.transform);


            }
            if (Input.GetKeyDown(KeyCode.L))//event:/player/footstep_metal
            {
                Utils.PlayFMODAsset(Util.GetFmodAsset("event:/sub/cyclops/cyclops_door_close"), Player.main.transform);


            }
            if (Input.GetKeyDown(KeyCode.K))//event:/player/footstep_metal
            {
                Utils.PlayFMODAsset(Util.GetFmodAsset("event:/sub/cyclops/cyclops_door_open"), Player.main.transform);


            }
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
              }
            */
            //event:/sub/base/door_close

        }
        IEnumerator Sequence2PodFalling()
        {
            WriteSettingsToCurrentSlot();
            wf.aboveWaterGravity = 500f;
            wf.underwaterGravity = 10f;
            ToggleHud(false);

            yield return new WaitForSecondsRealtime(10f);
            //entering atmosphere
            FireVfx.SetActive(true);
            FireVfx2.SetActive(true);
            FireVfx3.SetActive(true);
            MainCameraControl.main.ShakeCamera(10f, 10f, MainCameraControl.ShakeMode.BuildUp, 1f);
            yield return new WaitForSecondsRealtime(1f);
            FakeSpace.GetComponent<SkyFader>().startFadeOut();
            yield return new WaitForSecondsRealtime(10f);

            CrashedShipExploder.main.PlayExplosionFX();
            CrashedShipExploder.main.ShakePlayerCamera();
            //Utils.PlayFMODAsset(SaveManager.GetFmodAsset("event:/env/music/aurora_reveal"), Player.main.transform);
            CrashedShipExploder.main.shipExplodeSound.StartEvent();

            //deploy parachutes
            yield return new WaitForSecondsRealtime(10f);
            StartParaAnim();

            //deploy parachutes shake when open
            yield return new WaitForSecondsRealtime(1.25f);
            MainCameraControl.main.ShakeCamera(1f, 0.5f, MainCameraControl.ShakeMode.Linear, 1f);
            CoroutineHost.StartCoroutine(WfAboveScaler());

            //wait for the pod to hit the water
            while (!podHitWater)
            {
                yield return null;
            }
            CoroutineHost.StartCoroutine(WfWaterScaler());
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
            Destroy(parachute);
        }

        IEnumerator LockCamera()
        {
            PlayerBody = GameObject.Find("Player/body");
            PlayerBody.SetActive(false);
            PlayerCam = GameObject.Find("Player/camPivot/camRoot");
            PlayerCam.GetComponent<MainCameraControl>()._cinematicMode = true;

            yield return new WaitForSecondsRealtime(0.1f);
            MainCameraControl.main.ShakeCamera(0.5f, 4f, MainCameraControl.ShakeMode.Quadratic, 1f);

            while (FirstZoomOffset <= 15)
            {
                Player.main.SetPosition(EscapePod.main.transform.position + new Vector3(0, 0, 5 + FirstZoomOffset));

                PlayerCam.GetComponent<MainCameraControl>().LookAt(EscapePod.main.transform.position);

                FirstZoomOffset += (FirstZoomOffset * FirstZoomOffset) * Time.deltaTime;

                yield return null;
            }

            while (EscapePod.main.transform.position.y > 0)
            {
                Player.main.SetPosition(EscapePod.main.transform.position + new Vector3(0, 0, 20));

                PlayerCam.GetComponent<MainCameraControl>().LookAt(EscapePod.main.transform.position);

                oxygenManager.AddOxygen(10f);

                yield return null;
            }

            while (FirstMove1 <= 10 && FirstMove2 <= 10)
            {
                Player.main.SetPosition(EscapePod.main.transform.position + new Vector3(0, 0 + FirstMove1, 20 - FirstMove2));

                PlayerCam.GetComponent<MainCameraControl>().LookAt(EscapePod.main.transform.position);

                FirstMove1 += 1f * Time.deltaTime;
                FirstMove2 += 1f * Time.deltaTime;

                oxygenManager.AddOxygen(10f);

                yield return null;
            }

            while (!podlanded)
            {
                Player.main.SetPosition(EscapePod.main.transform.position + new Vector3(0, 10, 10));

                PlayerCam.GetComponent<MainCameraControl>().LookAt(EscapePod.main.transform.position);

                oxygenManager.AddOxygen(10f);

                yield return null;
            }
            EndCutscene();
        }
        IEnumerator WfAboveScaler()
        {
            yield return new WaitForSecondsRealtime(0.02f);
            WfAboveOffset += 1;
            wf.aboveWaterGravity = 500 - WfAboveOffset;

            if (WfAboveOffset == 150)
            {
                yield break;
            }
            CoroutineHost.StartCoroutine(WfAboveScaler());
        }
        IEnumerator WfWaterScaler()
        {
            yield return new WaitForSecondsRealtime(0.05f);
            WfUnderOffset += 1;
            wf.underwaterGravity = 30 + WfUnderOffset;
            if (WfUnderOffset == 240)
            {
                yield break;
            }
            CoroutineHost.StartCoroutine(WfWaterScaler());
        }

        IEnumerator CheckIfUnderwater()
        {
            yield return new WaitForSeconds(0.01f);
            if (EscapePod.main.transform.position.y <= 0)
            {
                podHitWater = true;
                yield break;
            }
            CoroutineHost.StartCoroutine(CheckIfUnderwater());
        }

        IEnumerator CheckIfLanded()
        {
            LastPos = EscapePod.main.transform.position;
            yield return new WaitForSeconds(0.1f);

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
            ToggleHud(true);
        }
        void StartParaAnim()
        {
            parachute.SetActive(true);
            parachute2.SetActive(true);
            parachute.GetComponent<Animation>().Play();
            parachute2.GetComponent<Animation>().Play();
        }

        void ToggleHud(bool ToggleValue)
        {
            Player.main._cinematicModeActive = !ToggleValue;
            GameObject.Find("Player/camPivot/camRoot/camOffset/pdaCamPivot/SpawnPlayerMask").SetActive(ToggleValue);
            GameObject.Find("uGUI(Clone)/ScreenCanvas/HUD/Content/HandReticle").SetActive(ToggleValue);

        }
    }
}
