using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LifePodRemastered.presetSystem;

public class Mode
{
    protected EscapePodMainMenu escapePodMainMenu;

    private String nameLanguageKey;
    private String descriptionLanguageKey;

    private bool automaticCameraSpinIn3dMap;

    public Mode(EscapePodMainMenu escapePodMainMenu, String nameLanguageKey, String descriptionLanguageKey, bool automaticCameraSpinIn3dMap)
    {
        this.escapePodMainMenu = escapePodMainMenu;
        this.nameLanguageKey = nameLanguageKey;
        this.descriptionLanguageKey = descriptionLanguageKey;
        this.automaticCameraSpinIn3dMap = automaticCameraSpinIn3dMap;
    }
    public virtual void ToggleMode(bool toggle)
    {
        
    }
    public virtual void ReloadLanguage()
    {

    }
    public virtual void EnableInteraction(bool toggle)
    {

    }
    public virtual String Name()
    {
        return Language.main.Get(nameLanguageKey);
    }
    
    public virtual String Description()
    {
        return Language.main.Get(descriptionLanguageKey);
    }

    public virtual bool AutomaticCameraSpin()
    {
        return automaticCameraSpinIn3dMap;
    }
}

