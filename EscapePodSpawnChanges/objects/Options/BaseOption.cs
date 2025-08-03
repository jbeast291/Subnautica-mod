using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LifePodRemastered.presetSystem;

internal class BaseOption
{
    private string optionID;
    private string titleLanguageKey;
    private string descriptionLanguageKey;

    public BaseOption(string optionID, String titleLanguageKey, String descriptionLanguageKey)
    {
        this.optionID = optionID;
        this.titleLanguageKey = titleLanguageKey;
        this.descriptionLanguageKey = descriptionLanguageKey;
    }

    public virtual void ReloadLanguage()
    {

    }

    public virtual float GetHeight()
    {
        return 0;
    }
    public virtual string GetOptionID()
    {
        return optionID;
    }
    public virtual void SetToggledState(bool interactable, object value)
    {

    }

    public string GetTitleLanguageKey()
    {
        return titleLanguageKey;
    }
    public string GetDescriptionLanguageKey()
    {
        return descriptionLanguageKey;
    }
}

