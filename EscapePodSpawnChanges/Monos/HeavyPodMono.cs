using Nautilus.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
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
        private void Awake()
        {
            if (main != null)
            {
                Debug.LogError("Duplicate HeavyPodMono found!");
                Destroy(this);
                return;
            }
            main = this;
        }
        public void Start()
        {
            Action loaded = onLoaded;
            if (!Info.newSave)
            {
                
                Nautilus.Utility.SaveUtils.RegisterOneTimeUseOnLoadEvent(loaded);
            } else {
                loaded.Invoke();
            }
        }
        public void onLoaded()
        {
            CoroutineHost.StartCoroutine(freezeLoop());
        }

        IEnumerator freezeLoop()//dont move the pod if the player is not next to it, otherwise it will clip thru terrain
        {
            if (SaveUtils.inGameSave.HeavyPodToggle)
            {
                wf.underwaterGravity = BepInEx.myConfig.verticalMotionRate;
            }
            else
            {
                wf.underwaterGravity = -1 * BepInEx.myConfig.verticalMotionRate;
            }

            if (Vector3.Distance( Player.main.transform.position, EscapePod.main.transform.position) < 20)
            {
                EscapePod.main.GetComponent<Rigidbody>().isKinematic = false;

            } else
            {
                EscapePod.main.GetComponent<Rigidbody>().isKinematic = true;
            }

            //FAIL SAFE if somehow the pod escapes the map it should be reset to origin
            //this should be impossible with the current protections but just in case never hurts 
            if(EscapePod.main.transform.position.y < -2000f)
            {
                EscapePod.main.transform.position = Vector3.zero;
            }
            yield return new WaitForSeconds(0.25f);
            CoroutineHost.StartCoroutine(freezeLoop());
        }
    }
}
