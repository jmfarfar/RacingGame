using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Motorcycle : MonoBehaviour, IVehicle
{
    [Header("Gears Setup")]
    [SerializeField] List<float> gearsMaxVelocities = new List<float>();

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
    private VehicleControls.Player2Actions carActions;
    private bool grounded;
    private float carRotation = 0f;
    [SerializeField] private float carRotationSpeed;
    float normalizedForwardMovement;
    Gears currentGear = Gears.Gear1;
    private float accelBkp;
    private float maxSpeedFactor = 0.2f;
    private float minAcceleration = 0.1f;
    private Animator animator;

    private float sf;


    private void Awake()
    {
        inputActions = new VehicleControls();
        inputActions.Enable();
        carActions = inputActions.Player2;
        sphereCollider = rigidBody.GetOrAddComponent<SphereCollider>();
        inputActions.Player2.GearUp.performed += OnGearUp;
        inputActions.Player2.GearDown.performed += OnGearDown;
        //inputActions.Player2.Willie.performed += OnWillie;
    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        visual.Translate(0f, -sphereCollider.radius, 0f);
        accelBkp = accelForce;

        for (int i = 1; i <= gearsMaxVelocities.Count; i++)
        {
            gearsMaxVelocities[i - 1] = (maxVelocity / 5f) * i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Steer();
        transform.position = rigidBody.position;

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

    //private void OnGUI()
    //{
    //    GUIStyle fontSize = new GUIStyle(GUI.skin.GetStyle("label"));
    //    fontSize.fontSize = 40;

    //    GUI.Label(new Rect(300, 50, 600, 100), "curve: " + (1 - acceleration.Evaluate(sf / (maxVelocity / 3.6f))), fontSize);
    //    GUI.Label(new Rect(300, 100, 600, 100), "km/h: " + rigidBody.velocity.magnitude * 3.6f, fontSize);
    //    GUI.Label(new Rect(300, 150, 600, 100), "m/s: " + rigidBody.velocity.magnitude, fontSize);
    //    GUI.Label(new Rect(300, 200, 600, 100), "Gear: " + currentGear, fontSize);
    //    GUI.Label(new Rect(300, 250, 600, 100), "Velocity From Gear Range: " + VelocityFromGearRange((sf / 3.6f) / (maxVelocity / 3.6f)), fontSize);
    //    GUI.Label(new Rect(300, 300, 600, 100), "Velocity Normalized: " + (sf / 3.6f) / (maxVelocity / 3.6f), fontSize);
    //}

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
        turnSpeed = 180f;
        lateralFriction = 0.5f;
    }

    public void RecoverTraction()
    {
        turnSpeed = 120f;
        lateralFriction = 2.0f;
    }

    public void ActivateTurbo()
    {
        nitro.ConsumeNitroBar(20f);
        rigidBody.AddForce(transform.forward * turbo * acceleration.Evaluate(VelocityFromGearRange((sf / 3.6f) / (maxVelocity / 3.6f))) * accelForce * carActions.Accelerate.ReadValue<float>(), ForceMode.Acceleration);

    }


    public void Movement()
    {
        var speedForward = Vector3.Dot(rigidBody.velocity, transform.forward);
        var speedLateral = Vector3.Dot(rigidBody.velocity, transform.right);

        sf = rigidBody.velocity.magnitude * 3.6f;

        normalizedForwardMovement = Mathf.Clamp(speedForward / 30f, -1f, 1f);

        grounded = Physics.SphereCast(rigidBody.position + transform.up * 0.1f, sphereCollider.radius, -transform.up, out var hit, 0.2f);
        if (grounded)
        {
            if (carActions.Turbo.IsPressed() && !nitro.NitroDepleted)
                ActivateTurbo();
            else
                rigidBody.AddForce(transform.forward * acceleration.Evaluate(VelocityFromGearRange((sf / 3.6f) / (maxVelocity / 3.6f))) * accelForce * carActions.Accelerate.ReadValue<float>(), ForceMode.Acceleration);

            //if (carActions.Willie.IsPressed())
            //{
            //    Willie();
            //}
            //else
            //{
            //    NormalPosition();
            //}
            rigidBody.AddForce(-transform.forward * brakeForce * carActions.Brake.ReadValue<float>(), ForceMode.Acceleration);
            rigidBody.AddForce(-transform.right * Mathf.Abs(speedLateral * lateralFriction) * Mathf.Sign(speedLateral), ForceMode.Acceleration);
            rigidBody.AddForce(-transform.forward * Mathf.Min(Mathf.Abs(speedForward), forwardFriction) * Mathf.Sign(speedForward), ForceMode.Acceleration);

            Vector3 horizontalMovement = rigidBody.velocity;
            horizontalMovement.y = 0f;
            horizontalMovement = Vector3.ClampMagnitude(horizontalMovement, maxVelocity / 3.6f);
            horizontalMovement.y = rigidBody.velocity.y;
            rigidBody.velocity = horizontalMovement;

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

    // sets a min and max velocity according to the current gear of the vehicle to avoid players to begin on 5th gear
    private float VelocityFromGearRange(float x)
    {
        // 1 / (gear - 1 * maxSpeedFactor) - (gear * maxSpeedFactor) * (X + minAcceleration) + (gear - 1 * maxSpeedFactor) / (gear * maxSpeedFactor) - (gear - 1 * maxSpeedFactor)
        float a = (((int)currentGear - 3f) * maxSpeedFactor);
        float b = ((int)currentGear * maxSpeedFactor);

        return Mathf.Clamp(((1 / (b - a)) * x) + (a / (a - b)), 1 / ((int)currentGear * (int)currentGear * (int)currentGear * (int)currentGear), (int)currentGear * maxSpeedFactor);
    }

    
}
