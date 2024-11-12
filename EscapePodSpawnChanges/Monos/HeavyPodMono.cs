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
            if (!Info.newSave)
            {
                Action loaded = onLoaded;
                Nautilus.Utility.SaveUtils.RegisterOneTimeUseOnLoadEvent(loaded);
            } else {
                updateState();
            }
        }
        public void onLoaded()
        {
            updateState();
            CoroutineHost.StartCoroutine(freezeLoop());
        }
        public void updateState()
        {
            if (SaveUtils.inGameSave.HeavyPodToggle)
            {
                wf.underwaterGravity = 10f;
            } else
            {
                wf.underwaterGravity = -10f;
            }
            EscapePod.main.GetComponent<Rigidbody>().isKinematic = true;
            //EscapePod.main.GetComponent<Rigidbody>().freezeRotation = true;
        }

        IEnumerator freezeLoop()//dont move the pod if the player is not next to it, otherwise it will clip thru terrain
        {
            yield return new WaitForSeconds(1f);
            if (Vector3.Distance( new Vector3(Player.main.transform.position.x, Player.main.transform.position.y, Player.main.transform.position.z),
                new Vector3(EscapePod.main.transform.position.x, EscapePod.main.transform.position.y, EscapePod.main.transform.position.z)) < 20)
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
            CoroutineHost.StartCoroutine(freezeLoop());
        }
    }
}
