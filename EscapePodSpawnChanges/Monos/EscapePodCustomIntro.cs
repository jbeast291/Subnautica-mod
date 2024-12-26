using HarmonyLib;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UWE;
using static LifePodRemastered.SaveUtils;
using LifePodRemastered;
using UnityEngine.UI;
using mset;
using System;
using Nautilus.Utility;
using TMPro;
using LifePodRemastered.Fade_Sfxmonos;
using FMOD;
using Nautilus.Handlers;
using LifePodRemastered.Monos;
using static VFXParticlesPool;

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
        GameObject SmokeVfx;
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
        GameObject PodFallOverlay;


        FMOD.Sound FirstAudio;

        public void Awake()
        {
            //register onloaded to be called on game finished loading. waiting for game to load essentially
            Action loaded = Onloaded;
            Nautilus.Utility.SaveUtils.RegisterOneTimeUseOnLoadEvent(loaded);

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

            //disable pausing
            IngameMenu.main.canBeOpen = false;

            //for disabling pda notifs durring cine
            Info.CinematicActive = true;
            Info.mutePdaEvents = true;
        }
        public void SetupAudio()
        {
            //register audio from wavs to be played with fmod
            FirstAudio = CustomSoundHandler.RegisterCustomSound("IntroExplosions", Path.Combine(Info.PathToAudioFolder, "SubnauticaIntro-01.wav"), AudioUtils.BusPaths.PlayerSFXs);
            FirstAudio = CustomSoundHandler.RegisterCustomSound("IntroEnterOrbit", Path.Combine(Info.PathToAudioFolder, "SubnauticaIntro-02.wav"), AudioUtils.BusPaths.PlayerSFXs);
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

            PodFallOverlay = GameObject.Find("uGUI(Clone)/CustomIntroCanvas(Clone)/PodFallOverlay");


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
            yield return new WaitForSeconds(1);//show for a moment

            //start playing audio
            if(CustomSoundHandler.TryPlayCustomSound("IntroExplosions", out Channel channel))
            {
                channel.setVolume(2f);
            }

            yield return new WaitForSeconds(2);
            PresentingText.GetComponent<TextFader>().startFadeOut(50);
            yield return new WaitUntil(() => PresentingText.GetComponent<TextFader>().DoneFadeOut == true); // wait for finished fading
            PressAnyButtonText.SetActive(false);

            //Subnautica Logo fading
            SubnauticaLogoImage.GetComponent<ImageFader>().startFadeIn(50);
            yield return new WaitUntil(() => SubnauticaLogoImage.GetComponent<ImageFader>().DoneFadeIn == true); // wait for finished fading
            yield return new WaitForSeconds(6.5f);//5.25

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
            PlayerCam = GameObject.Find("Player/camPivot/camRoot");
            PlayerBody = GameObject.Find("Player/body");

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
            if (!SaveUtils.settingsCache.CinematicOverlayToggle)
            {
                PodFallOverlay.SetActive(false);
            } else
            {
                PodFallOverlay.FindChild("StaticOverlay").AddComponent<StaticOverlayShaker>();
            }

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

            SmokeVfx = Instantiate(assetBundle.LoadAsset<GameObject>("Smoke"));
            SmokeVfx.transform.parent = EscapePod.main.gameObject.transform;
            SmokeVfx.transform.position = EscapePod.main.transform.position;
            SmokeVfx.transform.localPosition = new Vector3(0, 0, 0);
            SmokeVfx.SetActive(false);
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
        IEnumerator Sequence2PodFalling()
        {
            wf.aboveWaterGravity = 500f;
            ToggleHud(false);

            MainCameraControl.main.ShakeCamera(0.5f, 2.5f, MainCameraControl.ShakeMode.Sqrt, 1f);//small shake so it doesnt look so stiff at the start

            yield return new WaitForSeconds(2.5f);

            // entering atmosphere

            FireVfx.SetActive(true);
            FireVfx2.SetActive(true);

            if (CustomSoundHandler.TryPlayCustomSound("IntroEnterOrbit", out Channel channel))
            {
                channel.setVolume(2f);
            }

            MainCameraControl.main.ShakeCamera(1, 10f, MainCameraControl.ShakeMode.Sqrt, 2f);
            yield return new WaitForSeconds(10f);

            //fire is enabled and the sky starts to fade, roaring more intense 
            FakeSpace.GetComponent<SkyFader>().startFadeOut();
            FireVfx3.SetActive(true);
            MainCameraControl.main.ShakeCamera(3, 4f, MainCameraControl.ShakeMode.Sqrt, 2f);
            yield return new WaitForSeconds(2f);

            //beeping starts
            MainCameraControl.main.ShakeCamera(5, 6f, MainCameraControl.ShakeMode.Sqrt, 2f);
            yield return new WaitForSeconds(4f);

            //after shake,smoke effect after
            MainCameraControl.main.ShakeCamera(3f, 2f, MainCameraControl.ShakeMode.Sqrt, 1f);
            SmokeVfx.SetActive(true);
            yield return new WaitForSeconds(2f);
            MainCameraControl.main.ShakeCamera(0.5f, 2f, MainCameraControl.ShakeMode.Sqrt, 1f);

            
            yield return new WaitForSeconds(10f);

            //deploy parachutes
            StartParaAnim();

            //deploy parachutes shake when open
            yield return new WaitForSeconds(1.25f);
            MainCameraControl.main.ShakeCamera(1f, 0.5f, MainCameraControl.ShakeMode.Linear, 1f);
            CoroutineHost.StartCoroutine(WfAboveScaler());

            //wait for the pod to hit the water
            while (!podHitWater)
            {
                yield return null;
            }
            if (settingsCache.HeavyPodToggle)
            {
                CoroutineHost.StartCoroutine(WfWaterScaler());
            }
        }

        IEnumerator Parachute()
        {
            yield return new WaitForSeconds(0.1f);
            while (parachute.transform.position.y > -40.9f)
            {
                yield return null;
            }
            parachute.GetComponent<Animation>().Play();
            yield return new WaitForSeconds(5.1f);
            Destroy(parachute);
        }

        IEnumerator LockCamera()
        {
            //obtaining refs moved to first sequence
            PlayerBody.SetActive(false);
            PlayerCam.GetComponent<MainCameraControl>()._cinematicMode = true;

            while (FirstZoomOffset <= 15)
            {
                Player.main.SetPosition(EscapePod.main.transform.position + new Vector3(0, 0, 5 + FirstZoomOffset));

                PlayerCam.GetComponent<MainCameraControl>().LookAt(EscapePod.main.transform.position);

                FirstZoomOffset += (FirstZoomOffset * FirstZoomOffset) * Time.deltaTime;

                yield return null;
            }

            while (EscapePod.main.transform.position.y > 0 && !podlanded)
            {
                Player.main.SetPosition(EscapePod.main.transform.position + new Vector3(0, 0, 20));

                PlayerCam.GetComponent<MainCameraControl>().LookAt(EscapePod.main.transform.position);

                oxygenManager.AddOxygen(10f);

                yield return null;
            }

            while (FirstMove1 <= 10 && FirstMove2 <= 10 && !podlanded)
            {
                Player.main.SetPosition(EscapePod.main.transform.position + new Vector3(0, 0 + FirstMove1, 20 - FirstMove2));

                PlayerCam.GetComponent<MainCameraControl>().LookAt(EscapePod.main.transform.position);

                FirstMove1 += 1f * Time.deltaTime;
                FirstMove2 += 1f * Time.deltaTime;

                oxygenManager.AddOxygen(10f);

                yield return null;
            }

            //move this code to its own place should not be here :/
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
            yield return new WaitForSeconds(0.02f);
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
            yield return new WaitForSeconds(0.05f);
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
                Info.mutePdaEvents = false;//depth notifs to play when sinking
                yield break;
            }
            CoroutineHost.StartCoroutine(CheckIfUnderwater());
        }

        IEnumerator CheckIfLanded()
        {
            LastPos = EscapePod.main.transform.position;
            yield return new WaitForSeconds(0.5f);

            if (EscapePod.main.transform.position.y <= (LastPos.y + 0.5) && 
                EscapePod.main.transform.position.y >= (LastPos.y - 0.5) &&
                Time.deltaTime != 0)
            {
                podlanded = true;
                yield break;
            }
            CoroutineHost.StartCoroutine(CheckIfLanded());
        }

        void EndCutscene()
        {
            //reset pod above water gravity to normal
            wf.aboveWaterGravity = 9.8f; 

            //first person body
            PlayerBody.SetActive(true);
            //give back camera controll
            PlayerCam.GetComponent<MainCameraControl>()._cinematicMode = false;

            //put player in escape pod
            Player.main.SetPosition(EscapePod.main.transform.position + new Vector3(0, 3, 0));
            //update their state so they are considered walking and not swimming inside the escape pod
            Player.main.escapePod.Update(true);
            //reset the players velocity just incase they built up a lot and when set inside hte pod they may die from fall damage otherwise
            Player.main.groundMotor.SetVelocity(Vector3.zero);
            //reenable hud
            ToggleHud(true);

            //renable pausing
            IngameMenu.main.canBeOpen = true;

            //renable pda notifs
            Info.CinematicActive = false;
            Info.mutePdaEvents = false;

            //destroy unneeded gameobjects
            Destroy(parachute);
            Destroy(parachute2);
            Destroy(FireVfx);
            Destroy(FireVfx2);
            Destroy(FireVfx3);
            Destroy(SmokeVfx);
            Destroy(FakeSpace);
            Destroy(CineUiEmpty);

            //add Heavy pod script to this pod
            EscapePod.main.gameObject.EnsureComponent<HeavyPodMono>();

            //remove script from object
            Destroy(this);
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
