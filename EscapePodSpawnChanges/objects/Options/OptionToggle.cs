using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LifePodRemastered.presetSystem;

internal class OptionToggle : BaseOption
{
    private Toggle toggle;
    private RectTransform rt;

    private TextMeshProUGUI title;
    private TextMeshProUGUI description;
    private GameObject padlockIcom;

    private Action<bool, bool> onChangeAction;

    public OptionToggle(string optionID, String titleLanguageKey, String descriptionLanguageKey, Toggle toggle, bool initialValue, Action<bool, bool> onChangeAction):
        base(optionID, titleLanguageKey, descriptionLanguageKey)
    {
        this.onChangeAction = onChangeAction;
        this.toggle = toggle;

        rt = toggle.gameObject.GetComponent<RectTransform>();
        title = toggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        description = toggle.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        padlockIcom = toggle.transform.GetChild(3).gameObject;
        padlockIcom.SetActive(false);

        ReloadLanguage();

        toggle.onValueChanged.AddListener(delegate {
            OnValueChanged(toggle);
        });

        toggle.SetIsOnWithoutNotify(initialValue);
    }
    
    public override void ReloadLanguage()
    {
        title.text = Language.main.Get(base.GetTitleLanguageKey());
        title.ForceMeshUpdate();
        description.text = Language.main.Get(base.GetDescriptionLanguageKey());
        description.ForceMeshUpdate();//dont know if these are needed yet !!!

        float preferredHeight = description.preferredHeight + title.preferredHeight;

        rt.sizeDelta = new Vector2(rt.sizeDelta.x, preferredHeight);
    }

    public override float GetHeight()
    {
        return rt.rect.height;
    }
    public override void SetToggledState(bool interactable, object value)
    {
        if (value is bool boolValue)
        {
            toggle.interactable = interactable;
            padlockIcom.SetActive(!interactable);
            toggle.isOn = boolValue;
            return;
        }
        BepInExEntry.Logger.LogError("Bool required to set toggle state! ID:" + GetOptionID());
    }

    public void OnValueChanged(Toggle change)
    {
        onChangeAction(change.isOn, toggle.interactable);
    }
}

