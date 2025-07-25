﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LifePodRemastered.objects;

internal class ModeInputText : Mode
{
    GameObject inputTextRoot;

    TextMeshProUGUI placeholder;
    TextMeshProUGUI example;
    public ModeInputText(EscapePodMainMenu escapePodMainMenu, String nameLanguageKey, String descriptionLanguageKey, GameObject inputTextRoot)
    : base(escapePodMainMenu, nameLanguageKey, descriptionLanguageKey)
    {
        this.inputTextRoot = inputTextRoot;

        inputTextRoot.GetComponent<TMP_InputField>().onEndEdit.AddListener(OnEndInputFieldEdit);

        placeholder = inputTextRoot.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();//NOTE: the second child is needed as a caret is loaded on init, even though in assetbundle heirarchy otherwise
        example = inputTextRoot.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        ReloadLanguage();
    }
    public override void ToggleMode(bool toggle)
    {
        inputTextRoot.SetActive(toggle);
    }
    public override void ReloadLanguage()
    {
        placeholder.text = Language.main.Get("LPR.ModeInputTextPlaceholder");
        example.text = Language.main.Get("LPR.ModeInputExample");
    }
    void OnEndInputFieldEdit(string s)
    {
        escapePodMainMenu.MoveSelecedPointFromWorldPoint(Util.StringToVector3(s));
        escapePodMainMenu.PlayButtonPressSound();
    }
}

