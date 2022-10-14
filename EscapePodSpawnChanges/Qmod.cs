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

namespace EscapePodSpawnChanges
{
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

            Config = OptionsPanelHandler.Main.RegisterModOptions<MyConfig>();
            Logger.Log(Logger.Level.Info, "Patched successfully!", null, true);

        }
    }
    [Menu("Escape Pod Changes")]
    public class MyConfig : ConfigFile
    {
        [Toggle("Heavy Pod")]
        public bool ToggleHeavyPod = true;
    }
}
