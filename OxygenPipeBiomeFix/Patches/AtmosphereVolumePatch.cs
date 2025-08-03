using HarmonyLib;
using UnityEngine;

namespace OxygenPipeBiomeFix.patches;

[HarmonyPatch(typeof(AtmosphereVolume))]
internal class AtmosphereVolumePatch
{
    [HarmonyPatch(nameof(AtmosphereVolume.Start))]
    [HarmonyPostfix]
    public static void OnStartPatch(DisplayManager __instance)
    {
        int noRayCastLayer = LayerMask.NameToLayer("Ignore Raycast");
        __instance.gameObject.layer = noRayCastLayer;
    }
}

