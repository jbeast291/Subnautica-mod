﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;

namespace LifePodRemastered
{
    internal class SkyFader : MonoBehaviour
    {
        Renderer rend;
        float count = 1;      
        void Start()
        {
            rend = GetComponent<Renderer>();
        }

        public void startFadeOut()
        {
            count = 1;
            CoroutineHost.StartCoroutine(FadeOut());
        }
        IEnumerator FadeOut()
        {
            while (count > 0.8f)
            {
                count -= 0.025f * Time.deltaTime;
                Color VisableColor = new Color(0, 0, 0, count);
                rend.material.SetColor("_Color", VisableColor);
                yield return null;
            }
            while (count > 0f)
            {
                count -= 0.35f * Time.deltaTime;
                Color VisableColor = new Color(0, 0, 0, count);
                rend.material.SetColor("_Color", VisableColor);
                yield return null;
            }

        }
    }
}
