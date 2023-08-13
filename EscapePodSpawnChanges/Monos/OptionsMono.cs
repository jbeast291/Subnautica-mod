using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static LifePodRemastered.SaveManager;

namespace LifePodRemastered
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

            ReadSettingsFromModFolder();
            heavyPodToggle.isOn = settingsCache.HeavyPodToggle;
            firstTimeAnimToggle.isOn = settingsCache.FirstTimeToggle;
            experimentalSettingToggle.isOn = settingsCache.ExSettingToggle;
            customIntroToggle.isOn = settingsCache.CustomIntroToggle;
            heightReqToggle.isOn = settingsCache.HeightReqToggle;
            if (!settingsCache.ExSettingToggle)
            {
                experimentalSettingsGroup.SetActive(false);
            }
        }

        public void HeavyPodToggle(Toggle change)
        {
            if (change.isOn)
            {
                settingsCache.HeavyPodToggle = true;
            }
            else
            {
                settingsCache.HeavyPodToggle = false;
            }
            WriteSettingsToModFolder();
        }
        public void FirstTimeAnimToggle(Toggle change)
        {
            if (change.isOn)
            {
                settingsCache.FirstTimeToggle = true;
            }
            else
            {
                settingsCache.FirstTimeToggle = false;
            }
            WriteSettingsToModFolder();
        }
        public void ExperimentalSettingToggle(Toggle change)
        {
            if(change.isOn)
            {
                experimentalSettingsGroup.SetActive(true);
                settingsCache.ExSettingToggle = true;
            }
            else
            {
                experimentalSettingsGroup.SetActive(false);
                settingsCache.ExSettingToggle = false;
                settingsCache.CustomIntroToggle = true;
                settingsCache.HeightReqToggle = true;
                customIntroToggle.isOn = true;
                heightReqToggle.isOn = true;
            }
            WriteSettingsToModFolder();
        }
        public void CustomIntroToggle(Toggle change)
        {
            if (change.isOn)
            {
                settingsCache.CustomIntroToggle = true;
            }
            else
            {
                settingsCache.CustomIntroToggle = false;
            }
            WriteSettingsToModFolder();
        }
        public void HeightReqToggle(Toggle change)
        {
            if (change.isOn)
            {
                settingsCache.HeightReqToggle = true;
            }
            else
            {
                settingsCache.HeightReqToggle = false;
            }
            WriteSettingsToModFolder();
        }
    }
}
