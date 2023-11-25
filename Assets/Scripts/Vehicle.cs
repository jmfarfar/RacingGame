using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

public abstract class Vehicle : MonoBehaviour
{
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

    public SphereCollider sphereCollider;
    private VehicleControls inputActions;
    private VehicleControls.GameActions carActions;
    private bool grounded;
    private float carRotation = 0f;
    [SerializeField] private float carRotationSpeed;
    float normalizedForwardMovement;

    private float sf;


    private void Awake()
    {
        inputActions = new VehicleControls();
        inputActions.Enable();
        carActions = inputActions.Game;
        sphereCollider = rigidBody.GetOrAddComponent<SphereCollider>();
    }
    // Start is called before the first frame update
    void Start()
    {
        visual.Translate(0f, -sphereCollider.radius, 0f);
    }

    // Update is called once per frame
    void Update()
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
        transform.position = rigidBody.position;
    }

    private void FixedUpdate()
    {
        var speedForward = Vector3.Dot(rigidBody.velocity, transform.forward);
        var speedLateral = Vector3.Dot(rigidBody.velocity, transform.right);

        sf = rigidBody.velocity.magnitude;

        normalizedForwardMovement = Mathf.Clamp(speedForward / 30f, -1f, 1f);

        grounded = Physics.SphereCast(rigidBody.position + transform.up * 0.1f, sphereCollider.radius, -transform.up, out var hit, 0.2f);
        if (grounded)
        {
            //rigidBody.AddTorque(transform.right * carActions.Accelerate.ReadValue<float>() * accelForce, ForceMode.Force);
            if (carActions.Turbo.IsPressed() && hasItem)
                StartCoroutine(ActivateTurbo());
            else
                rigidBody.AddForce(transform.forward * (1 - acceleration.Evaluate(sf / (maxVelocity / 3.6f))) * accelForce * carActions.Accelerate.ReadValue<float>(), ForceMode.Acceleration);

            rigidBody.AddForce(-transform.forward * brakeForce * carActions.Brake.ReadValue<float>(), ForceMode.Acceleration);
            rigidBody.AddForce(-transform.right * Mathf.Abs(speedLateral * lateralFriction) * Mathf.Sign(speedLateral), ForceMode.Acceleration);
            rigidBody.AddForce(-transform.forward * Mathf.Min(Mathf.Abs(speedForward), forwardFriction) * Mathf.Sign(speedForward), ForceMode.Acceleration);

            var fwd = Vector3.Cross(transform.right, hit.normal);
            Quaternion targetRotation = Quaternion.LookRotation(fwd, hit.normal);
            visual.rotation = Quaternion.Lerp(visual.rotation, targetRotation, carRotationSpeed * 2f * Time.deltaTime);

        }

        Debug.Log("m/s: " + rigidBody.velocity.magnitude);
    }

    public abstract void Drift();

    public abstract void RecoverTraction();

    IEnumerator ActivateTurbo()
    {
        rigidBody.AddForce(transform.forward * turbo * (1 - acceleration.Evaluate(sf / (maxVelocity / 3.6f))) * accelForce * carActions.Accelerate.ReadValue<float>(), ForceMode.Acceleration);
        yield return new WaitForSeconds(3f);
        hasItem = false;
    }

    private void OnGUI()
    {
        GUIStyle fontSize = new GUIStyle(GUI.skin.GetStyle("label"));
        fontSize.fontSize = 40;

        GUI.Label(new Rect(300, 50, 600, 100), "curve: " + (1 - acceleration.Evaluate(sf / (maxVelocity / 3.6f))), fontSize);
        GUI.Label(new Rect(300, 100, 600, 100), "km/h: " + rigidBody.velocity.magnitude * 3.6f, fontSize);
    }

}
