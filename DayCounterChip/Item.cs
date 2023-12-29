using System.Reflection;
using System.IO;
using Nautilus.Assets;
using Nautilus.Crafting;
using Nautilus.Utility;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Assets.Gadgets;
using static CraftData;

namespace DayCounterChip
{
    public class DayCounterItem
    {
        internal static string AssetsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets");
        public static PrefabInfo Info { get; private set; } = PrefabInfo
            .WithTechType("DayCounterChip", "Day Counter Chip", "Displays a Number that counts the days you have been on 4546B\n\nTo configure please modify Options->Mods")
            .WithIcon(ImageUtils.LoadSpriteFromFile(AssetsFolder + "/DayCounterChipIcon.png"));

        public static void Register()
        {
            var customPrefab = new CustomPrefab(Info);
            var DayCounterChipObj = new CloneTemplate(Info, TechType.MapRoomHUDChip);
            customPrefab.SetGameObject(DayCounterChipObj);
            customPrefab.SetRecipe(new RecipeData(){
                craftAmount = 1,
                Ingredients = {
                     new Ingredient(TechType.Titanium, 2),
                     new Ingredient(TechType.CopperWire, 1)}
            }
            )
                .WithFabricatorType(CraftTree.Type.Fabricator)
                .WithStepsToFabricatorTab("Personal", "Equipment");
            customPrefab.SetEquipment(EquipmentType.Chip);
            customPrefab.SetPdaGroupCategory(TechGroup.Personal, TechCategory.Equipment);
            customPrefab.SetUnlock(TechType.Titanium);//unlock instantly
            customPrefab.Register();
        }
    }
}