using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LifePodRemastered.Monos;

internal class HeavyPodHandTarget : HandTarget, IHandTarget
{

    public void OnHandHover(GUIHand hand)
    {
        HandReticle.main.SetText(HandReticle.TextType.Hand, "LPR.ToggleHeavyPodButtonInGame", true, GameInput.Button.LeftHand);
        HandReticle.main.SetIcon(HandReticle.IconType.Interact, 1f);
    }
    public void OnHandClick(GUIHand guiHand)
    {
        HeavyPodMono.main.ToggleHeavyPod(true);
    }
}

