using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class StoreMaterials : MonoBehaviour
{
    [SerializeField] private MaterialDataScriptableObject materialData;
    [SerializeField] private GameObject[] gameObjects;

    private void Start() 
    {
        if (materialData.materialData.Length != gameObjects.Length){return;}
        
        for (int i = 0; i < gameObjects.Length; i++)
        {
            if (materialData.materialData[i].materials == null){return;}
            gameObjects[i].GetComponent<Renderer>().sharedMaterials = materialData.materialData[i].materials;
        }
    }

    //private void Update() 
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        Store();
    //    }
    //}

    public void Store() 
    {
        materialData.materialData = new MaterialDataScriptableObject.MaterialData[gameObjects.Length];

        for (int i = 0; i < gameObjects.Length; i++)
        {
            MaterialDataScriptableObject.MaterialData data = new MaterialDataScriptableObject.MaterialData();
            data.gameObjectName = gameObjects[i].name;
            data.materials = gameObjects[i].GetComponent<Renderer>().sharedMaterials;

            materialData.materialData[i] = data;
        }
    }
}
