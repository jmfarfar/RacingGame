using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Car : Vehicle
{
    public float amplitude = 0.2f;
    public float frequency = 1f;

    [SerializeField] private float maxAngle = 30f;
    [SerializeField] private float maxTorque = 300f;
    [SerializeField] private float brakeTorque = 30000f;

    Vector3 posOffset = new Vector3();
    Vector3 tempPos = new Vector3();

    public InputActionAsset inputActions;
    InputActionMap gameplayActionMap;
    InputAction brakeInputAction;
    InputAction accelerateInputAction;
    InputAction steerInputAction;

    float handBrake, torque;
    public float angle;

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

    // Start is called before the first frame update
    void Start()
    {
        posOffset = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.position = tempPos;
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
