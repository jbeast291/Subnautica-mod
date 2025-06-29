using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinChanger : MonoBehaviour
{
    [SerializeField] public SkinnedMeshRenderer meshSkinnedRendererThingy;
    [SerializeField] public Mesh newMesh;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) {
            changeMesh();
        }
    }
    public void changeMesh()
    {
        meshSkinnedRendererThingy.sharedMesh = newMesh;
    }
}
