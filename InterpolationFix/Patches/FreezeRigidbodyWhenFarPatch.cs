using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.TextCore;

namespace InterpolationFix.Patches;

/// <summary>
/// Transpiler patch for FreezeRigidbodyWhenFar to replace calls to UWE.Utils.SetIsKinematicAndUpdateInterpolation(...) with just setting
/// the rigidbody to kinematic. This ensures it doesn't touch interpolation (it doesn't even need to, to function).
/// </summary>
[HarmonyPatch(typeof(FreezeRigidbodyWhenFar), nameof(FreezeRigidbodyWhenFar.FixedUpdate))]
internal class FreezeRigidbodyWhenFarPatch
{ 
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher matcher = new CodeMatcher(instructions);

        
        ReplaceSetIsKinematicAndUpdateInterpolationCalls(matcher);
        ReplaceSetIsKinematicAndUpdateInterpolationCalls(matcher);
        
        return matcher.InstructionEnumeration();
    }

    public static void ReplaceSetIsKinematicAndUpdateInterpolationCalls(CodeMatcher matcher)
    {
        matcher
            .MatchForward(true, new CodeMatch(ci => ci.opcode == OpCodes.Ldloc_0))//find where it loads rigidbody
            .Advance(2)//hop past ldc.i4.1 and onto ldc.i4.0
            .RemoveInstruction()//remove ldc.i4.0;
            .SetAndAdvance(OpCodes.Call, AccessTools.PropertySetter(typeof(Rigidbody), nameof(Rigidbody.isKinematic)));//replace old call with just a set
    }
}
