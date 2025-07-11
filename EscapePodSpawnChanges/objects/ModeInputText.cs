using System;
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
    public ModeInputText(EscapePodMainMenu escapePodMainMenu, String nameLanguageKey, String descriptionLanguageKey, GameObject inputTextRoot)
    : base(escapePodMainMenu, nameLanguageKey, descriptionLanguageKey)
    {
        this.inputTextRoot = inputTextRoot;

        inputTextRoot.GetComponent<TMP_InputField>().onEndEdit.AddListener(OnEndInputFieldEdit);
    }
    public override void ToggleMode(bool toggle)
    {
        inputTextRoot.SetActive(toggle);
    }
    void OnEndInputFieldEdit(string s)
    {
        //Info.SelectedSpawn = StringToVector3(s);
        //playButton.interactable = true;//make the start button pressable, once point is selected
        //Info.OverideSpawnHeight = true;
        //Info.OverideSpawnHeight = true;// <--- make sure this is disabled
        escapePodMainMenu.MoveSelecedPointFromWorldPoint(Util.StringToVector3(s));
        escapePodMainMenu.PlayButtonPressSound();
    }
}

