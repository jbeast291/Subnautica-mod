using LifePodRemastered.Monos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LifePodRemastered.presetSystem;

internal class ModeSpecific : Mode
{
    bool active;
    GameObject YLevelNoticeBackground;
    public ModeSpecific(EscapePodMainMenu escapePodMainMenu, String nameLanguageKey, String descriptionLanguageKey, GameObject YLevelNoticeBackground)
    : base(escapePodMainMenu, nameLanguageKey, descriptionLanguageKey, false)
    {
        escapePodMainMenu.registerPointerDownEvent(onClick);
        this.YLevelNoticeBackground = YLevelNoticeBackground;
    }
    public override void ToggleMode(bool toggle)
    {
        active = toggle;
        YLevelNoticeBackground.SetActive(toggle);
    }
    public override void EnableInteraction(bool toggle)
    {
        YLevelNoticeBackground.SetActive(toggle);
    }

    public void onClick()
    {
        //take controll on any mode
        //if(!active && escapePodMainMenu.CheckValidMousePosition(Input.mousePosition) && escapePodMainMenu.activeMapDisplayType == MapDisplayType.ThreeDimensional)
        //{
        //    escapePodMainMenu.ResetCurrentModeToDefault();
        //    MiniWorldController.main.ToggleControll();
        //    return;
        //}

        if (!active || !escapePodMainMenu.CheckValidMousePosition(Input.mousePosition))
        {
            return;
        }

        if (escapePodMainMenu.activeMapDisplayType == MapDisplayType.TwoDimensional)
        {
            escapePodMainMenu.MoveSelecedPointFromWorldPoint(escapePodMainMenu.MousePositionToWorldPoint(Input.mousePosition));
            escapePodMainMenu.PlayButtonPressSound();
            return;
        }

        if (escapePodMainMenu.activeMapDisplayType == MapDisplayType.ThreeDimensional)
        {
            MiniWorldController.main.ToggleControll();
            return;
        }
    }

    public override String Description()
    {
        if (escapePodMainMenu.activeMapDisplayType == MapDisplayType.TwoDimensional)
        {
            return Language.main.Get("LPR.ModeSpecific2dDescription");
        }
        if (escapePodMainMenu.activeMapDisplayType == MapDisplayType.ThreeDimensional)
        {
            return Language.main.Get("LPR.ModeSpecific3dDescription");
        }
        return "Error";
    }
}

