using HarmonyLib;
using UnityEngine;

namespace InterpolationFix.Patches
{
    [HarmonyPatch(typeof(Player))]
    internal class PlayerPatch
    {
        static bool registeredForHighFixedTimestep;

        /// <summary>
        /// Patch based off of Player.ApplyHighPrecisionPhysicsIfNecessary() but specifically tailored to the life pod.
        /// Works by checking if the player is standing on one of the colliders of the lifepod and turning off its interpolation/double fixed update if so.
        /// </summary>
        [HarmonyPatch(nameof(Player.ApplyHighPrecisionPhysicsIfNecessary))]
        [HarmonyPostfix]
        public static void OnApplyHighPrecisionPhysicsIfNecessary(Player __instance)
        {
            MainGameController mainGameController = MainGameController.Instance;
            if (mainGameController == null || EscapePod.main == null || EscapePod.main.gameObject == null)
            {
                return;
            }
            Transform playerPlatform = __instance.gameObject.GetComponent<GroundMotor>().movingPlatform.hitPlatform;
            if (playerPlatform == null)//is null if player isnt standing on anything
            {
                return;
            }

            GameObject escapePod = EscapePod.main.gameObject;
            bool requiresHPPhysics = lifePodRequiresHighPrecisionPhysics(playerPlatform.root.gameObject);

            if (requiresHPPhysics && !registeredForHighFixedTimestep)
            {
                registeredForHighFixedTimestep = true;
                mainGameController.RegisterHighFixedTimestepBehavior(__instance);
                if (escapePod != null && escapePod.GetComponent<Rigidbody>() != null)
                {
                    escapePod.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.None;
                    escapePod.GetComponent<WorldForces>().lockInterpolation = true;
                    return;
                }
            }
            else if (!requiresHPPhysics && registeredForHighFixedTimestep)
            {
                registeredForHighFixedTimestep = false;
                mainGameController.DeregisterHighFixedTimestepBehavior(__instance);
                if (escapePod != null && escapePod.GetComponent<Rigidbody>() != null)
                {
                    escapePod.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
                    escapePod.GetComponent<WorldForces>().lockInterpolation = false;
                }
            }
        }

        public static bool lifePodRequiresHighPrecisionPhysics(GameObject platformRoot)
        {
            if (platformRoot == null || EscapePod.main == null || EscapePod.main.gameObject == null)
            {
                return false;
            }

            if (GameObject.ReferenceEquals(EscapePod.main.gameObject, platformRoot) && 
                (EscapePod.main.GetComponent<Rigidbody>().velocity.sqrMagnitude > 0.001f ||
                EscapePod.main.GetComponent<Rigidbody>().angularVelocity.sqrMagnitude > 0.001f)
            )
            {
                return true;
            }
            return false;
        }
    }
}
