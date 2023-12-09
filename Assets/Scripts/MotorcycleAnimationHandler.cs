using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MotorcycleAnimationHandler : MonoBehaviour
{
    VehicleControls inputActions;
    VehicleControls.Player2Actions actions;
    private Animator animator;
    [SerializeField] private Rigidbody rigidbodyObject;
    [SerializeField] private Renderer tireRenderer;
    float forwardVelocity, lateralVelocity, tirePatternOffset, willieForce, blendTreeYAxis;


    //ES-2023.11.25_EnemyBehavior
    public Motorcycle motorcycle;

    // Start is called before the first frame update
    void Awake()
    {
        inputActions = new VehicleControls();
        inputActions.Enable();
        actions = inputActions.Player2;
        animator = GetComponent<Animator>();
    }
    //
    private void Start()
    {
        
    }

    private void Update()
    {
        tirePatternOffset += forwardVelocity * 0.1f;

        if (motorcycle.isUser()) //ES-2023.11.25_EnemyBehavior
        {
            lateralVelocity = Mathf.Lerp(lateralVelocity, actions.Steer.ReadValue<float>(), 0.1f);
            
        }
        else
        {
            lateralVelocity = Mathf.Lerp(lateralVelocity, motorcycle.horizontal, 0.1f);
        }
        float x = Mathf.Clamp(forwardVelocity,0f, 1f);
        float y = actions.Willie.ReadValue<float>();

        blendTreeYAxis = x - (1 * y * x) - (y * x);

        animator.SetFloat("XVelocity", lateralVelocity);
        animator.SetFloat("YVelocity", Mathf.Lerp(animator.GetFloat("YVelocity"), blendTreeYAxis, 0.1f));
        
        tireRenderer.sharedMaterial.SetFloat("_PatternOffset", tirePatternOffset);
    }

    void FixedUpdate()
    {
        forwardVelocity = Mathf.Clamp(Mathf.Round(Vector3.Dot(rigidbodyObject.velocity, transform.forward)), -1f, 1f);

    }    
}
