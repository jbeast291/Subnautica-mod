using System.Reflection;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Collections;
using UWE;

namespace DayCounterChip
{
    public class DayCounterChipFuntion : MonoBehaviour
    {
        public static AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "daycounterchipbundle"));

        readonly DayNightCycle dayNightCycle = DayNightCycle.main;

        static GameObject DayCounter;
        static GameObject DayCounterImage1;
        static GameObject DayCounterImage2;
        TMPro.TextMeshProUGUI DayCounterText;

        public void Awake()
        {
            DayCounter = Instantiate(assetBundle.LoadAsset<GameObject>("DayCounteChip"));
            DayCounter.transform.parent = GameObject.Find("uGUI(Clone)/ScreenCanvas/HUD/Content").transform;
            DayCounterImage1 = GameObject.Find("LeftSideBackGround");
            DayCounterImage2 = GameObject.Find("LeftSideBackGround2");
            DayCounterText = GameObject.Find("DayText").GetComponent<TMPro.TextMeshProUGUI>();
            DayCounter.SetActive(true);
            DayCounter.transform.position = new Vector3(-0.2444f, - 0.9074f, 1);
        }
        public void Start()
        {
            UpdateImages();
            CoroutineHost.StartCoroutine(Check());
        }
        public void Update()
        {
            if (CheckIfEquipmentIsInSlot(DayCounterChip.TechTypeID) && MainCameraControl.main.cinematicMode == false)
            {
                float Currentday = (float)(dayNightCycle.GetDay() - 0.5f);
#pragma warning disable IDE0071
                DayCounterText.text = $"Day: {Currentday.ToString("N0")}";
#pragma warning restore IDE0071

                //DayCounterText.transform.position = new Vector3(2.0146f, 1.2974f, 1.4027f);

                DayCounter.SetActive(true);
            }
            else
            {
                DayCounter.SetActive(false);
            }
        }
        public static bool CheckIfEquipmentIsInSlot(TechType techtype) // Checks all the equipment slots and sees if the techtype is there (if you wondering i "borrowed" this from another mod)
        {
            Equipment equipment = null;
            EquipmentType equipmentType = EquipmentType.Chip;

            if (equipment == null)
            {
                equipment = Inventory.main?.equipment;
            }

            if (equipment == null)
            {
                return false;
            }

            List<string> Slotslist = new List<string>();
            equipment.GetSlots(equipmentType, Slotslist);
            Equipment equipment1 = equipment;

            for (int i = 0; i < Slotslist.Count; i++)
            {
                string slot = Slotslist[i];
                TechType tt = equipment1.GetTechTypeInSlot(slot);
                if (tt == techtype)
                {
                    return true;
                }
            }
            return false;
        }
        public static void UpdateImages()
        {
            Debug.Log("update");
            if (DayCounterImage1 != null && DayCounterImage2 != null)
            {
                if (BepInEx.myConfig.BackGroundChoice == "BackGround 1")
                {
                    DayCounterImage1.SetActive(true);
                    DayCounterImage2.SetActive(false);
                }
                if (BepInEx.myConfig.BackGroundChoice == "BackGround 2")
                {
                    DayCounterImage1.SetActive(false);
                    DayCounterImage2.SetActive(true);
                }
                if (BepInEx.myConfig.BackGroundChoice == "No BackGround")
                {
                    DayCounterImage1.SetActive(false);
                    DayCounterImage2.SetActive(false);
                }
            }
        }
        public static void UpdatePosition()
        {
            DayCounter.transform.position = new Vector3(BepInEx.myConfig.PosX, BepInEx.myConfig.PosY, 1.4027f);
        }
        public IEnumerator Check()
        {
            yield return new WaitForSecondsRealtime(2);
            UpdatePosition();
            Debug.Log("Check");
        }
    }
}
