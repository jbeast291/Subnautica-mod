using HarmonyLib;

namespace DayCounterChip
{
    [HarmonyPatch(typeof(Player))]
    internal class PlayerPatch
    {
        [HarmonyPatch(nameof(Player.Start))]
        [HarmonyPostfix]
        static void PlayerPatchStartPostfix(Player __instance)
        {
            __instance.gameObject.EnsureComponent<DayCounterChipFuntion>();// ensures that the component is attached to the player
        }
    }
}
