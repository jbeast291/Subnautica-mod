using Logger = QModManager.Utility.Logger;
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
        GameObject DayCounterText;

        public void Awake()
        {
            DayCounter = Instantiate(assetBundle.LoadAsset<GameObject>("DayCounteChip"));
            DayCounter.transform.parent = GameObject.Find("uGUI(Clone)/ScreenCanvas").transform;
            DayCounterImage1 = GameObject.Find("LeftSideBackGround");
            DayCounterImage2 = GameObject.Find("LeftSideBackGround2");
            DayCounterText = GameObject.Find("DayText");
            DayCounter.SetActive(true);
            DayCounter.transform.position = new Vector3(-0.2444f, -0.9074f, 1);

            DayCounterImage1.SetActive(false);
            DayCounterImage2.SetActive(false);
        }
        public void Update()
        {
            if (CheckIfEquipmentIsInSlot(DayCounterChip.TechTypeID))
            {
                DayCounter.transform.position = new Vector3(QMod.Config.PosX, QMod.Config.PosY, 1.4027f);
                DayCounterText.GetComponent<Text>().text = $"Day: {dayNightCycle.GetDay().ToString("N0")}";
                DayCounterText.GetComponent<Text>().color = GetColorFromConfig();
                DayCounter.SetActive(true);

                if (QMod.Config.BackGroundChoice == "BackGround 1")
                {
                    DayCounterImage1.SetActive(true);
                    DayCounterImage2.SetActive(false);
                }
                if (QMod.Config.BackGroundChoice == "BackGround 2")
                {
                    DayCounterImage1.SetActive(false);
                    DayCounterImage2.SetActive(true);
                }
                if (QMod.Config.BackGroundChoice == "No BackGround")
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
        public Color GetColorFromConfig() // gets the color form the config file and returns the color 
        {
            QMod.Config.Load();
            if (QMod.Config.ColorChoice == "Blue")
            {
                return Color.blue;
            }
            if (QMod.Config.ColorChoice == "Red")
            {
                return Color.red;
            }
            if (QMod.Config.ColorChoice == "White")
            {
                return Color.white;
            }
            if (QMod.Config.ColorChoice == "Green")
            {
                return Color.green;
            }
            if (QMod.Config.ColorChoice == "Black")
            {
                return Color.black;
            }
            if (QMod.Config.ColorChoice == "Cyan")
            {
                return Color.cyan;
            }
            if (QMod.Config.ColorChoice == "Gray")
            {
                return Color.gray;
            }
            if (QMod.Config.ColorChoice == "Magenta")
            {
                return Color.magenta;
            }
            if (QMod.Config.ColorChoice == "Yellow")
            {
                return Color.yellow;
            }
            else // if there is an error in setting the color in teh config and it does not equal anything above 
            {
                Logger.Log(Logger.Level.Error, "Error no color value in config", null, true);
                return Color.white;
            }
        }
        public static bool CheckIfEquipmentIsInSlot(TechType techtype) // Checks all the equipment slots and sees if the teachtype is there (if you wondering i "borrowed" this from another mod)
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
