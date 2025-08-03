using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LifePodRemastered.Monos;

internal class MiniWorldController : MonoBehaviour, IInputHandler
{
    public static MiniWorldController main;
    public GameObject lifePodModel;
    public int uiLayer;

    public float distance = 0.32f;
    public float moveSensitivity = 100f;
    public float sprintMultiplier = 3f;
    public float cameraTurnSpeedWhenNotInControll = 10f;

    public float mapRebuildRate = 0.01f; 

    private float pitch = 45f;//for rolling values, prefered to not change this
    private float yaw = 0f;

    private bool controllActive;


    public GameObject camera;

    public void Awake()
    {
        if (main != null)
        {
            UnityEngine.Debug.LogError($"Duplicate {this.GetType().Name} found!");
            Destroy(this);
            return;
        }
        main = this;
        uiLayer = LayerMask.NameToLayer("Minimap");
    }
    public void Start()
    {
        CreateMapCamera();
        CreateMapSubObjects();
        this.gameObject.SetActive(false);
    }
    public void Update()
    {
        if (EscapePodMainMenu.main == null)
        {
            return;
        }
        if (!controllActive && EscapePodMainMenu.main.GetCurrentMode().AutomaticCameraSpin())
        {
            pitch = 45;
            Vector2 lookDelta = new Vector2(cameraTurnSpeedWhenNotInControll * Time.deltaTime, 0);
            MoveCameraAroundLifePod(lookDelta);
        }
    }
    public void ToggleControll()
    {
        controllActive = !controllActive;
        if (controllActive)
        {
            InputHandlerStack.main.Push(this);
            UWE.Utils.lockCursor = true;
        } else
        {
            UWE.Utils.lockCursor = false;
        }
    }

    public void CreateMapCamera()
    {
        camera = new GameObject("MiniMapCamera");

        Camera camComp = camera.AddComponent<Camera>();

        // Culling mask to only render the "Minimap" layer
        camComp.cullingMask = 1 << uiLayer;
        camComp.clearFlags = CameraClearFlags.Depth;
        camComp.depth = 1;
        camComp.fieldOfView = 60;

        camera.transform.position = new Vector3(0, 9.82f, 0);
        camera.transform.rotation = Quaternion.Euler(90, 0, 0);
    }
    public void CreateMapSubObjects()
    {
        this.gameObject.transform.position = Vector3.zero;
        this.gameObject.AddComponent<MiniWorld>();
        this.gameObject.transform.position = new Vector3(0, 9.5f, 0);

        GameObject hologramHolder = new GameObject("hologram holder");
        hologramHolder.transform.SetParent(this.gameObject.transform, false);
        hologramHolder.transform.localPosition = Vector3.zero;
        hologramHolder.transform.localRotation = Quaternion.identity;
        hologramHolder.transform.localScale = Vector3.one;

        GameObject holograms = new GameObject("holograms");
        holograms.transform.SetParent(hologramHolder.transform, false);
        holograms.transform.localPosition = Vector3.zero;
        holograms.transform.localRotation = Quaternion.identity;
        holograms.transform.localScale = Vector3.one;

        MiniWorld mw = this.gameObject.GetComponent<MiniWorld>();
        mw.hologramHolder = hologramHolder.transform;
        mw.hologramObject = holograms;
        mw.hologramMaterial = LPRGlobals.assetBundle.LoadAsset<Material>("SolidHologram2");
        mw.mapWorldRadius = 400;
        mw.mapColor = new Color(0.2f, 0.2f, 0.5f, 1);

        lifePodModel = Instantiate(LPRGlobals.assetBundle.LoadAsset<GameObject>("life_pod_simple"));
        lifePodModel.transform.SetParent(this.gameObject.transform.transform, false);
        lifePodModel.transform.localPosition = new Vector3(0, 0, 0);
        lifePodModel.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        lifePodModel.layer = uiLayer;
    }

    public bool HandleInput()
    {
        if (GameInput.GetButtonDown(GameInput.Button.UICancel))
        {
            ToggleControll();
        }

        Vector2 lookDelta = GameInput.GetLookDelta();
        MoveCameraAroundLifePod(lookDelta);

        Vector3 moveDirection = GameInput.GetMoveDirection();
        //based on camera rotation
        Vector3 forward = camera.transform.forward;
        Vector3 right = camera.transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * moveDirection.z + right * moveDirection.x;

        //re-add y movement
        move.y = moveDirection.y;
        move *= Time.deltaTime * moveSensitivity;
        if (GameInput.GetButtonHeld(GameInput.Button.Sprint))
        {
            move *= sprintMultiplier;
        }
        LPRGlobals.SelectedSpawn += move;
        EscapePodMainMenu.main.UpdateCoordsDisplay();
        return controllActive;
    }
    public void MoveCameraAroundLifePod(Vector2 lookDelta)
    {
        yaw += lookDelta.x;
        pitch -= lookDelta.y;
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        Vector3 offset = Quaternion.Euler(pitch, yaw, 0f) * new Vector3(0, 0, -distance);
        camera.transform.position = lifePodModel.transform.position + offset;
        camera.transform.LookAt(lifePodModel.transform.position);
    }
    public void HideMiniworld()
    {
        this.gameObject.SetActive(false);
    }
    public void ShowMiniworld()
    {
        this.gameObject.SetActive(true);
    }

    public bool HandleLateInput()
    {
        return true;
    }

    public void OnFocusChanged(InputFocusMode mode)
    {

    }
    public static void instantiateMiniWorldMainMenu()
    {
        if(MiniWorldController.main != null)
        {
            UnityEngine.Debug.LogError("Duplicate MiniWorld found!");
            return;
        }
        GameObject miniWorld = new GameObject("miniworld");
        SceneManager.MoveGameObjectToScene(miniWorld, SceneManager.GetSceneByName("MenuEnvironment"));
        miniWorld.AddComponent<MiniWorldController>();
    }
}

