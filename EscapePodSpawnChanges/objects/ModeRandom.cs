using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace LifePodRemastered.objects;

internal class ModeRandom : Mode
{
    GameObject randomRoot;
    TextMeshProUGUI randomButtonText;
    public ModeRandom(EscapePodMainMenu escapePodMainMenu, String nameLanguageKey, String descriptionLanguageKey, GameObject randomRoot)
    : base(escapePodMainMenu, nameLanguageKey, descriptionLanguageKey)
    {
        this.randomRoot = randomRoot;

        randomRoot.GetComponent<Button>().onClick.AddListener(OnRandomizePointButtonClick);
        randomButtonText = randomRoot.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        ReloadLanguage();
    }
    public override void ToggleMode(bool toggle)
    {
        randomRoot.SetActive(toggle);
    }
    public override void ReloadLanguage()
    {
        randomButtonText.text = Language.main.Get("LPR.ModeRandomButton");
    }
    void OnRandomizePointButtonClick()
    {
        Info.OverideSpawnHeight = false;
        Vector3 ranvector3 = new Vector3(Random.Range(-1800, 1800), 0, Random.Range(-1800, 1800));

        escapePodMainMenu.MoveSelecedPointFromWorldPoint(ranvector3);
        escapePodMainMenu.PlayButtonPressSound();
    }
}

