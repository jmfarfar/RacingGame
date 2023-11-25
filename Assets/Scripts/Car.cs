using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

enum Gears
{
    Gear1, Gear2, Gear3, Gear4, Gear5
}

public class Car : MonoBehaviour, IVehicle
{
    [Header("Gears Setup")]
    [SerializeField] List<float> gearsMaxVelocities = new List<float>();
    //[SerializeField] float maxVelocityG1;
    //[SerializeField] float maxVelocityG2;
    //[SerializeField] float maxVelocityG3;
    //[SerializeField] float maxVelocityG4;
    //[SerializeField] float maxVelocityG5;

    [Header("Physics")]
    public Rigidbody rigidBody;
    public float accelForce;
    public float turbo;
    public float brakeForce;
    public float forwardFriction;
    public float lateralFriction;
    public float turnSpeed;
    public Transform visual;
    public bool hasItem;
    public AnimationCurve acceleration;
    public float maxVelocity;
    public Nitro nitro;

    private SphereCollider sphereCollider;
    private VehicleControls inputActions;
    private VehicleControls.GameActions carActions;
    private bool grounded;
    private float carRotation = 0f;
    [SerializeField] private float carRotationSpeed;
    float normalizedForwardMovement;
    Gears currentGear = Gears.Gear1;
    private float accelBkp;

    private float sf;


    private void Awake()
    {
        inputActions = new VehicleControls();
        inputActions.Enable();
        carActions = inputActions.Game;
        sphereCollider = rigidBody.GetOrAddComponent<SphereCollider>();
        inputActions.Game.GearUp.performed += OnGearUp;
        inputActions.Game.GearDown.performed += OnGearDown;
    }
    // Start is called before the first frame update
    void Start()
    {
        visual.Translate(0f, -sphereCollider.radius, 0f);
        accelBkp = accelForce;
    }

    // Update is called once per frame
    void Update()
    {
        Steer();
        transform.position = rigidBody.position;

        if ((gearsMaxVelocities[(int)currentGear] - (rigidBody.velocity.magnitude * 3.6f)) > 20f && currentGear != Gears.Gear1)
        {
            accelForce /= 2;
            
        }
        else
        {
            accelForce = accelBkp;
        }

    }

    private void FixedUpdate()
    {
        maxVelocity = currentGear switch
        {
            Gears.Gear1 => gearsMaxVelocities[0],
            Gears.Gear2 => gearsMaxVelocities[1],
            Gears.Gear3 => gearsMaxVelocities[2],
            Gears.Gear4 => gearsMaxVelocities[3],
            Gears.Gear5 => gearsMaxVelocities[4],
            _ => maxVelocity
        };

        Movement();
        //Debug.Log("m/s: " + rigidBody.velocity.magnitude);
    }

    private void OnGUI()
    {
        GUIStyle fontSize = new GUIStyle(GUI.skin.GetStyle("label"));
        fontSize.fontSize = 40;

        GUI.Label(new Rect(300, 50, 600, 100), "curve: " + (1 - acceleration.Evaluate(sf / (maxVelocity / 3.6f))), fontSize);
        GUI.Label(new Rect(300, 100, 600, 100), "km/h: " + rigidBody.velocity.magnitude * 3.6f, fontSize);
        GUI.Label(new Rect(300, 150, 600, 100), "Gear: " + currentGear, fontSize);
    }

    void OnGearUp(InputAction.CallbackContext context)
    {
        if (currentGear < Gears.Gear5)
        {
            currentGear++;
            
        }
    }

    void OnGearDown(InputAction.CallbackContext context)
    {
        if (currentGear > Gears.Gear1)
        {
            currentGear--;
            
        }
    }

    public void Drift()
    {
        turnSpeed = 135f;
    }

    public void RecoverTraction()
    {
        turnSpeed = 90f;
    }

    public void ActivateTurbo()
    {
        nitro.ConsumeNitroBar();
        rigidBody.AddForce(transform.forward * turbo * (1 - acceleration.Evaluate(sf / (maxVelocity / 3.6f))) * accelForce * carActions.Accelerate.ReadValue<float>(), ForceMode.Acceleration);

    }


    public void Movement()
    {
        var speedForward = Vector3.Dot(rigidBody.velocity, transform.forward);
        var speedLateral = Vector3.Dot(rigidBody.velocity, transform.right);

        sf = rigidBody.velocity.magnitude;

        normalizedForwardMovement = Mathf.Clamp(speedForward / 30f, -1f, 1f);

        grounded = Physics.SphereCast(rigidBody.position + transform.up * 0.1f, sphereCollider.radius, -transform.up, out var hit, 0.2f);
        if (grounded)
        {
            //rigidBody.AddTorque(transform.right * carActions.Accelerate.ReadValue<float>() * accelForce, ForceMode.Force);
            if (carActions.Turbo.IsPressed() && !nitro.NitroDepleted)
                ActivateTurbo();
            else
                rigidBody.AddForce(transform.forward * (1 - acceleration.Evaluate(sf / (maxVelocity / 3.6f))) * accelForce * carActions.Accelerate.ReadValue<float>(), ForceMode.Acceleration);

            rigidBody.AddForce(-transform.forward * brakeForce * carActions.Brake.ReadValue<float>(), ForceMode.Acceleration);
            rigidBody.AddForce(-transform.right * Mathf.Abs(speedLateral * lateralFriction) * Mathf.Sign(speedLateral), ForceMode.Acceleration);
            rigidBody.AddForce(-transform.forward * Mathf.Min(Mathf.Abs(speedForward), forwardFriction) * Mathf.Sign(speedForward), ForceMode.Acceleration);

            var fwd = Vector3.Cross(transform.right, hit.normal);
            Quaternion targetRotation = Quaternion.LookRotation(fwd, hit.normal);
            visual.rotation = Quaternion.Lerp(visual.rotation, targetRotation, carRotationSpeed * 2f * Time.deltaTime);

        }
    }

    public void Steer()
    {
        if (grounded)
        {
            carRotation = Mathf.Lerp(carRotation, carActions.Steer.ReadValue<float>(), carRotationSpeed * Time.deltaTime);
            transform.Rotate(transform.up, carRotation * turnSpeed * normalizedForwardMovement * Time.deltaTime);

            if (carActions.Drift.IsPressed())
                Drift();
            else
                RecoverTraction();
        }
    }
}
