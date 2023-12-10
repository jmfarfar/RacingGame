using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cameras : MonoBehaviour
{
    [SerializeField] private CinemachineBrain cineBrain;
    [SerializeField] private List<CinemachineVirtualCamera> virtualCameras;
    [SerializeField] private CinemachineVirtualCamera reverseCamera;

    private VehicleControls inputActions;
    private VehicleControls.Player3Actions actionsP3;
    private VehicleControls.Player4Actions actionsP4;
    private int currentCamera = 0;

    // Start is called before the first frame update
    void Awake()
    {
        inputActions = new VehicleControls();
        inputActions.Enable();
        //actionsP3 = inputActions.Player3;
        //actionsP4 = inputActions.Player4;
        //inputActions.Player3.ChangeCamera.performed += OnChangeCamera;
        //inputActions.Player4.ChangeCamera.performed += OnChangeCamera;
    }

    // Update is called once per frame
    void Update()
    {
        //if (inputActions.Player3.Brake.IsPressed())
        //{
        //    reverseCamera.MoveToTopOfPrioritySubqueue();
        //}
        //else
        //{
        //    virtualCameras[currentCamera].MoveToTopOfPrioritySubqueue();
        //}
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

}
