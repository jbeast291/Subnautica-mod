using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UWE;

namespace LifePodRemastered.Monos;

internal class PowerRelayController : MonoBehaviour
{
    PowerRelay PRelay;
    PowerFX PFX;
    bool loaded = false;
    GameObject PFXPrefab;

    Rigidbody EscapePodRigidbody;

    int powerDistance = 15;

    public void Start()
    {
        CoroutineHost.StartCoroutine(getPowerFXPrefab());
        Action onloaded = OnLoaded;
        Nautilus.Utility.SaveUtils.RegisterOneTimeUseOnLoadEvent(onloaded);
    }

    public IEnumerator getPowerFXPrefab()
    {
        BepInExEntry.Logger.LogInfo("Loading Solar Panel Prefab...");
        CoroutineTask<GameObject> solarPanelTask = CraftData.GetPrefabForTechTypeAsync(TechType.SolarPanel);
        yield return solarPanelTask;

        GameObject SolarPanelGameObject = solarPanelTask.GetResult();
        PowerFX PFX = SolarPanelGameObject.GetComponent<PowerFX>();
        PFXPrefab = Instantiate(PFX.vfxPrefab);
        PFXPrefab.SetActive(false);
        BepInExEntry.Logger.LogInfo("Loaded Solar Panel Prefab for PowerFX!");
    }

    public void OnLoaded()
    {
        CoroutineHost.StartCoroutine(SetupLifePodPowerRelay());
    }

    public IEnumerator SetupLifePodPowerRelay()
    {
        yield return new WaitUntil(() => PFXPrefab != null);
        PRelay = EscapePod.main.GetComponent<PowerRelay>();
        PRelay.dontConnectToRelays = false;
        PRelay.maxOutboundDistance = powerDistance;

        PFX = EscapePod.main.gameObject.AddComponent<PowerFX>();
        PRelay.powerFX = PFX;
        PFX.vfxPrefab = PFXPrefab;

        EscapePodRigidbody = EscapePod.main.gameObject.GetComponent<Rigidbody>();

        CoroutineHost.StartCoroutine(ReloadConnectionAsyncLoop());
        CoroutineHost.StartCoroutine(UpdateVisualTether());
    }

    public IEnumerator ReloadConnectionAsyncLoop()
    {
        CoroutineHost.StartCoroutine(PRelay.UpdateConnectionAsync());
        yield return new WaitForSeconds(5 + UnityEngine.Random.value);
        CoroutineHost.StartCoroutine(ReloadConnectionAsyncLoop());
    }

    public IEnumerator UpdateVisualTether()
    {
        if(PFX.target != null && EscapePodRigidbody.isKinematic == false)
        {
            PFX.UpdateTarget();
        }
        yield return new WaitForEndOfFrame();
        CoroutineHost.StartCoroutine(UpdateVisualTether());
    }
}

