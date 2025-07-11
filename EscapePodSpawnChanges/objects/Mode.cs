using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LifePodRemastered.objects;

class Mode
{
    protected EscapePodMainMenu escapePodMainMenu;

    private String nameLanguageKey;
    private String descriptionLanguageKey;

    public Mode(EscapePodMainMenu escapePodMainMenu, String nameLanguageKey, String descriptionLanguageKey)
    {
        this.escapePodMainMenu = escapePodMainMenu;
        this.nameLanguageKey = nameLanguageKey;
        this.descriptionLanguageKey = descriptionLanguageKey;
    }
    public virtual void ToggleMode(bool toggle)
    {
        
    }

    public String Name()
    {
        return Language.main.Get(nameLanguageKey);
    }
    
    public String Description()
    {
        return Language.main.Get(descriptionLanguageKey);
    }
}

