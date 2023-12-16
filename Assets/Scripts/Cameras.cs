using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Cameras : MonoBehaviour
{
    [SerializeField] private Camera p1Camera;
    [SerializeField] private Camera p2Camera;
    [SerializeField] private CinemachineBrain cineBrain;
    [SerializeField] private List<CinemachineVirtualCamera> virtualCameras;
    [SerializeField] private CinemachineVirtualCamera reverseCamera;

    private VehicleControls inputActions;
    private int currentCamera = 0;

    // Start is called before the first frame update
    void Awake()
    {
        inputActions = new VehicleControls();
        inputActions.Enable();
        GameManager.OnGameModeSelected += ResizeForTwoPlayers;
    }

    private void Start()
    {
        var cameras = GameObject.FindObjectsOfType<Camera>();
        p1Camera = cameras[1];
        p2Camera = cameras[0];
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReverseCamera()
    {
        reverseCamera.MoveToTopOfPrioritySubqueue();
    }

    public void NormalCamera()
    {
        virtualCameras[currentCamera].MoveToTopOfPrioritySubqueue();
    }

    public void OnChangeCamera(InputAction.CallbackContext context)
    {
        if (currentCamera < virtualCameras.Count - 1)
        {
            currentCamera++;
            virtualCameras[currentCamera].MoveToTopOfPrioritySubqueue();
        }
        else
        {
            // go back to the first camera on the list
            currentCamera = 0;
            virtualCameras[currentCamera].MoveToTopOfPrioritySubqueue();
        }

    }

    public void ResizeForTwoPlayers()
    {
        p1Camera.rect = new Rect(0f, 0.5f, 1f, 1f);
        p2Camera.gameObject.SetActive(true);
        p2Camera.rect = new Rect(0f, 0f, 1f, 0.5f);
    }

    private void OnDisable()
    {
        GameManager.OnGameModeSelected -= ResizeForTwoPlayers;
    }
}
