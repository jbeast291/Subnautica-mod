using FMOD;
using FMODUnity;
using Nautilus.Handlers;
using Nautilus.Utility;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UWE;
using static RootMotion.FinalIK.RagdollUtility;

namespace LifePodRemastered.Monos;

internal class HeavyPodMono : MonoBehaviour
{
    WorldForces wf = EscapePod.main.GetComponent<WorldForces>();

    public static HeavyPodMono main;

    //this should not be fucked with, double models causes(ed) major issues
    protected bool hasInit = false;

    GameObject newLifePodModel;
    GameObject pontoon;
    GameObject noPontoon;

    WaterClipProxy wcp;

    bool playAnimationNext = false;
    public bool playerRespawning;

    public Shader marmosetUber;

    //Interface
    GameObject uiRoot;
    public GameObject HeavyPodEnabledImage;
    public GameObject HeavyPodDisabledImage;

    public void Awake()
    {
        if (main != null)
        {
            UnityEngine.Debug.LogError("Duplicate HeavyPodMono found!");
            Destroy(this);
            return;
        }
        main = this;
    }
    public void Start()
    {
        Action onloaded = OnLoaded;
        if (!LPRGlobals.newSave || !SaveUtils.settingsCache.customIntroToggle)//if the intro is disabled the loop must still run, otherwise let it start the loop after the intro
        {
            Nautilus.Utility.SaveUtils.RegisterOneTimeUseOnLoadEvent(onloaded);
        }

    }
    public void OnLoaded()
    {
        InitPodModelAndEffects();
        StartFreezeLoop();
        StartSpecIntLoop();
        if (EscapePod.main.liveMixin.GetHealthFraction() >= 1)
        {
            CreatePodUI();
        }
    }
    public void StartFreezeLoop()
    {
        CoroutineHost.StartCoroutine(FreezeLoop());
    }
    public void StartSpecIntLoop()
    {
        CoroutineHost.StartCoroutine(SpecularIntensityWithDepthLoop());
    }
    public void CreatePodUI()
    {
        GameObject.Find("EscapePod/escapepod_weldablePoints/weldPoint_electricPannel").SetActive(false);
        uiRoot = Instantiate(LPRGlobals.assetBundle.LoadAsset<GameObject>("EscapePodPannelUi"));
        uiRoot.transform.SetParent(GameObject.Find("EscapePod/escapepod_weldablePoints").transform, false);
        uiRoot.transform.localPosition = new Vector3(-1.3994f, 1.7f, 1.6581f);
        uiRoot.transform.localRotation = Quaternion.Euler(0, 352.6214f, 0);
        uiRoot.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);

