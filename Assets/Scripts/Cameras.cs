using Cinemachine;
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
    private VehicleControls.GameActions actions;
    private int currentCamera = 0;

    // Start is called before the first frame update
    void Awake()
    {
        inputActions = new VehicleControls();
        inputActions.Enable();
        actions = inputActions.Game;
        inputActions.Game.ChangeCamera.performed += OnChangeCamera;
    }

    // Update is called once per frame
    void Update()
    {
        if (inputActions.Game.Brake.IsPressed())
        {
            reverseCamera.MoveToTopOfPrioritySubqueue();
        }
        else
        {
            virtualCameras[currentCamera].MoveToTopOfPrioritySubqueue();
        }
    }

    void OnChangeCamera(InputAction.CallbackContext context)
    {
        if (currentCamera < virtualCameras.Count-1)
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
