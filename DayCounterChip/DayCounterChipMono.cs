using System.Reflection;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


namespace DayCounterChip
{
    internal class DayCounterChipFuntion : MonoBehaviour
    {
        public static AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "daycounterchipbundle"));

        DayNightCycle dayNightCycle = DayNightCycle.main;

        GameObject DayCounter;
        GameObject DayCounterImage1;
        GameObject DayCounterImage2;
        TMPro.TextMeshPro DayCounterText;

        public void Awake()
        {
            DayCounter = Instantiate(assetBundle.LoadAsset<GameObject>("DayCounteChip"));
            DayCounter.transform.parent = GameObject.Find("uGUI(Clone)/ScreenCanvas/HUD/Content").transform;
            DayCounterImage1 = GameObject.Find("LeftSideBackGround");
            DayCounterImage2 = GameObject.Find("LeftSideBackGround2");
            DayCounterText = GameObject.Find("DayText").GetComponent<TMPro.TextMeshPro>();
            DayCounter.SetActive(true);
            DayCounter.transform.position = new Vector3(-0.2444f, - 0.9074f, 1);

            DayCounterImage1.SetActive(true);
            DayCounterImage2.SetActive(true);
        }
        public void Start()
        {
            DayCounterText.isOverlay = true;
            DayCounterText.alignment = TMPro.TextAlignmentOptions.Center;
            DayCounterText.gameObject.layer = 31;
        }
        public void Update()
        {
            if (CheackIfEquipmentIsInSlot(DayCounterChip.TechTypeID))
            {
                DayCounter.transform.position = new Vector3(BepInEx.Config.PosX, BepInEx.Config.PosY, 1.4027f);
                float Currentday = (float)(dayNightCycle.GetDay() - 0.5f);
                DayCounterText.text = $"Day: {Currentday.ToString("N0")}";

                //DayCounterText.transform.position = new Vector3(2.0146f, 1.2974f, 1.4027f);

                DayCounter.SetActive(true);

                if (BepInEx.Config.BackGroundChoice == "BackGround 1")
                {
                    DayCounterImage1.SetActive(true);
                    DayCounterImage2.SetActive(false);
                }
                if (BepInEx.Config.BackGroundChoice == "BackGround 2")
                {
                    DayCounterImage1.SetActive(false);
                    DayCounterImage2.SetActive(true);
                }
                if (BepInEx.Config.BackGroundChoice == "No BackGround")
                {
                    DayCounterImage1.SetActive(false);
                    DayCounterImage2.SetActive(false);
                }
            }
            else
            {
                DayCounter.SetActive(false);
            }
        }
        public static bool CheackIfEquipmentIsInSlot(TechType techtype) // Checks all the equipment slots and sees if the teachtype is there (if you wondering i "borrowed" this from another mod)
        {
            Equipment equipment = null;
            EquipmentType equipmentType = EquipmentType.Chip;

            if (equipment == null)
            {
                equipment = Inventory.main != null ? Inventory.main.equipment : null;
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
    }
}
