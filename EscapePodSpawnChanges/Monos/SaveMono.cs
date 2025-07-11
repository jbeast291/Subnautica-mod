using Nautilus.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LifePodRemastered.Monos;

internal class SaveMono : MonoBehaviour
{
    public void Awake()
    {
        if (!Info.newSave)
        {
            SaveUtils.CreateDefaultSlotSettingsIfNotExist();
            SaveUtils.ReadSettingsFromCurrectSlot();
        }
        Action save = OnSave;
        Nautilus.Utility.SaveUtils.RegisterOnSaveEvent(save);
    }
    
    void OnSave()//called when the game is saved
    {
        SaveUtils.WriteSettingsToCurrentSlot();
    }
}
