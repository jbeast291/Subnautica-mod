using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LifePodRemastered.objects;

internal class ModeSpecific : Mode
{
    private bool active;
    public ModeSpecific(EscapePodMainMenu escapePodMainMenu, String nameLanguageKey, String descriptionLanguageKey)
    : base(escapePodMainMenu, nameLanguageKey, descriptionLanguageKey)
    {
        escapePodMainMenu.registerPointerDownEvent(onClick);
    }
    public override void ToggleMode(bool toggle)
    {
        active = toggle;
    }

    public void onClick(PointerEventData eventData)
    {
        if (active && eventData.button == PointerEventData.InputButton.Left && 
            escapePodMainMenu.CheckValidMousePosition(Input.mousePosition)
           )
        {
            escapePodMainMenu.MoveSelecedPointFromWorldPoint(escapePodMainMenu.MousePositionToWorldPoint(Input.mousePosition));
            escapePodMainMenu.PlayButtonPressSound();
        }
    }
}

