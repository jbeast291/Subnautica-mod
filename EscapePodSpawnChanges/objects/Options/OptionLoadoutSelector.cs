using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LifePodRemastered.presetSystem.Options;

internal class OptionLoadoutSelector : OptionSelector
{
    ModePreset modePreset;
    public OptionLoadoutSelector(string optionID, string titleLanguageKey, string descriptionLanguageKey, GameObject root, string initialLoadoutName, ModePreset modePresetRef, Action<string, bool> onChangeAction) 
        : base(optionID, titleLanguageKey, descriptionLanguageKey, root, initialLoadoutName, modePresetRef.getPresetFileNames(), onChangeAction)
    {
        this.modePreset = modePresetRef;
    }
    public override void ReloadLanguage()
    {
        title.text = Language.main.Get(base.GetTitleLanguageKey());
        title.ForceMeshUpdate();

        description.text = Language.main.Get(base.GetDescriptionLanguageKey()) + modePreset.getPresetByIndex(base.activeSelectedIndex).getItemListString();
        description.ForceMeshUpdate();

        float preferredHeight = description.preferredHeight + title.preferredHeight + LeftButton.GetComponent<RectTransform>().rect.height;

        rootRT.sizeDelta = new Vector2(rootRT.sizeDelta.x, preferredHeight);
        OptionsMono.main.updateViewPortHeight();
    }
    public override void UpdateSlectedInUi()
    {
        base.UpdateSlectedInUi();
        ReloadLanguage();
    }
    public override void OnValueChanged(string selected)
    {
        base.OnValueChanged(selected);
        LPRGlobals.selectedLoadout = modePreset.getPresetByIndex(base.activeSelectedIndex);
    }
}

