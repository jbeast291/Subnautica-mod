using FMOD;
using FMODUnity;
using Nautilus.Handlers;
using Nautilus.Utility;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UWE;

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

    public Shader marmosetUber;

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
        if (!Info.newSave || !SaveUtils.settingsCache.CustomIntroToggle)//if the intro is disabled the loop must still run, otherwise let it start the loop after the intro
        {
            Nautilus.Utility.SaveUtils.RegisterOneTimeUseOnLoadEvent(onloaded);
        }
    }
    public void OnLoaded()
    {
        InitPodModelAndEffects();
        StartFreezeLoop();
        StartSpecIntLoop();
    }
    public void StartFreezeLoop()
    {
        CoroutineHost.StartCoroutine(FreezeLoop());
    }
    public void StartSpecIntLoop()
    {
        CoroutineHost.StartCoroutine(SpecularIntensityWithDepthLoop());
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
            BepInEx.Logger.LogError("Do not Init the Escape pod Model twice!!");
            return;
        }
        newLifePodModel = Instantiate(Info.assetBundle.LoadAsset<GameObject>("LifePodModel"));
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
        Sound pontoonInflate = CustomSoundHandler.RegisterCustomSound("pontoonInflate", Path.Combine(Info.PathToAudioFolder, "pontoonInflate.mp3"), AudioUtils.BusPaths.PDAVoice);
        pontoonInflate.setMode(MODE._3D);
        Sound pontoonDeflate = CustomSoundHandler.RegisterCustomSound("pontoonDeflate", Path.Combine(Info.PathToAudioFolder, "pontoonDeflate.mp3"), AudioUtils.BusPaths.PDAVoice);
        pontoonDeflate.setMode(MODE._3D);

        hasInit = true;
    }


    IEnumerator FreezeLoop()//dont move the pod if the player is not next to it, otherwise it will clip thru terrain
    {          
        if (SaveUtils.inGameSave.HeavyPodToggle)
        {
            wf.underwaterGravity = BepInEx.MyConfig.verticalMotionRate;
            HidePontoons(playAnimationNext);
        }
        else
        {
            wf.underwaterGravity = -1 * BepInEx.MyConfig.verticalMotionRate;
            ShowPontoons(playAnimationNext);
        }
        if (playAnimationNext)
        {
            playAnimationNext = false;
        }

        if (Vector3.Distance( Player.main.transform.position, EscapePod.main.transform.position) < 20)
        {
            EscapePod.main.GetComponent<Rigidbody>().isKinematic = false;

        } else
        {
            EscapePod.main.GetComponent<Rigidbody>().isKinematic = true;
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
