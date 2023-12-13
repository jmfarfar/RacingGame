using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTexture : MonoBehaviour
{
    public Material[] BodyColor;
    Material currMat;
    Renderer renderer;


    // Start is called before the first frame update
    void Start()
    {
        renderer = this.GetComponent<Renderer>();
    }

        // Update is called once per frame
    void Update()
    {
        
    }

    public void whiteColor()
    {
        renderer.material = BodyColor[0];
        currMat = renderer.material;
    }

    public void orangeColor()
    {
        renderer.material = BodyColor[1];
        currMat = renderer.material;
    }

    public void violetColor()
    {
        renderer.material = BodyColor[2];
        currMat = renderer.material;
    }

    public void redColor()
    {
        renderer.material = BodyColor[3];
        currMat = renderer.material;
    }
}
