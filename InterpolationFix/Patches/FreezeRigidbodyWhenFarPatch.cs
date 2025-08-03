using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace InterpolationFix.Patches
{
    /// <summary>
    /// Transpiler patch for FreezeRigidbodyWhenFar to replace calls to UWE.Utils.SetIsKinematicAndUpdateInterpolation(...) with just setting
    /// the rigidbody to kinematic. This ensures it doesnt touch interpolation (it doesn't even need to, to function).
    /// </summary>
    [HarmonyPatch(typeof(FreezeRigidbodyWhenFar))]
    [HarmonyPatch("FixedUpdate")]
    internal class FreezeRigidbodyWhenFarPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            var targetMethod = AccessTools.Method(
                typeof(UWE.Utils),//there is abuguity so must be very specific :/
                "SetIsKinematicAndUpdateInterpolation",
                new Type[] { typeof(Rigidbody), typeof(bool), typeof(bool) }
            );

            var isKinematicSetter = AccessTools.PropertySetter(typeof(Rigidbody), "isKinematic");

            for (int i = 1; i < codes.Count; i++)
            {
                if (//find UWE.Utils.SetIsKinematicAndUpdateInterpolation(...) call
                    codes[i].Calls(targetMethod)
                )
                {
                    //rigidbody ref and true opcode already there, so use them
                    codes[i - 1] = new CodeInstruction(OpCodes.Callvirt, isKinematicSetter);//set rigidbody to kinematic
                    codes.RemoveAt(i);//dont need the fourth instruction
                }
            }
            BepInEx.Logger.LogInfo("Finished Transpiler for FreezeRigidbodyWhenFar.FixedUpdate...");
            return codes;
        } 
    }
}
