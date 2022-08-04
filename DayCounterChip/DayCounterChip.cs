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
    internal class DayCounterChipFuntion : MonoBehaviour
    {
        public Text text;
        public static AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets"), "daycounterchipbundle"));

        public Image image;

        DayNightCycle dayNightCycle = DayNightCycle.main;

        Font arial;
        RectTransform rectTransform;
        public void Awake()
        {
            QMod.Config.Load();

            GameObject canvasGO = new GameObject();
            canvasGO.name = "Canvas";
            canvasGO.AddComponent<Canvas>();
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();


            Canvas canvas;
            canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            //create text

            GameObject imageGO = new GameObject();
            imageGO.transform.parent = canvasGO.transform;
            imageGO.AddComponent<Image>();

            image = imageGO.GetComponent<Image>();
            image.transform.position = new Vector3(100, 100, 0);
            image.gameObject.SetActive(false);

            RectTransform RectTransform2 = image.GetComponent<RectTransform>();
            RectTransform2.localPosition = new Vector3(0, 0, 0);
            RectTransform2.sizeDelta = new Vector2(313, 97);

            //create image for background

            GameObject textgo = new GameObject();
            textgo.transform.parent = canvasGO.transform;
            textgo.AddComponent<Text>();

            text = textgo.GetComponent<Text>();
            text.font = Player.main.textStyle.font;
            text.fontSize = 48;
            text.alignment = TextAnchor.MiddleCenter;
            text.GetComponent<Transform>();
            text.color = GetColorFromConfig();

            rectTransform = text.GetComponent<RectTransform>();
            rectTransform.localPosition = new Vector3(0, 0, 0);
            rectTransform.sizeDelta = new Vector2(600, 200);
        }
        public void Update()
        {
            if (CheackIfEquipmentIsInSlot(DayCounterChip.TechTypeID))
            {
                text.color = GetColorFromConfig();
                text.transform.position = new Vector3(QMod.Config.PosX, QMod.Config.PosY, 0);
                text.text = $"Day: {dayNightCycle.GetDay().ToString("N0")}";
                //
                if(QMod.Config.BackGroundChoice == "BackGround 1")
                {
                    image.sprite = assetBundle.LoadAsset<Sprite>("BackGround");
                    image.rectTransform.position = new Vector3(QMod.Config.BackGround1PosX, QMod.Config.BackGround1PosY, 0);
                    image.gameObject.SetActive(true);
                }
                if(QMod.Config.BackGroundChoice == "BackGround 2")
                {
                    image.sprite = assetBundle.LoadAsset<Sprite>("BackGround2");
                    image.rectTransform.position = new Vector3(QMod.Config.BackGround2PosX, QMod.Config.BackGround2PosY, 0);
                    image.gameObject.SetActive(true);
                }
                if(QMod.Config.BackGroundChoice == "No BackGround")
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
        public Color GetColorFromConfig()
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
            else
            {
                Logger.Log(Logger.Level.Info, "Error no color value in config", null, true);
                return Color.white;
            }
        }
        public static bool CheackIfEquipmentIsInSlot(TechType techtype)
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
            __instance.gameObject.EnsureComponent<DayCounterChipFuntion>();
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
        [Slider("Text PosX", 0, 2500, DefaultValue = 2387.7f)]
        public float PosX = 2387.7f;

        [Slider("Text PosY", 0, 1500, DefaultValue = 1375.65f)]
        public float PosY = 1375.65f;

        [Slider("BackGround Style 1 PosX", 0, 2500, DefaultValue = 2386.25f)]
        public float BackGround1PosX = 2386.25f;

        [Slider("BackGround Style 1 PosY", 0, 1500, DefaultValue = 1371.65f)]
        public float BackGround1PosY = 1371.65f;

        [Slider("BackGround Style 2 PosX", 0, 2500, DefaultValue = 2388.6501f)]
        public float BackGround2PosX = 2388.6501f;

        [Slider("BackGround Style 2 PosY", 0, 1500, DefaultValue = 1333.9f)]
        public float BackGround2PosY = 1333.9f;

        [Choice("Text Color", new[] { "Blue", "Red", "White", "Green", "Black", "Cyan", "Gray", "Magenta", "Yellow" }, Tooltip = "changes text color")]
        public string ColorChoice = "White";

        [Choice("BackGround Style", new[] { "BackGround 1", "BackGround 2", "No BackGround" }, Tooltip = "Changes the style of the image behind the text")]
        public string BackGroundChoice = "BackGround 2";
    }
}
