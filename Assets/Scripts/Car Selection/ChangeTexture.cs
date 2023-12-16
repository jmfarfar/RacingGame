using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTexture : MonoBehaviour
{
    public Material[] BodyColor;
    Material currMat;
    Renderer renderer;

    private StoreMaterials storeMaterials;


    // Start is called before the first frame update
    void Start()
    {
        renderer = this.GetComponent<Renderer>();
        storeMaterials = FindAnyObjectByType<StoreMaterials>();
    }

        // Update is called once per frame
    void Update()
    {
        
    }

    public void purpleColor()
    {
        renderer.sharedMaterial = BodyColor[0];
        currMat = renderer.sharedMaterial;
        storeMaterials.Store();
    }

    public void greenColor()
    {
        renderer.sharedMaterial = BodyColor[1];
        currMat = renderer.sharedMaterial;
        storeMaterials.Store();
    }

    public void blackColor()
    {
        renderer.sharedMaterial = BodyColor[2];
        currMat = renderer.sharedMaterial;
        storeMaterials.Store();
    }

}
