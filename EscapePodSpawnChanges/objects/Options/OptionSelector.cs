using FMOD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CraftingAnalytics;
using static QuickSlots;
using static uGUI_OptionsPanel;

namespace LifePodRemastered.presetSystem.Options;

internal class OptionSelector : BaseOption
{
    protected GameObject root;
    protected RectTransform rootRT;

    protected TextMeshProUGUI title;
    protected TextMeshProUGUI description;
    protected TextMeshProUGUI selectedText;

    protected Button LeftButton;
    protected Button RightButton;

    protected int activeSelectedIndex = 0;
    protected List<string> selectableOptionsList;

    protected Action<string, bool> onChangeAction;

    public OptionSelector(string optionID, string titleLanguageKey, string descriptionLanguageKey, GameObject root, string initialValue, List<string> selectableOptionsList, Action<string, bool> onChangeAction) :
        base(optionID, titleLanguageKey, descriptionLanguageKey)
    {
        this.root = root;
        this.onChangeAction = onChangeAction;

        rootRT = root.GetComponent<RectTransform>();

        LeftButton = root.transform.GetChild(1).GetComponent<Button>();
        LeftButton.onClick.AddListener(MoveSelectorLeft);
        RightButton = root.transform.GetChild(3).GetComponent<Button>();
        RightButton.onClick.AddListener(MoveSelectorRight);

        title = root.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        selectedText = root.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        description = root.transform.GetChild(4).GetComponent<TextMeshProUGUI>();

        this.selectableOptionsList = selectableOptionsList;

        int indexOfInitialValue = selectableOptionsList.IndexOf(initialValue);
        this.activeSelectedIndex = indexOfInitialValue != -1 ? indexOfInitialValue : 0;
    }
    public void init()
    {
        ReloadLanguage();
        UpdateSlectedInUi();
    }
    public override void ReloadLanguage()
    {
        title.text = Language.main.Get(base.GetTitleLanguageKey());
        title.ForceMeshUpdate();
        description.text = Language.main.Get(base.GetDescriptionLanguageKey());
        description.ForceMeshUpdate();

        float preferredHeight = description.preferredHeight + title.preferredHeight + LeftButton.GetComponent<RectTransform>().rect.height;

        rootRT.sizeDelta = new Vector2(rootRT.sizeDelta.x, preferredHeight);
    }
    public virtual void UpdateSlectedInUi()
    {
        if(selectableOptionsList.Count() > 0)
        {
            selectedText.text = selectableOptionsList[activeSelectedIndex];
            OnValueChanged(selectableOptionsList[activeSelectedIndex]);
        }
    }
    public void MoveSelectorLeft()
    {
        if (activeSelectedIndex != 0)
        {
            activeSelectedIndex--;
        }
        else
        {
            activeSelectedIndex = selectableOptionsList.Count - 1;
        }
        EscapePodMainMenu.main.PlayButtonPressSound();
        UpdateSlectedInUi();
    }
    public void MoveSelectorRight()
    {
        if (activeSelectedIndex != selectableOptionsList.Count - 1)
        {
            activeSelectedIndex++;
        }
        else
        {
            activeSelectedIndex = 0;
        }
        EscapePodMainMenu.main.PlayButtonPressSound();
        UpdateSlectedInUi();
    }
    public override void SetToggledState(bool interactable, object value)
    {
        if (value is string stringValue)
        {
            LeftButton.interactable = interactable;
            RightButton.interactable = interactable;

            int indexOfInitialValue = selectableOptionsList.IndexOf(stringValue);
            activeSelectedIndex = indexOfInitialValue != -1 ? indexOfInitialValue : 0;
            UpdateSlectedInUi();
            return;
        }
        BepInExEntry.Logger.LogError("String required to set selector state! ID:" + GetOptionID());
    }
    public override float GetHeight()
    {
        return rootRT.rect.height;
    }
    public virtual void OnValueChanged(string selected)
    {
        onChangeAction(selected, LeftButton.interactable);
    }
}

