using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace LifePodRemastered.Monos
{
    internal class OptionsMono : MonoBehaviour
    {
        Toggle heavyPodToggle;
        Toggle firstTimeAnimToggle;
        Toggle experimentalSettingToggle;
        Toggle customIntroToggle;
        Toggle heightReqToggle;

        GameObject experimentalSettingsGroup;

        public void Awake()
        {
            heavyPodToggle = GameObject.Find("HeavyPodToggle").GetComponent<Toggle>();
            firstTimeAnimToggle = GameObject.Find("FirstTimeAnimationsToggle").GetComponent<Toggle>();
            experimentalSettingToggle = GameObject.Find("ExperimentalToggle").GetComponent<Toggle>();
            customIntroToggle = GameObject.Find("CustomIntroToggle").GetComponent<Toggle>();
            heightReqToggle = GameObject.Find("HeightReqToggle").GetComponent<Toggle>();

            experimentalSettingsGroup = GameObject.Find("ExperimentalSettings"); 
        }
        public void Start()
        {
            heavyPodToggle.onValueChanged.AddListener(delegate{
                HeavyPodToggle(heavyPodToggle);
            });
            firstTimeAnimToggle.onValueChanged.AddListener(delegate {
                FirstTimeAnimToggle(firstTimeAnimToggle);
            });
            experimentalSettingToggle.onValueChanged.AddListener(delegate {
                ExperimentalSettingToggle(experimentalSettingToggle);
            });
            customIntroToggle.onValueChanged.AddListener(delegate {
                CustomIntroToggle(customIntroToggle);
            });
            heightReqToggle.onValueChanged.AddListener(delegate {
                HeightReqToggle(heightReqToggle);
            });
        }
        public void Update()
        {
            
        }





        public void HeavyPodToggle(Toggle change)
        {

        }
        public void FirstTimeAnimToggle(Toggle change)
        {

        }
        public void ExperimentalSettingToggle(Toggle change)
        {

        }
        public void CustomIntroToggle(Toggle change)
        {

        }
        public void HeightReqToggle(Toggle change)
        {

        }
    }
}
