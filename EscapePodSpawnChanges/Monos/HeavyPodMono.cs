using FMOD;
using FMODUnity;
using Nautilus.Handlers;
using Nautilus.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;

namespace LifePodRemastered.Monos
{
    internal class HeavyPodMono : MonoBehaviour
    {
        WorldForces wf = EscapePod.main.GetComponent<WorldForces>();

        public static HeavyPodMono main;

        //this should not be fucked with, double models causes(ed) major issues
        protected bool hasInit = false;

        GameObject pontoon;
        GameObject noPontoon;

        WaterClipProxy wcp;

        bool playAnimationNext = false;

        private void Awake()
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
            Action onloaded = onLoaded;
            if (!Info.newSave)
            {
                Nautilus.Utility.SaveUtils.RegisterOneTimeUseOnLoadEvent(onloaded);
            } else {
                //onloaded.Invoke();
            }
        }
        public void onLoaded()
        {
            initPodModelAndEffects();
            startLoop();
        }
        public void startLoop()
        {
            CoroutineHost.StartCoroutine(freezeLoop());
        }
        public void hidePontoons(bool animate)
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
                //disable the pontoon after the animation is complete
                //CoroutineHost.StartCoroutine(HidePontoonsLater(Util.clipLength(pontoon.GetComponent<Animator>(), "PontoonDeflate")));
            }else
            {
                pontoon.SetActive(false);
            }
        }
        
        private IEnumerator HidePontoonsLater(float time)
        {
            UnityEngine.Debug.Log(time);
            yield return new WaitForSeconds(time + 0.01f);
            yield return new WaitForEndOfFrame();
            //ensure it should still be disabled
            if (SaveUtils.inGameSave.HeavyPodToggle)
            {
                UnityEngine.Debug.Log(time);
                pontoon.SetActive(false);
            }
               
        }

        public void showPontoons(bool animate)
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
        public void sheduleAnimation()
        {
            playAnimationNext = true;
        }


        public void initPodModelAndEffects() // by default the pontoons will show
        {
            if (hasInit)
            {
                BepInEx.Logger.LogError("Do not Init the Escape pod Model twice!!");
                return;
            }
            GameObject testingObj = Instantiate(Info.assetBundle.LoadAsset<GameObject>("LifePodModel"));
            testingObj.transform.parent = EscapePod.main.gameObject.transform;
            testingObj.transform.position = EscapePod.main.transform.position;
            testingObj.transform.localPosition = new Vector3(-0.1468f, -0.2f, -0.0031f);
            testingObj.transform.localScale = new Vector3(10000, 10000, 10000);
            testingObj.transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));//rotation fix needed

            SkinnedMeshRenderer oldSMRfromLifePod = GameObject.Find("EscapePod/models/Life_Pod_damaged_03/lifepod_damaged_03_geo/life_pod_damaged").GetComponent<SkinnedMeshRenderer>();

            //add MarmosetUber
            testingObj.GetComponent<MeshRenderer>().material.shader = oldSMRfromLifePod.material.shader;
            // copy shader props
            testingObj.GetComponent<MeshRenderer>().material.CopyPropertiesFromMaterial(oldSMRfromLifePod.material);

            // remove/hide old model
            GameObject.Find("EscapePod/models/Life_Pod_damaged_03/lifepod_damaged_03_geo/life_pod_damaged").SetActive(false);
            GameObject.Find("EscapePod/models/Life_Pod_damaged_LOD1/lifepod_damaged_03_geo/life_pod_damaged").SetActive(false);

            // change waterclip
            wcp = GameObject.Find("EscapePod/DistanceFieldProxy").GetComponent<WaterClipProxy>();
            wcp.distanceFieldMin = new Vector3(-5.4705f, -2.658f, -4.0242f);
            wcp.distanceFieldSize = new Vector3(10.9411f, 9.1471f, 8.2484f);
            wcp.UpdateMaterial();

            // decals
            GameObject decalsObj = testingObj.FindChild("Decals");
            decalsObj.GetComponent<MeshRenderer>().material.shader = oldSMRfromLifePod.materials[1].shader;
            // copy shader props, note that there are multiple materials and the decals are always at index 1
            decalsObj.GetComponent<MeshRenderer>().material.CopyPropertiesFromMaterial(oldSMRfromLifePod.materials[1]);

            // No Pontoons Shaders/Material Properties (the texture is already applied from the Asset Bundle)
            noPontoon = testingObj.FindChild("NoPontoons");
            noPontoon.GetComponent<MeshRenderer>().material.shader = oldSMRfromLifePod.material.shader;
            noPontoon.GetComponent<MeshRenderer>().material.shaderKeywords = new string[] { "MARMO_SPECMAP", "_ZWRITE_ON" };
            noPontoon.GetComponent<MeshRenderer>().material.SetFloat("_SpecInt", 0.1f);
            noPontoon.GetComponent<MeshRenderer>().material.SetFloat("_Fresnel", 0.9f);

            // Pontoons Shaders/Material Properties
            pontoon = testingObj.FindChild("Pontoons");
            pontoon.GetComponent<MeshRenderer>().material.shader = oldSMRfromLifePod.material.shader;
            pontoon.GetComponent<MeshRenderer>().material.CopyPropertiesFromMaterial(oldSMRfromLifePod.material);

            // Sounds
            Sound pontoonInflate = CustomSoundHandler.RegisterCustomSound("pontoonInflate", Path.Combine(Info.PathToAudioFolder, "pontoonInflate.mp3"), AudioUtils.BusPaths.PDAVoice);
            pontoonInflate.setMode(MODE._3D);
            Sound pontoonDeflate = CustomSoundHandler.RegisterCustomSound("pontoonDeflate", Path.Combine(Info.PathToAudioFolder, "pontoonDeflate.mp3"), AudioUtils.BusPaths.PDAVoice);
            pontoonDeflate.setMode(MODE._3D);

            hasInit = true;
        }


        IEnumerator freezeLoop()//dont move the pod if the player is not next to it, otherwise it will clip thru terrain
        {          
            if (SaveUtils.inGameSave.HeavyPodToggle)
            {
                wf.underwaterGravity = BepInEx.myConfig.verticalMotionRate;
                hidePontoons(playAnimationNext);
            }
            else
            {
                wf.underwaterGravity = -1 * BepInEx.myConfig.verticalMotionRate;
                showPontoons(playAnimationNext);
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
            CoroutineHost.StartCoroutine(freezeLoop());
        }
    }
}
