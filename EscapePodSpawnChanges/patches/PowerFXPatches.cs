using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifePodRemastered.patches;

[HarmonyPatch(typeof(PowerFX))]
internal class PowerFXPatches
{
    /*[HarmonyPatch(nameof(PowerFX.Update))]
    [HarmonyPostfix]
    public static void ChangeStartingSupplies(ref LootSpawner __instance)
    {
        __instance.escapePodTechTypes.Clear();
        __instance.escapePodTechTypes.AddRange(LPRGlobals.selectedLoadout.LifePodStorageTechTypes);
    }*/
}

