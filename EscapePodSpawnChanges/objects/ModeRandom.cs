using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace LifePodRemastered.objects;

internal class ModeRandom : Mode
{
    GameObject randomRoot;
    public ModeRandom(EscapePodMainMenu escapePodMainMenu, String nameLanguageKey, String descriptionLanguageKey, GameObject randomRoot)
    : base(escapePodMainMenu, nameLanguageKey, descriptionLanguageKey)
    {
        this.randomRoot = randomRoot;

        randomRoot.GetComponent<Button>().onClick.AddListener(OnRandomizePointButtonClick);
        //TextMeshProUGUI PresetTextTEXT = PresetText.GetComponent<TextMeshProUGUI>();

        //PresetTextTEXT.text = selectedPreset[0];
    }
    public override void ToggleMode(bool toggle)
    {
        randomRoot.SetActive(toggle);
    }
    void OnRandomizePointButtonClick()
    {
        Info.OverideSpawnHeight = false;
        Vector3 ranvector3 = new Vector3(Random.Range(-2000, 2000), 0, Random.Range(-2000, 2000));

        escapePodMainMenu.MoveSelecedPointFromWorldPoint(ranvector3);
        escapePodMainMenu.PlayButtonPressSound();
    }
}

