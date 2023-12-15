using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

enum Gears
{
    Gear1 = 1, Gear2 = 2, Gear3 = 3, Gear4 = 4, Gear5 = 5
}

//ES-2023.11.25_EnemyBehavior
public enum driver
{
    AI,
    User,
}

public class Car : MonoBehaviour, IVehicle
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
    private VehicleControls.Player3Actions carActions;
    private bool grounded;
    private float carRotation = 0f;
    [SerializeField] private float carRotationSpeed;
    float normalizedForwardMovement;
    Gears currentGear = Gears.Gear1;
    private float accelBkp;
    private float maxSpeedFactor = 0.2f;
    private float minAcceleration = 0.1f;

    private float sf;

    //ES-2023.11.25_EnemyBehavior
    [Header("EnemyAI")]
    [SerializeField] public driver driverController;
    public Transform currentWaypoint;
    [HideInInspector] public float horizontal;
    public TrackWaypoints wayPoints;
    private List<Transform> nodes = new List<Transform>();
    public float radius = 6;
    public int distanceOffset = 1;
    private int currentNode;
    [Range(-1, 1)] public int accelerationAI = 1;
    [Range(1, 0)] public int brakPower = 0;
    public float KPH;
    [SerializeField][Range(0, 5)] private float steerForce;
    [SerializeField] private AnimationCurve enginePowerAI;
    private float minRPM = 0;
    private Cameras cameras;


    private void Awake()
    {
        if (isUser()) //ES-2023.11.25_EnemyBehavior
        {
            cameras = GetComponentInChildren<Cameras>();

            inputActions = new VehicleControls();
            inputActions.Enable();
            carActions = inputActions.Player3;
            inputActions.Player3.GearUp.performed += OnGearUp;
            inputActions.Player3.GearDown.performed += OnGearDown;
            inputActions.Player3.ChangeCamera.performed += cameras.OnChangeCamera;


        }

        sphereCollider = rigidBody.GetOrAddComponent<SphereCollider>();

    }

    // Start is called before the first frame update
    void Start()
    {

        visual.Translate(0f, -sphereCollider.radius, 0f);
        accelBkp = accelForce;

        for (int i = 1; i <= gearsMaxVelocities.Count; i++)
        {
            gearsMaxVelocities[i - 1] = (maxVelocity / 5f) * i;
        }

        //ES-2023.11.25_EnemyBehavior
        wayPoints = GameObject.FindGameObjectWithTag("Path").GetComponent<TrackWaypoints>();
        currentWaypoint = gameObject.transform;
        nodes = wayPoints.nodes;
    }

    // Update is called once per frame
    void Update()
    {
        // if (isUser()) //ES-2023.11.25_EnemyBehavior
        // {
        //     Steer();
        //     if (inputActions.Player3.Brake.IsPressed())
        //         cameras.ReverseCamera();
        //     else
        //         cameras.NormalCamera();
        // }
        // else
        //     AIDrive();

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

        //ES-2023.11.25_EnemyBehavior
        if (isUser())
        {
            Movement();
            Steer();
            if (inputActions.Player3.Brake.IsPressed())
                cameras.ReverseCamera();
            else
                cameras.NormalCamera();
        }
        //Debug.Log("m/s: " + rigidBody.velocity.magnitude);
        else
        {
            AIMovement();
            Shifter();
            AIDrive();
        }
    }

    private void OnGUI()
    {
        GUIStyle fontSize = new GUIStyle(GUI.skin.GetStyle("label"));
        fontSize.fontSize = 40;

        GUI.Label(new Rect(300, 50, 600, 100), "curve: " + (1 - acceleration.Evaluate(sf / (maxVelocity / 3.6f))), fontSize);
        GUI.Label(new Rect(300, 100, 600, 100), "km/h: " + rigidBody.velocity.magnitude * 3.6f, fontSize);
        GUI.Label(new Rect(300, 150, 600, 100), "m/s: " + rigidBody.velocity.magnitude, fontSize);
        GUI.Label(new Rect(300, 200, 600, 100), "Gear: " + currentGear, fontSize);
        GUI.Label(new Rect(300, 250, 600, 100), "Velocity From Gear Range: " + VelocityFromGearRange((sf / 3.6f) / (maxVelocity / 3.6f)), fontSize);
        GUI.Label(new Rect(300, 300, 600, 100), "Velocity Normalized: " + (sf / 3.6f) / (maxVelocity / 3.6f), fontSize);
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
        lateralFriction = 0.5f;
    }

    public void RecoverTraction()
    {
        turnSpeed = 90f;
        lateralFriction = 2.0f;
    }

    public void ActivateTurbo()
    {
        nitro.ConsumeNitroBar(10f);
        rigidBody.AddForce(transform.forward * turbo * acceleration.Evaluate(VelocityFromGearRange((sf / 3.6f) / (maxVelocity / 3.6f))) * accelForce * carActions.Accelerate.ReadValue<float>(), ForceMode.Acceleration);

    }


    public void Movement()
    {
        var speedForward = Vector3.Dot(rigidBody.velocity, transform.forward);
        var speedLateral = Vector3.Dot(rigidBody.velocity, transform.right);

        sf = rigidBody.velocity.magnitude * 3.6f;

        normalizedForwardMovement = Mathf.Clamp(speedForward / 30f, -1f, 1f);

        grounded = Physics.SphereCast(rigidBody.position + transform.up * 0.1f, sphereCollider.radius, -transform.up, out var hit, 2f);
        if (grounded)
        {
            if (carActions.Turbo.IsPressed() && !nitro.NitroDepleted)
                ActivateTurbo();
            else
                rigidBody.AddForce(transform.forward * acceleration.Evaluate(VelocityFromGearRange((sf / 3.6f) / (maxVelocity / 3.6f))) * accelForce * carActions.Accelerate.ReadValue<float>(), ForceMode.Acceleration);

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
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, carRotationSpeed * 2f * Time.deltaTime);

        }
    }

    public void Steer()
    {
        if (grounded)
        {
            carRotation = Mathf.Lerp(carRotation, carActions.Steer.ReadValue<float>(), carRotationSpeed * Time.deltaTime);
            transform.Rotate(0f, carRotation * turnSpeed * normalizedForwardMovement * Time.deltaTime, 0f);

            if (carActions.Drift.IsPressed())
                Drift();
            else
                RecoverTraction();
        }
    }

    private float VelocityFromGearRange(float x)
    {
        // 1 / (gear - 1 * maxSpeedFactor) - (gear * maxSpeedFactor) * (X + minAcceleration) + (gear - 1 * maxSpeedFactor) / (gear * maxSpeedFactor) - (gear - 1 * maxSpeedFactor)
        float a = (((int)currentGear - 3f) * maxSpeedFactor);
        float b = ((int)currentGear * maxSpeedFactor);

        return Mathf.Clamp(((1 / (b - a)) * x) + (a / (a - b)), 1 / ((int)currentGear * (int)currentGear * (int)currentGear * (int)currentGear), (int)currentGear * maxSpeedFactor);
    }

    //---------------------------------------------------------------------------------------------------

    private void AIDrive()
    {
        CalculateDistanceOfWaypoints();
        AISteer();
    }

    private void AISteer()
    {
        Vector3 relative = transform.InverseTransformPoint(currentWaypoint.transform.position);
        relative /= relative.magnitude;

        horizontal = (relative.x / relative.magnitude) * steerForce;

        carRotation = Mathf.Lerp(carRotation, horizontal, carRotationSpeed * Time.deltaTime);
        transform.Rotate(transform.up, carRotation * turnSpeed * normalizedForwardMovement * Time.deltaTime);

    }


    private void CalculateDistanceOfWaypoints()
    {
        Vector3 position = gameObject.transform.position;
        float distance = Mathf.Infinity;

        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 difference = nodes[i].transform.position - position;
            float currentDistance = difference.magnitude;
            if (currentDistance < distance)
            {
                if ((i + distanceOffset) >= nodes.Count)
                {
                    currentWaypoint = nodes[1];
                    distance = currentDistance;
                }
                else
                {
                    currentWaypoint = nodes[i + distanceOffset];
                    distance = currentDistance;
                }
                currentNode = i;
            }

        }
    }


    private void AIMovement()
    {
        BrakeVehicle();
        var speedForward = Vector3.Dot(rigidBody.velocity, transform.forward);
        var speedLateral = Vector3.Dot(rigidBody.velocity, transform.right);

        sf = rigidBody.velocity.magnitude * 3.6f;

        normalizedForwardMovement = Mathf.Clamp(speedForward / 30f, -1f, 1f);

        grounded = Physics.SphereCast(rigidBody.position + transform.up * 0.1f, sphereCollider.radius, -transform.up, out var hit, 0.2f);
        if (grounded)
        {

            rigidBody.AddForce(transform.forward * enginePowerAI.Evaluate(VelocityFromGearRange((sf / 3.6f) / (maxVelocity / 3.6f))) * accelForce * accelerationAI, ForceMode.Acceleration);

            rigidBody.AddForce(-transform.forward * brakeForce * brakPower, ForceMode.Acceleration);
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
        KPH = rigidBody.velocity.magnitude * 3.6f;
    }

    private void BrakeVehicle()
    {
        if (accelerationAI < 0) brakPower = 1;
        else brakPower = 0;
    }


    private void Shifter()
    {

        if (!grounded) return;
        // Debug.Log("KPH: " + KPH + " |maxVelocity: " + maxVelocity + " | minVelocity: " + minRPM + "  " + gearsMaxVelocities[(int)currentGear]);
        if (KPH > maxVelocity && currentGear < Gears.Gear5)
        {
            minRPM = maxVelocity;
            currentGear++;
            return;
        }

        if (KPH < minRPM && currentGear > Gears.Gear1)
        {
            if (currentGear - 1 == Gears.Gear1) minRPM = 0;
            else minRPM = gearsMaxVelocities[(int)currentGear - 2];

            currentGear--;
        }
    }

    public bool isUser()
    {
        return driverController == driver.User;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(currentWaypoint.position, 3);
    }

    private void OnDisable()
    {
        if (isUser())
        {
            inputActions.Player3.GearUp.performed -= OnGearUp;
            inputActions.Player3.GearDown.performed -= OnGearDown;
            inputActions.Player3.ChangeCamera.performed -= cameras.OnChangeCamera;

        }
    }

}
