using BepInEx.Bootstrap;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace LifePodRemastered.Compatability;

internal class PrototypeExpansionPatches
{
    public static void loadCompatabilityPatches()
    {
        var harmony = BepInExEntry.harmony;
        BepInExEntry.Logger.LogInfo("PrototypeSub mod loaded, applying compatability patches...");

        var otherAssembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name.Equals("PrototypeSubMod", StringComparison.OrdinalIgnoreCase));

        if (otherAssembly == null)
        {
            BepInExEntry.Logger.LogError("Couldn't find PrototypeSubMod assembly.");
            return;
        }

        var patchType = otherAssembly.GetType("PrototypeSubMod.Patches.GameInput_Patches");
        if (patchType == null)
        {
            BepInExEntry.Logger.LogError("Couldn't find PrototypeSubMod.Patches.GameInput_Patches type.");
            return;
        }

        var postfixMethod = patchType.GetMethod("GetLookDelta_Postfix", BindingFlags.NonPublic | BindingFlags.Static);
        if (postfixMethod == null)
        {
            BepInExEntry.Logger.LogError("Couldn't find GetLookDelta_Postfix method.");
            return;
        }

        var prefix = typeof(PrototypeExpansionPatches).GetMethod(nameof(GetLookDelta_Postfix_Prefix), BindingFlags.NonPublic | BindingFlags.Static);
        if (prefix == null)
        {
            BepInExEntry.Logger.LogError("Couldn't find prefix method.");
            return;
        }

        harmony.Patch(postfixMethod, prefix: new HarmonyMethod(prefix));

        BepInExEntry.Logger.LogInfo("PrototypeSub mod loaded, applying compatability patches...");
    }
    private static bool GetLookDelta_Postfix_Prefix()
    {
        return Player.main != null;
    }
}

