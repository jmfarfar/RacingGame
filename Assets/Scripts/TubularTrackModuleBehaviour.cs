using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Video;

public class TubularTrackModuleBehaviour : MonoBehaviour
{
    private MeshCollider[] towerObstacles;
    private BoxCollider[] boosterPadsColliders;
    private Transform[] boosterPads;
    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private float towerMovementSpeed;
    [SerializeField] private float towerDirectionChangeSpeed;
    [SerializeField] private int rotationTimeInterval;
    private List<List<Vector3>> towerPositions = new List<List<Vector3>>();
    private List<Vector3> boosterPadRotations = new List<Vector3>();

    void Awake()
    {
        SetupObstacleLists();
        boosterPadsColliders = GetComponentsInChildren<BoxCollider>();
    }

    void Update()
    {
        RaiseAndLowerTowers();

        for (int i = 0; i < boosterPadsColliders.Length; i++)
        {
            boosterPadsColliders[i].transform.localEulerAngles = new Vector3(
                Mathf.Clamp(Mathf.Sin(Time.fixedTime + i) * rotationTimeInterval, 0f, 1f) * 180f,
                0f,
                0f
            );
        }
    }

    private void SetupObstacleLists()
    {
        towerObstacles = GetComponentsInChildren<MeshCollider>();
        for (int i = 0; i < towerObstacles.Length; i++)
        {
            Transform t = towerObstacles[i].transform;
            List<Vector3> positions = new List<Vector3>
            {
                t.position,
                t.position + t.forward * 10f
            };
            towerPositions.Add(positions);
        }
    }

    private void RaiseAndLowerTowers()
    {
        for (int i = 0; i < towerObstacles.Length; i++)
        {
            if ((Time.fixedTime + (i % 10)) % 10f < 5f)
            {
                Transform t = towerObstacles[i].transform;
                t.position = Vector3.Lerp(t.position, towerPositions[i][1], Time.deltaTime * 2f);
            }
            else
            {
                Transform t = towerObstacles[i].transform;
                t.position = Vector3.Lerp(t.position, towerPositions[i][0], Time.deltaTime * 2f);
            }
        }
    }
}
