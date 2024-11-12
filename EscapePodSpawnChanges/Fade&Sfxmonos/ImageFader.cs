using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UWE;

namespace LifePodRemastered.Fade_Sfxmonos
{
    internal class ImageFader : MonoBehaviour
    {
        Image img;
        float count = 1;
        public bool DoneFadeOut = false;
        public bool DoneFadeIn = false;

        //Defualt start color
        Color color = Color.white;

        void Start()
        {
            img = GetComponent<Image>();
        }

        public void SetStartingColor(Color StartingColor)
        {
            color = StartingColor;
        }

        public void startFadeOut(int speedMulti)
        {
            count = 1;
            DoneFadeOut = false;
            CoroutineHost.StartCoroutine(FadeOut(speedMulti));
        }
        IEnumerator FadeOut(int speedMulti)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            while (count > 0)
            {
                count -= 0.025f * Time.deltaTime * speedMulti;
                Color VisableColor = new Color(color.r, color.g, color.b, count);
                img.color = VisableColor;
                yield return null;
            }
            DoneFadeOut = true;
        }


        public void startFadeIn(int speedMulti)
        {
            count = 0;
            DoneFadeIn = false;
            CoroutineHost.StartCoroutine(FadeIn(speedMulti));
        }
        IEnumerator FadeIn(int speedMulti)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            while (count < 1)
            {
                count += 0.025f * Time.deltaTime * speedMulti;
                Color VisableColor = new Color(color.r, color.g, color.b, count);
                img.color = VisableColor;
                yield return null;
            }
            DoneFadeIn = true;
        }
    }
}