        HeavyPodHandTarget handTarget = uiRoot.transform.Find("HeavyPodToggleButton").gameObject.AddComponent<HeavyPodHandTarget>();
        HeavyPodDisabledImage = uiRoot.transform.Find("HeavyPodToggleButton").Find("Enabled").gameObject;
        HeavyPodEnabledImage = uiRoot.transform.Find("HeavyPodToggleButton").Find("Disabled").gameObject;
        UpdateUI();
    }
    public void ToggleHeavyPod(bool PlaySound)
    {
        if (PlaySound)
        {
            FMODUWE.PlayOneShot(Util.GetFmodAsset("event:/sub/cyclops/lights_on"), uiRoot.transform.position, 1f);
        }

        SaveUtils.inGameSave.HeavyPodToggle = !SaveUtils.inGameSave.HeavyPodToggle;
        SheduleAnimation();
        if (uiRoot != null)
        {
            UpdateUI();
        }

    }
    public void UpdateUI()
    {
        if (SaveUtils.inGameSave.HeavyPodToggle)
        {
            HeavyPodEnabledImage.SetActive(true);
            HeavyPodDisabledImage.SetActive(false);
            return;
        }
        HeavyPodEnabledImage.SetActive(false);
        HeavyPodDisabledImage.SetActive(true);
    }
    public void HidePontoons(bool animate)
    {
        if (!hasInit)
        {
            return;
        }

        //update water displacement to fit size
        wcp.distanceFieldMin = new Vector3(-5.4705f, -2.658f, -4.0242f);
        wcp.distanceFieldSize = new Vector3(10.9411f, 9.1471f, 8.2484f);
        wcp.UpdateMaterial();

        if (animate)
        {
            pontoon.GetComponent<Animator>().Play("PontoonDeflate");
            if (CustomSoundHandler.TryPlayCustomSound("pontoonDeflate", out Channel channel))
            {
                channel.setVolume(2f);
                VECTOR podLocation = EscapePod.main.transform.position.ToFMODVector();
                VECTOR vel = EscapePod.main.rigidbodyComponent.velocity.ToFMODVector();
                channel.set3DAttributes(ref podLocation, ref vel);
            }
        }else
        {
            pontoon.SetActive(false);
        }
    }

    public void ShowPontoons(bool animate)
    {
        if (!hasInit)
        {
            return;
        }
        pontoon.SetActive(true);

        //update water displacement to normal
        wcp.Rebuild();

        if (animate)
        {
            pontoon.GetComponent<Animator>().Play("PontoonDeploy");
            if (CustomSoundHandler.TryPlayCustomSound("pontoonInflate", out Channel channel))
            {
                channel.setVolume(1f);
                VECTOR podLocation = EscapePod.main.transform.position.ToFMODVector();
                VECTOR vel = EscapePod.main.rigidbodyComponent.velocity.ToFMODVector();
                channel.set3DAttributes(ref podLocation, ref vel);
            }
        }
    }
    public void SheduleAnimation()
    {
        playAnimationNext = true;
    }

    public void InitPodModelAndEffects() // by default the pontoons will show
    {
        if (hasInit)
        {
            BepInExEntry.Logger.LogError("Do not Init the Escape pod Model twice!!");
            return;
        }
        newLifePodModel = Instantiate(LPRGlobals.assetBundle.LoadAsset<GameObject>("LifePodModel"));
        newLifePodModel.transform.parent = EscapePod.main.gameObject.transform;
        newLifePodModel.transform.position = EscapePod.main.transform.position;
        newLifePodModel.transform.localPosition = new Vector3(-0.1468f, -0.2f, -0.0031f);
        newLifePodModel.transform.localScale = new Vector3(10000, 10000, 10000);
        newLifePodModel.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));//rotation fix needed

        SkinnedMeshRenderer oldSMRfromLifePod = GameObject.Find("EscapePod/models/Life_Pod_damaged_03/lifepod_damaged_03_geo/life_pod_damaged").GetComponent<SkinnedMeshRenderer>();

        this.marmosetUber = oldSMRfromLifePod.material.shader;

        //add MarmosetUber
        newLifePodModel.GetComponent<MeshRenderer>().material.shader = oldSMRfromLifePod.material.shader;
        // copy shader props
        newLifePodModel.GetComponent<MeshRenderer>().material.CopyPropertiesFromMaterial(oldSMRfromLifePod.material);

        // remove/hide old model
        GameObject.Find("EscapePod/models/Life_Pod_damaged_03/lifepod_damaged_03_geo/life_pod_damaged").SetActive(false);
        GameObject.Find("EscapePod/models/Life_Pod_damaged_LOD1/lifepod_damaged_03_geo/life_pod_damaged").SetActive(false);

        // change waterclip
        wcp = GameObject.Find("EscapePod/DistanceFieldProxy").GetComponent<WaterClipProxy>();
        wcp.distanceFieldMin = new Vector3(-5.4705f, -2.658f, -4.0242f);
        wcp.distanceFieldSize = new Vector3(10.9411f, 9.1471f, 8.2484f);
        wcp.UpdateMaterial();

        // decals
        GameObject decalsObj = newLifePodModel.FindChild("Decals");
        decalsObj.GetComponent<MeshRenderer>().material.shader = oldSMRfromLifePod.materials[1].shader;
        // copy shader props, note that there are multiple materials and the decals are always at index 1
        decalsObj.GetComponent<MeshRenderer>().material.CopyPropertiesFromMaterial(oldSMRfromLifePod.materials[1]);

        // No Pontoons Shaders/Material Properties (the texture is already applied from the Asset Bundle)
        noPontoon = newLifePodModel.FindChild("NoPontoons");
        noPontoon.GetComponent<MeshRenderer>().material.shader = oldSMRfromLifePod.material.shader;
        noPontoon.GetComponent<MeshRenderer>().material.shaderKeywords = new string[] { "MARMO_SPECMAP", "_ZWRITE_ON" };
        noPontoon.GetComponent<MeshRenderer>().material.SetFloat("_SpecInt", 0.1f);
        noPontoon.GetComponent<MeshRenderer>().material.SetFloat("_Fresnel", 0.9f);

        // Pontoons Shaders/Material Properties
        pontoon = newLifePodModel.FindChild("Pontoons");
        pontoon.GetComponent<MeshRenderer>().material.shader = oldSMRfromLifePod.material.shader;
        pontoon.GetComponent<MeshRenderer>().material.CopyPropertiesFromMaterial(oldSMRfromLifePod.material);

        // Sounds
        Sound pontoonInflate = CustomSoundHandler.RegisterCustomSound("pontoonInflate", Path.Combine(LPRGlobals.PathToAudioFolder, "pontoonInflate.mp3"), AudioUtils.BusPaths.PDAVoice);
        pontoonInflate.setMode(MODE._3D);
        Sound pontoonDeflate = CustomSoundHandler.RegisterCustomSound("pontoonDeflate", Path.Combine(LPRGlobals.PathToAudioFolder, "pontoonDeflate.mp3"), AudioUtils.BusPaths.PDAVoice);
        pontoonDeflate.setMode(MODE._3D);

        hasInit = true;
    }

    IEnumerator FreezeLoop()//dont move the pod if the player is not next to it, otherwise it will clip thru terrain
    {
        //Gravity And animations
        if (SaveUtils.inGameSave.HeavyPodToggle)
        {
            wf.underwaterGravity = SaveUtils.settingsCache.VertialMotionRate;
            wf.aboveWaterGravity = wf.underwaterGravity;
            HidePontoons(playAnimationNext);
        }
        else
        {
            wf.underwaterGravity = -1 * SaveUtils.settingsCache.VertialMotionRate;
            wf.aboveWaterGravity = 9.81f;
            ShowPontoons(playAnimationNext);
        }
        if (playAnimationNext)
        {
            playAnimationNext = false;
        }

        //Freeze System
        if (playerRespawning || Vector3.Distance(Player.main.transform.position, EscapePod.main.transform.position) > 20)
        {
            EscapePod.main.GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            EscapePod.main.GetComponent<Rigidbody>().isKinematic = false;
        }

        yield return new WaitForSeconds(0.25f);
        CoroutineHost.StartCoroutine(FreezeLoop());
    }
    IEnumerator SpecularIntensityWithDepthLoop()
    {
        float surfaceValue = 1.5f;
        if (EscapePod.main.transform.position.y < -1)
        {
            float depth = Mathf.Abs(EscapePod.main.transform.position.y);
            float maxDepth = 30f;

            // value will be surfaceValue at depth=0, and approach 0 as depth approaches maxDepth
            float underwater = surfaceValue * Mathf.Clamp01(1 - (depth / maxDepth));

            newLifePodModel.GetComponent<MeshRenderer>().material.SetFloat("_SpecInt", underwater);
            noPontoon.GetComponent<MeshRenderer>().material.SetFloat("_SpecInt", underwater/15);
            pontoon.GetComponent<MeshRenderer>().material.SetFloat("_SpecInt", underwater);
        }
        else
        {
            newLifePodModel.GetComponent<MeshRenderer>().material.SetFloat("_SpecInt", surfaceValue);
            noPontoon.GetComponent<MeshRenderer>().material.SetFloat("_SpecInt", surfaceValue/15);
            pontoon.GetComponent<MeshRenderer>().material.SetFloat("_SpecInt", surfaceValue);
        }
        yield return new WaitForSeconds(0.25f);
        CoroutineHost.StartCoroutine(SpecularIntensityWithDepthLoop());
    }
}
