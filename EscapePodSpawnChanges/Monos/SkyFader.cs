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
    internal class SkyFader : MonoBehaviour
    {
        Renderer rend;
        float count = 1;

        // Start is called before the first frame update
        void Start()
        {
            rend = GetComponent<Renderer>();
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void startFadeOut()
        {
            count = 1;
            CoroutineHost.StartCoroutine(FadeOut());
        }
        IEnumerator FadeOut()
        {
            Debug.Log("started");
            while (count > 0.8f)
            {
                count -= 0.025f * Time.deltaTime;
                Color VisableColor = new Color(0, 0, 0, count);
                rend.material.SetColor("_Color", VisableColor);
                Debug.Log(count);
                yield return null;
            }
            while (count > 0f)
            {
                count -= 0.35f * Time.deltaTime;
                Color VisableColor = new Color(0, 0, 0, count);
                rend.material.SetColor("_Color", VisableColor);
                Debug.Log(count);
                yield return null;
            }

        }
    }
}
