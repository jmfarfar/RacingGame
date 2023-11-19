using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum DriveType
{
    RearWheel,
    FrontWheel,
    AllWheel
}

public class Drive : MonoBehaviour
{

    [SerializeField] private float maxAngle = 30f;    
    [SerializeField] private float maxTorque = 300f;    
    [SerializeField] private float brakeTorque = 30000f;
    [SerializeField] private GameObject wheelShape;

    [SerializeField] private float criticalSpeed = 5f;
    [SerializeField] private int stepBelow = 5;
    [SerializeField] private int stepAbove = 1;

    [SerializeField] private DriveType driveType;
    WheelCollider[] wheels;
    float handBrake, torque;
    public float angle;

    public InputActionAsset inputActions;
    InputActionMap gameplayActionMap;
    InputAction brakeInputAction;
    InputAction accelerateInputAction;
    InputAction steerInputAction;

    // Start is called before the first frame update
    void Awake()
    {
        gameplayActionMap = inputActions.FindActionMap("Game");

        brakeInputAction = gameplayActionMap.FindAction("Brake");
        accelerateInputAction = gameplayActionMap.FindAction("Accelerate");
        steerInputAction = gameplayActionMap.FindAction("Steer");

        brakeInputAction.performed += GetBrakeInput;
        brakeInputAction.canceled += GetBrakeInput;

        steerInputAction.performed += GetAngleInput;
        steerInputAction.canceled += GetAngleInput;

        accelerateInputAction.performed += GetTorqueInput;
        accelerateInputAction.canceled += GetTorqueInput;
    }

    private void Start()
    {
        wheels = GetComponentsInChildren<WheelCollider>();

        for (int i = 0; i < wheels.Length; i++)
        {
            var wheel = wheels[i];

            if (wheelShape != null)
            {
                var ws = Instantiate(wheelShape);
                ws.transform.parent = wheel.transform;
            }
        }
    }

    void Update()
    {
        wheels[0].ConfigureVehicleSubsteps(criticalSpeed, stepBelow, stepAbove);

        foreach (var wheel in wheels)
        {
            if (wheel.transform.localPosition.z > 0)
            {
                wheel.steerAngle = angle;
            }

            if (wheel.transform.localPosition.z < 0)
            {
                wheel.brakeTorque = handBrake;
            }

            if (wheel.transform.localPosition.z < 0 && driveType != DriveType.FrontWheel)
            {
                wheel.motorTorque = torque;
            }

            if (wheel.transform.localPosition.z > 0 && driveType != DriveType.RearWheel)
            {
                wheel.motorTorque = torque;
            }

            if (wheelShape)
            {
                Quaternion q;
                Vector3 p;
                wheel.GetWorldPose(out p, out q);

                Transform shapeTransform = wheel.transform.GetChild(0);

                if (wheel.name == "w0l" || wheel.name == "w1l" || wheel.name == "w2l")
                {
                    shapeTransform.rotation = q * Quaternion.Euler(0, 180, 0);
                    shapeTransform.position = p;
                }
                else
                {
                    shapeTransform.position = p;
                    shapeTransform.rotation = q;
                }
            }
        }
    }

    void GetBrakeInput(InputAction.CallbackContext context)
    {
        handBrake = context.ReadValue<float>() * brakeTorque;
    }

    void GetAngleInput(InputAction.CallbackContext context)
    {
        angle = context.ReadValue<float>() * maxAngle;
    }

    void GetTorqueInput(InputAction.CallbackContext context)
    {
        torque = context.ReadValue<float>() * maxTorque;
    }

    private void OnEnable()
    {
        brakeInputAction.Enable();
        accelerateInputAction.Enable();
        steerInputAction.Enable();
    }

    private void OnDisable()
    {
        steerInputAction.Disable();
        brakeInputAction.Disable();
        accelerateInputAction.Disable();
    }
}
