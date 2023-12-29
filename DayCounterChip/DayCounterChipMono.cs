using System.Reflection;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Nautilus.Handlers;

namespace DayCounterChip
{
    public class DayCounterChipFuntion : MonoBehaviour
    {
        public static AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "daycounterchipbundle"));

        readonly DayNightCycle dayNightCycle = DayNightCycle.main;

        static GameObject DayCounter;
        static GameObject DayCounterImage1;
        static GameObject DayCounterImage2;
        static GameObject PDAImage;
        static Color PdaTextColor = new Color(0, 1, 1, 1); // light blue
        static Color WhiteTextColor = new Color(1, 1, 1, 1); // White
        static Vector3 PdaModeScale = new Vector3(500, 500, 500);
        static Vector3 HudModeScale = new Vector3(935.3073f, 935.3074f, 935.3073f);
        static PDA pDA;
        static TMPro.TextMeshProUGUI DayCounterText;

        public void Awake()
        {
            DayCounter = Instantiate(assetBundle.LoadAsset<GameObject>("DayCounteChip"));
            DayCounterImage1 = GameObject.Find("BackGround1");
            DayCounterImage2 = GameObject.Find("BackGround2");
            PDAImage = GameObject.Find("BackGround3");
            DayCounterText = GameObject.Find("DayText").GetComponent<TMPro.TextMeshProUGUI>();
            pDA = GameObject.Find("Player/body/player_view/export_skeleton/head_rig/neck/chest/clav_L/clav_L_aim/shoulder_L/elbow_L/hand_L/attachL/PlayerPDA").GetComponent<PDA>();

            DayCounter.SetActive(true);
        }

        public void Start()
        {
            if (BepInEx.myConfig.PdaMode)
                SetupPdaMode();
            else
            {
                SetupHudMode();
                UpdatePosition();
            }
            UpdateImages();

        }

        public void LateUpdate()
        {
            if (CheckIfEquipmentIsInSlot(DayCounterItem.Info.TechType) && (MainCameraControl.main.cinematicMode == false) && (pDA.state == PDA.State.Closed || BepInEx.myConfig.PdaMode))
            {
                float Currentday = (float)(dayNightCycle.GetDay() - 0.5f);
                DayCounterText.text = $"Day: {Currentday:N0}";

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
            for (int i = 0; i < Slotslist.Count; i++)
            {
                string slot = Slotslist[i];
                TechType tt = equipment.GetTechTypeInSlot(slot);
                if (tt == techtype)
                {
                    return true;
                }
            }
            return false;
        }
        public static void UpdateImages()
        {
            if (DayCounterImage1 != null && DayCounterImage2 != null)
            {
                if (BepInEx.myConfig.BackGroundChoice == "BackGround 1")
                {
                    DayCounterImage1.SetActive(true);
                    DayCounterImage2.SetActive(false);
                    PDAImage.SetActive(false);
                    UpdateTextColor();
                }
                if (BepInEx.myConfig.BackGroundChoice == "BackGround 2")
                {
                    DayCounterImage1.SetActive(false);
                    DayCounterImage2.SetActive(true);
                    PDAImage.SetActive(false);
                    UpdateTextColor();
                }
                if (BepInEx.myConfig.BackGroundChoice == "Pda Style")
                {
                    DayCounterImage1.SetActive(false);
                    DayCounterImage2.SetActive(false);
                    PDAImage.SetActive(true);
                    UpdateTextColor();
                }
                if (BepInEx.myConfig.BackGroundChoice == "No BackGround")
                {
                    DayCounterImage1.SetActive(false);
                    DayCounterImage2.SetActive(false);
                    PDAImage.SetActive(false);
                }
            }
        }
        public static void UpdatePosition()
        {
            if (!BepInEx.myConfig.PdaMode)
            {
                DayCounter.transform.localPosition = new Vector3 (BepInEx.myConfig.PosX - (150 * BepInEx.myConfig.Scale), BepInEx.myConfig.PosY, 1.4027f);
            }
        }
        
        public static void UpdateMode()
        {
            if(BepInEx.myConfig.PdaMode)
            {
                SetupPdaMode();
                UpdateTextColor();
            }
            else
            {
                SetupHudMode();
                UpdateScale();
                UpdatePosition();
                UpdateTextColor();
            }
        }

        public static void UpdateScale()
        {
            if (!BepInEx.myConfig.PdaMode)
            {
                DayCounter.transform.localScale = new Vector3(
                    HudModeScale.x,
                    HudModeScale.y * BepInEx.myConfig.Scale,
                    HudModeScale.z * BepInEx.myConfig.Scale);
            }
        }

        static void SetupPdaMode()
        {
            DayCounter.transform.SetParent(GameObject.Find("uGUI_PDAScreen(Clone)/Content/InventoryTab").transform);
            DayCounter.transform.localPosition = new Vector3(-79.3639f, 331.4845f, -2.0231f);
            DayCounter.transform.localRotation = Quaternion.Euler(0, 270, 0);
            DayCounter.transform.localScale = PdaModeScale;

        }

        static void SetupHudMode()
        {
            DayCounter.transform.SetParent(GameObject.Find("uGUI(Clone)/ScreenCanvas/HUD/Content").transform);
            DayCounter.transform.localPosition = new Vector3(960.285f, 674.0301f, 217.4586f);
            DayCounter.transform.localRotation = Quaternion.Euler(0, 270, 0);
            DayCounter.transform.localScale = new Vector3(
                HudModeScale.x, 
                HudModeScale.y * BepInEx.myConfig.Scale,
                HudModeScale.z * BepInEx.myConfig.Scale);
        }

        static void UpdateTextColor()
        {
            if (BepInEx.myConfig.PdaMode && BepInEx.myConfig.BackGroundChoice == "Pda Style")
                DayCounterText.color = PdaTextColor;
            else
                DayCounterText.color = WhiteTextColor;
        }
        
    }
}
