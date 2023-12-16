using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTexture : MonoBehaviour
{
    [System.Serializable]
    public struct MaterialSets
    {
        public Material[] materials;
    }
    public MaterialSets[] BodyColor;
    Material currMat;
    Renderer renderer;

    private StoreMaterials storeMaterials;


    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
        storeMaterials = FindAnyObjectByType<StoreMaterials>();
    }

        // Update is called once per frame
    void Update()
    {
        
    }

    public void purpleColor()
    {
        if (renderer == null){return;}
        renderer.sharedMaterials = BodyColor[0].materials;
        currMat = renderer.sharedMaterial;
        storeMaterials.Store();
    }

    public void greenColor()
    {
        if (renderer == null){return;}
        renderer.sharedMaterials = BodyColor[1].materials;
        currMat = renderer.sharedMaterial;
        storeMaterials.Store();
    }

    public void blackColor()
    {
        if (renderer == null){return;}
        renderer.sharedMaterials = BodyColor[2].materials;
        currMat = renderer.sharedMaterial;
        storeMaterials.Store();
    }

}
