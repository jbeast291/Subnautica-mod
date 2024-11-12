using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour
{
    public Renderer rend;
    float count = 1;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        count -= 0.01f;
        Color VisableColor = new Color(0, 0, 0, count);
        rend.material.SetColor("_Color", VisableColor);
    }
    

}
