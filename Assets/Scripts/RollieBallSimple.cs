using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class RollieBallSimple : MonoBehaviour
{
    public Rigidbody rigidBody;
    public float accelForce;
    public float brakeForce;
    public float forwardFriction;
    public float lateralFriction;
    public float turnSpeed;
    public Transform visual;
    private SphereCollider sphereCollider;
    private CarControls inputActions;
    private CarControls.GameActions carActions;
    private bool grounded;
    private float carRotation = 0f;
    [SerializeField] private float carRotationSpeed;
    float normalizedForwardMovement;


    private void Awake()
    {
        inputActions = new CarControls();
        inputActions.Enable();
        carActions = inputActions.Game;
        sphereCollider = rigidBody.GetOrAddComponent<SphereCollider>();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        if (grounded)
        {
            carRotation = Mathf.Lerp(carRotation, carActions.Steer.ReadValue<float>(), carRotationSpeed * Time.deltaTime);
            transform.Rotate(transform.up, carRotation * turnSpeed * normalizedForwardMovement * Time.deltaTime);
        }
        transform.position = rigidBody.position;
    }

    private void FixedUpdate()
    {
        var speedForward = Vector3.Dot(rigidBody.velocity, transform.forward);
        var speedLateral = Vector3.Dot(rigidBody.velocity, transform.right);
        normalizedForwardMovement = Mathf.Clamp(speedForward / 30f, -1f, 1f);

        grounded = Physics.SphereCast(rigidBody.position + transform.up * 0.1f, sphereCollider.radius, -transform.up, out var hit, 0.2f);
        if (grounded)
        {
            //rigidBody.AddTorque(transform.right * carActions.Accelerate.ReadValue<float>() * accelForce, ForceMode.Force);
            rigidBody.AddForce(transform.forward * accelForce * carActions.Accelerate.ReadValue<float>(), ForceMode.Acceleration);
            rigidBody.AddForce(-transform.forward * brakeForce * carActions.Brake.ReadValue<float>(), ForceMode.Acceleration);
            rigidBody.AddForce(-transform.right * Mathf.Min(Mathf.Abs(speedLateral) * rigidBody.mass, lateralFriction) * Mathf.Sign(speedLateral), ForceMode.Acceleration);
            rigidBody.AddForce(-transform.forward * Mathf.Min(Mathf.Abs(speedForward) * rigidBody.mass, forwardFriction) * Mathf.Sign(speedForward), ForceMode.Acceleration);

            var fwd = Vector3.Cross(transform.right, hit.normal);
            Quaternion targetRotation = Quaternion.LookRotation(fwd, hit.normal);
            visual.rotation = Quaternion.Lerp(visual.rotation, targetRotation, carRotationSpeed * 2f * Time.deltaTime);

        }

        Debug.Log("m/s: " + rigidBody.velocity.magnitude);
    }



}
