using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace InvisiblePlayerModel;

public static class PlayerModelModifier
{
    public static void UpdatePlayerModel()
    {
        if (Player.main == null)
        {
            return;
        }
        Transform bodyHolder = Player.main.transform.Find("body").Find("player_view").Find("male_geo");
        if (bodyHolder == null)
        {
            Plugin.Logger.LogError($"Failed to find Body Geo!");
            return;
        }
        
        RecurseModel(bodyHolder);
        Plugin.Logger.LogInfo("Updated Player Visibility!");
    }

    private static void RecurseModel(Transform parent)
    {
        if (parent.gameObject.TryGetComponent<SkinnedMeshRenderer>(out var meshRenderer))
        {
            if (Plugin.config.hideModel)
            {
                HideMesh(meshRenderer);
            }
            else
            {
                ShowMesh(meshRenderer);
            }
        }
        
        foreach (Transform child in parent)
        {
            RecurseModel(child);
        }
    }

    private static void HideMesh(SkinnedMeshRenderer meshRenderer)
    {
        if (meshRenderer.gameObject.name.Contains("head"))
        {
            HandleHeadMesh(meshRenderer);
            return;
        }
        
        if (!Plugin.config.hidePlayerShadows)
        {
            meshRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            AlphaClipTransparentMethod(false, meshRenderer);
        }
        else
        {
            AlphaClipTransparentMethod(true, meshRenderer);
        }
    }

    private static void ShowMesh(SkinnedMeshRenderer meshRenderer)
    {
        if (meshRenderer.gameObject.name.Contains("head"))
        {
            HandleHeadMesh(meshRenderer);
            return;
        }

        if (Plugin.config.hidePlayerShadows)
        {
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
        }
        else
        {
            meshRenderer.shadowCastingMode = ShadowCastingMode.On;
        }
        
        AlphaClipTransparentMethod(false, meshRenderer);
    }

    private static void HandleHeadMesh(SkinnedMeshRenderer meshRenderer)
    {
        if (Plugin.config.hidePlayerShadows)
        {
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            AlphaClipTransparentMethod(true, meshRenderer);
        }
        else
        {
            meshRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            AlphaClipTransparentMethod(false, meshRenderer);
        }
    }

    private static void AlphaClipTransparentMethod(bool enable, SkinnedMeshRenderer meshRenderer)
    {
        if (enable)
        {
            foreach (Material mat in meshRenderer.materials)
            { 
                mat.EnableKeyword("MARMO_ALPHA_CLIP");
                mat.SetFloat(ShaderPropertyID._Cutoff, 10.0f);//this makes the model invisible at the shader
            }
        }
        else
        {
            foreach (Material mat in meshRenderer.materials)
            {
                mat.DisableKeyword("MARMO_ALPHA_CLIP");
            }
        }
    }
}
