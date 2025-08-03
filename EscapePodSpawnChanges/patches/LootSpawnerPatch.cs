using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifePodRemastered.patches;

[HarmonyPatch(typeof(LootSpawner))]
internal class LootSpawnerPatch
{

    [HarmonyPatch(nameof(LootSpawner.GetEscapePodStorageTechTypes))]
    [HarmonyPrefix]
    public static bool ChangeStartingSupplies(ref LootSpawner __instance)
    {
        __instance.escapePodTechTypes.Clear();
        __instance.escapePodTechTypes.AddRange(LPRGlobals.selectedLoadout.LifePodStorageTechTypes);
        return true;
    }
}

