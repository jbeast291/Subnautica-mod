﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LifePodRemastered.objects;

internal class ModePreset : Mode
{
    GameObject presetRoot;

    TextMeshProUGUI presetText;

    TextMeshProUGUI presetSubText;
    TextMeshProUGUI presetSubSubText; // lol

    List<string[]> presetList;
    int CurrentPreset = 1;

    public ModePreset(EscapePodMainMenu escapePodMainMenu, String nameLanguageKey, String descriptionLanguageKey, GameObject presetRoot)
        : base(escapePodMainMenu, nameLanguageKey, descriptionLanguageKey)
    {
        this.presetRoot = presetRoot;

        presetList = Util.ReadPresetsFromModFolder();

        presetRoot.FindChild("ModePresetPointChoiceleft").GetComponent<Button>().onClick.AddListener(MovePresetLeft);
        presetRoot.FindChild("ModePresetPointChoiceRight").GetComponent<Button>().onClick.AddListener(MovePresetRight);

        presetText = presetRoot.FindChild("PresetText").GetComponent<TextMeshProUGUI>();

        presetSubText = presetRoot.GetComponent<TextMeshProUGUI>();
        presetSubSubText = presetRoot.transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        string[] selectedPreset = presetList[CurrentPreset - 1];
        presetText.text = selectedPreset[0];

        ReloadLanguage();
    }
    public override void ToggleMode(bool toggle)
    {
        presetRoot.SetActive(toggle);
        UpdateSelectedPresetInUI();
    }
    public override void ReloadLanguage()
    {
        presetSubText.text = Language.main.Get("LPR.ModePresetText");
        presetSubSubText.text = Language.main.Get("LPR.ModePresetSubText");
    }
    public void UpdateSelectedPresetInUI()
    {
        string[] selectedPreset = presetList[CurrentPreset - 1];
        presetText.text = selectedPreset[0];
        escapePodMainMenu.MoveSelecedPointFromWorldPoint(Util.StringToVector3(selectedPreset[1]));
    }

    public void MovePresetLeft()
    {
        if (CurrentPreset != 1)
        {
            CurrentPreset--;
            escapePodMainMenu.PlayButtonPressSound();
        }
        UpdateSelectedPresetInUI();
    }
    public void MovePresetRight()
    {
        if (CurrentPreset != presetList.Count)
        {
            CurrentPreset++;
            escapePodMainMenu.PlayButtonPressSound();
        }
        UpdateSelectedPresetInUI();
    }
}

