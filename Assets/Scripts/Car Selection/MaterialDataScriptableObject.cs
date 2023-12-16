using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MaterialData", menuName = "ScriptableObjects/NewMaterialData", order = 0)]
public class MaterialDataScriptableObject : ScriptableObject 
{
    [Serializable]
    public struct MaterialData
    {
        public string gameObjectName;
        public Material[] materials;
    }

    [SerializeField] public MaterialData[] materialData = new MaterialData[1];
}