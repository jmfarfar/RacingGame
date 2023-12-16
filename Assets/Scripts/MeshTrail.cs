using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    public float activeTime = 2f;
    public float meshRefreshRate = 0.1f;
    public Transform positionToSpawn;
    private bool isTrailActive;
    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    public Material mat;
    public float meshDestroyDelay = 3f;
    public string shaderVarRef;
    public float shaderVarRate = 0.1f;
    public float shaderVarRefreshRate = 0.05f;
    [SerializeField] private GameObject trail;

    private void Awake()
    {
        trail.SetActive(false);
    }

    public void ActivateTrail()
    {
        StartCoroutine(ActivateTrail(activeTime));
    }


    IEnumerator ActivateTrail(float timeActive)
    {
        while (timeActive > 0)
        {
            timeActive -= meshRefreshRate;
            skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            trail.SetActive(true);
            for(int i = 0; i<skinnedMeshRenderers.Length; i++)
            {
                GameObject gObj = new GameObject();
                gObj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);
                MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
                MeshFilter mf = gObj.AddComponent<MeshFilter>();


                Mesh mesh = new Mesh();
                MeshRenderer renderer = gObj.GetComponent<MeshRenderer>();
                skinnedMeshRenderers[i].BakeMesh(mesh);

                mf.mesh = mesh;
                mr.material = mat;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

                StartCoroutine(AnimateMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRefreshRate));

                Destroy(gObj, meshDestroyDelay);

            }
            yield return new WaitForSeconds(meshRefreshRate);

        }
                            trail.SetActive(false);
     //   isTrailActive = false;
    }

    IEnumerator AnimateMaterialFloat(Material mat, float goal, float rate, float refreshRate)
    {
        float valueToAnimate = mat.GetFloat(shaderVarRef);

        while( valueToAnimate > goal)
        {
            mat.SetFloat(shaderVarRef, valueToAnimate);
            yield return new WaitForSeconds(refreshRate);
        }
    }   
}
