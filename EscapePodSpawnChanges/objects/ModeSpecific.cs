using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace LifePodRemastered.objects;

internal class ModeSpecific : Mode
{
    Action onClickAction;
    public ModeSpecific(EscapePodMainMenu escapePodMainMenu, String nameLanguageKey, String descriptionLanguageKey)
    : base(escapePodMainMenu, nameLanguageKey, descriptionLanguageKey)
    {
        onClickAction = onClick;
    }

    public void onClick()
    {
        if (escapePodMainMenu.CheckValidMousePosition(Input.mousePosition))
        {
            escapePodMainMenu.MoveSelecedPointFromWorldPoint(escapePodMainMenu.MousePositionToWorldPoint(Input.mousePosition));
            escapePodMainMenu.PlayButtonPressSound();
        }
    }
    public Action getOnClickAction()
    {
        return onClickAction;
    }
}

