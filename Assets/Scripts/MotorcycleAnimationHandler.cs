using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MotorcycleAnimationHandler : MonoBehaviour
{
    VehicleControls inputActions;
    VehicleControls.GameActions actions;
    private Animator animator;
    [SerializeField] private Rigidbody rigidbodyObject;
    [SerializeField] private Renderer tireRenderer;
    float forwardVelocity, lateralVelocity, tirePatternOffset;


//ES-2023.11.25_EnemyBehavior
    public Motorcycle motorcycle;

    // Start is called before the first frame update
    void Awake()
    {
        inputActions = new VehicleControls();
        inputActions.Enable();
        actions = inputActions.Game;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        tirePatternOffset += forwardVelocity * 0.1f;

        if( motorcycle.isUser() ) //ES-2023.11.25_EnemyBehavior
        {        
            lateralVelocity = Mathf.Lerp(lateralVelocity, actions.Steer.ReadValue<float>(), 0.1f);
        }
        else
        {
            lateralVelocity = Mathf.Lerp(lateralVelocity, motorcycle.horizontal, 0.1f);
        }

        animator.SetFloat("XVelocity", lateralVelocity);
        animator.SetFloat("YVelocity", Mathf.Lerp(animator.GetFloat("YVelocity"), forwardVelocity, 0.1f));

        tireRenderer.sharedMaterial.SetFloat("_PatternOffset", tirePatternOffset);
    }

    void FixedUpdate()
    {
        forwardVelocity = Mathf.Clamp(Mathf.Round(Vector3.Dot(rigidbodyObject.velocity, transform.forward)), -1f, 1f);

    }
}
