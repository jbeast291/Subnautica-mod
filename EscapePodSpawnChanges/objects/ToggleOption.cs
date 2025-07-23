using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LifePodRemastered.objects;

internal class ToggleOption
{
    private Toggle toggle;
    private RectTransform rt;

    private TextMeshProUGUI title;
    private TextMeshProUGUI description;

    private string titleLanguageKey;
    private string descriptionLanguageKey;

    private Action<bool> onChangeAction;

    public ToggleOption(Toggle toggle, String titleLanguageKey, String descriptionLanguageKey, bool initialValue, Action<bool> onChangeAction)
    {
        this.onChangeAction = onChangeAction;
        this.toggle = toggle;
        this.titleLanguageKey = titleLanguageKey;
        this.descriptionLanguageKey = descriptionLanguageKey;

        title = toggle.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        description = toggle.transform.GetChild(2).GetComponent<TextMeshProUGUI>();

        reloadLanguage();

        toggle.onValueChanged.AddListener(delegate {
            OnValueChanged(toggle);
        });
    }
    
    public void reloadLanguage()
    {
        title.text = Language.main.Get(titleLanguageKey);
        title.ForceMeshUpdate();
        description.text = Language.main.Get(descriptionLanguageKey);
        description.ForceMeshUpdate();//dont know if these are needed yet !!!

        rt = toggle.gameObject.GetComponent<RectTransform>();

        float preferredHeight = description.preferredHeight + title.preferredHeight;

        rt.sizeDelta = new Vector2(rt.sizeDelta.x, preferredHeight);
    }
    public float GetHeight()
    {
        return rt.rect.height;
    }

    public void OnValueChanged(Toggle change)
    {
        onChangeAction(change.isOn);
    }
}

