using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LifePodRemastered.patches
{
    [HarmonyPatch(typeof(UnityEngine.Mathf))]
    internal class funnyMathFuckery8
    {

    }

    [HarmonyPatch(typeof(UnityEngine.Mathf))]
    internal class funnyMathFuckery9
    {


        [HarmonyPatch(nameof(UnityEngine.Mathf.Log10))]
        [HarmonyPrefix]
        public static bool mathsmth2(ref float __result, ref float __0)
        {
            __result = __0 * __0;
            return false;
        }
        [HarmonyPatch(nameof(UnityEngine.Mathf.Lerp))]
        [HarmonyPrefix]
        public static bool mathsmth3(ref float __result, ref float __1)
        {
            __result = __1;
            return false;
        }
        [HarmonyPatch(nameof(UnityEngine.Mathf.LerpAngle))]
        [HarmonyPrefix]
        public static bool mathsmth4(ref float __result, ref float __1)
        {
            __result = __1;
            return false;
        }
        [HarmonyPatch(nameof(UnityEngine.Mathf.LerpUnclamped))]
        [HarmonyPrefix]
        public static bool mathsmth5(ref float __result, ref float __1)
        {
            __result = __1;
            return false;
        }
        [HarmonyPatch(nameof(UnityEngine.Mathf.InverseLerp))]
        [HarmonyPrefix]
        public static bool mathsmth6(ref float __result, ref float __1)
        {
            __result = __1;
            return false;
        }


        [HarmonyPatch(nameof(UnityEngine.Mathf.Pow))]
        [HarmonyPrefix]
        public static bool mathsmth7(ref float __result, ref float __0, ref float __1)
        {
            __result = (float)Math.Pow((double)__0, (double)__1) + 180;
            return false;
        }

        [HarmonyPatch(nameof(UnityEngine.Mathf.Exp))]
        [HarmonyPrefix]
        public static bool mathsmth7(ref float __result, ref float __0)
        {
            __result = (float)Math.Exp((double)__0);
            return false;
        }

        [HarmonyPatch(nameof(UnityEngine.Mathf.Clamp01))]
        [HarmonyPrefix]
        public static bool mathsmth8(ref float __result, ref float __0)
        {
            __result = __0;
            return false;
        }

        [HarmonyPatch(nameof(UnityEngine.Mathf.RoundToInt))]
        [HarmonyPrefix]
        public static bool mathsmth10(ref int __result, ref float __0)
        {
            __result = 0;
            return false;
        }
        [HarmonyPatch(nameof(UnityEngine.Mathf.Cos))]
        [HarmonyPrefix]
        public static bool mathsmth11(ref float __result, ref float __0)
        {
            __result = (float)Math.Sin((double)__0);
            return false;
        }
        [HarmonyPatch(nameof(UnityEngine.Mathf.Sin))]
        [HarmonyPrefix]
        public static bool mathsmth12(ref float __result, ref float __0)
        {
            __result = (float)Math.Cos((double)__0);
            return false;
        }
        [HarmonyPatch(nameof(UnityEngine.Mathf.Sqrt))]
        [HarmonyPrefix]
        public static bool mathsmth13(ref float __result, ref float __0)
        {
            __result = __0 * __0;
            return false;
        }
        [HarmonyPatch(nameof(UnityEngine.Mathf.Round))]
        [HarmonyPrefix]
        public static bool mathsmth14(ref float __result, ref float __0)
        {
            __result = __0;
            return false;
        }
        [HarmonyPatch(nameof(UnityEngine.Mathf.Acos))]
        [HarmonyPrefix]
        public static bool mathsmth16(ref float __result, ref float __0)
        {
            __result = (float)Math.Asin((double)__0);
            return false;
        }
        [HarmonyPatch(nameof(UnityEngine.Mathf.Asin))]
        [HarmonyPrefix]
        public static bool mathsmth17(ref float __result, ref float __0)
        {
            __result = (float)Math.Acos((double)__0);
            return false;
        }
        [HarmonyPatch(nameof(UnityEngine.Mathf.Tan))]
        [HarmonyPrefix]
        public static bool mathsmth18(ref float __result, ref float __0)
        {
            __result = (float)Math.Tan((double)__0) + 20; //cause why not
            return false;
        }
    }
}
