using HarmonyLib;
using Logger = QModManager.Utility.Logger;
using RecipeData = SMLHelper.V2.Crafting.TechData;
using QModManager.API.ModLoading;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using SMLHelper.V2.Utility;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace DayCounterChip
{
    public class DayCounterChip : Equipable
    {
        public static TechType TechTypeID { get; protected set; }
        public override string AssetsFolder => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public DayCounterChip() : base("DayCounterChip", "Day Counter Chip", "Toggles a day counter when the key is pressed")
        {
            OnFinishedPatching += () =>
            {
                TechTypeID = this.TechType;
            };
        }
        public override EquipmentType EquipmentType => EquipmentType.Chip;
        public override TechType RequiredForUnlock => TechType.Compass;
        public override TechGroup GroupForPDA => TechGroup.Personal;
        public override TechCategory CategoryForPDA => TechCategory.Equipment;
        public override CraftTree.Type FabricatorType => CraftTree.Type.Fabricator;
        public override string[] StepsToFabricatorTab => new string[] { "Personal", "Equipment" };
        public override float CraftingTime => 1f;
        public override QuickSlotType QuickSlotType => QuickSlotType.Passive;

        protected override Atlas.Sprite GetItemSprite()
        {
            return ImageUtils.LoadSpriteFromFile(Path.Combine(AssetsFolder, "DayCounterChipIcon.png"));
        }

        protected override RecipeData GetBlueprintRecipe()
        {
            return new RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<Ingredient>(new Ingredient[]
                    {
                        new Ingredient(TechType.Titanium, 2),
                        new Ingredient(TechType.CopperWire, 1)
                    }
                )
            };
        }

        public override GameObject GetGameObject()
        {
            var prefab = CraftData.GetPrefabForTechType(TechType.MapRoomHUDChip);
            var obj = GameObject.Instantiate(prefab);
            return obj;
        }
    }
    public class Info
    {
        public static Resolution currentRes;
        public static Canvas canvas;
    }
    internal class DayCounterChipFuntion : MonoBehaviour
    {
        public Text text;
        public static AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "daycounterchipbundle"));

        public Image image;

        DayNightCycle dayNightCycle = DayNightCycle.main;

        RectTransform rectTransform;
        RectTransform RectTransform2;
        public void Awake()
        {
            QMod.Config.Load();

            Canvas canvas;
            canvas = Info.canvas.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            //create image for background

            GameObject imageGO = new GameObject();
            imageGO.transform.parent = canvas.transform;
            imageGO.AddComponent<Image>();

            image = imageGO.GetComponent<Image>();
            image.gameObject.SetActive(false);

            RectTransform2 = image.GetComponent<RectTransform>();
 

            //Create text for background

            GameObject textgo = new GameObject();
            textgo.transform.parent = canvas.transform;
            textgo.AddComponent<Text>();

            text = textgo.GetComponent<Text>();
            text.font = Player.main.textStyle.font;
            text.alignment = TextAnchor.MiddleCenter;
            text.GetComponent<Transform>();

            rectTransform = text.GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(0, 0, 0);
            rectTransform.sizeDelta = new Vector2(6000, 2000);
        }
        public void Update()
        {

            if (CheackIfEquipmentIsInSlot(DayCounterChip.TechTypeID))
            {
                float diff = Info.currentRes.width / 2560f; // used to make the scale work at different resolutions

                text.color = GetColorFromConfig(); 
                text.transform.position = new Vector3(QMod.Config.PosX * diff, QMod.Config.PosY * diff, 0); // set the position of the text based on the config and the resolution
                text.text = $"Day: {dayNightCycle.GetDay().ToString("N0")}"; // set the text to have the day count from daynightcycle
                text.fontSize = Mathf.RoundToInt(48f * diff); // Set the size of the text based on the resolution 

                if (QMod.Config.BackGroundChoice == "BackGround 1") 
                {
                    image.sprite = assetBundle.LoadAsset<Sprite>("BackGround"); // Load background 1 from the asset file
                    image.rectTransform.position = new Vector3((QMod.Config.PosX - 1.45f) * diff, (QMod.Config.PosY - 4f) * diff, 0); // set the position of the image based on the config and the resolution
                    RectTransform2.sizeDelta = new Vector2(313 * diff, 97 * diff); // Set the size of the text based on the resolution 
                    image.gameObject.SetActive(true);
                }

                //

                if (QMod.Config.BackGroundChoice == "BackGround 2")
                {
                    image.sprite = assetBundle.LoadAsset<Sprite>("BackGround2"); // Load background 2 from the asset file
                    image.rectTransform.position = new Vector3((QMod.Config.PosX + 0.9501f) * diff, (QMod.Config.PosY - 41.75f) * diff, 0); // set the position of the image based on the config and the resolution
                    RectTransform2.sizeDelta = new Vector2(313 * diff, 97 * diff); // Set the size of the text based on the resolution 
                    image.gameObject.SetActive(true);
                }

                //

                if (QMod.Config.BackGroundChoice == "No BackGround")
                {
                    image.gameObject.SetActive(false);
                }
            }

            if (!CheackIfEquipmentIsInSlot(DayCounterChip.TechTypeID) )
            {
                text.text = null;
                image.gameObject.SetActive(false);
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
                Logger.Log(Logger.Level.Info, "Error no color value in config", null, true);
                return Color.white;
            }
        }
        public static bool CheackIfEquipmentIsInSlot(TechType techtype) // Checks all the equipment slots and sees if the teachtype is there
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


    [HarmonyPatch(typeof(Player))]
    internal class PlayerPatch
    {
        [HarmonyPatch(nameof(Player.Start))]
        [HarmonyPostfix]
        public static void PlayerPatchStartPostfix(Player __instance)
        {
            __instance.gameObject.EnsureComponent<DayCounterChipFuntion>(); // ensures that the component is attached to the player
        }
    }

    [HarmonyPatch(typeof(DisplayManager))]
    internal class OnResolutionChangedPatch
    {
        [HarmonyPatch(nameof(DisplayManager.Update))]
        [HarmonyPostfix]
        public static void OnResolutionChangedPostFix(DisplayManager __instance)
        {
            Info.currentRes = __instance.resolution; // sets the resolution in Info to what is currently set
        }
    }
    [HarmonyPatch(typeof(uGUI))]
    internal class uGUIPatch
    {
        [HarmonyPatch(nameof(uGUI.Awake))]
        [HarmonyPostfix]
        public static void OnResolutionChangedPostFix(uGUI __instance)
        {
            Info.canvas = __instance.screenCanvas; // sets the Canvsas in Info to what is currently set
        }
    }

    [QModCore]
    public static class QMod
    {
        internal static MyConfig Config { get; private set; }

        [QModPatch]

        public static void Patch()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var modName = ($"Jbeast291_{assembly.GetName().Name}");
            Logger.Log(Logger.Level.Info, $"Patching {modName}", null, true);
            Harmony harmony = new Harmony(modName);
            harmony.PatchAll(assembly);

            DayCounterChip dayCounterChip = new DayCounterChip();
            dayCounterChip.Patch();

            Config = OptionsPanelHandler.Main.RegisterModOptions<MyConfig>();
            Logger.Log(Logger.Level.Info, "Patched successfully!", null, true);

        }
    }
    [Menu("Day Counter Chip")]
    public class MyConfig : ConfigFile
    {
        [Slider("Counter Position on screen X", 0, 2560, DefaultValue = 2387.7f)]
        public float PosX = 2387.7f;

        [Slider("Counter Position on screen Y", 0, 1440, DefaultValue = 1375.65f)]
        public float PosY = 1375.65f;

        [Choice("Text Color", new[] { "Blue", "Red", "White", "Green", "Black", "Cyan", "Gray", "Magenta", "Yellow" }, Tooltip = "changes text color")]
        public string ColorChoice = "White";

        [Choice("BackGround Style", new[] { "BackGround 1", "BackGround 2", "No BackGround" }, Tooltip = "Changes the style of the image behind the text")]
        public string BackGroundChoice = "BackGround 2";
    }
}
