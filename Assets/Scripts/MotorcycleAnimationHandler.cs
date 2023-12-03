using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MotorcycleAnimationHandler : MonoBehaviour
{
    VehicleControls inputActions;
    VehicleControls.Player2Actions actions;
    private Animator animator;
    [SerializeField] private Rigidbody rigidbodyObject;
    [SerializeField] private Renderer tireRenderer;
    float forwardVelocity, lateralVelocity, tirePatternOffset, willieForce;


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

        lateralVelocity = Mathf.Lerp(lateralVelocity, actions.Steer.ReadValue<float>(), 0.1f);
        willieForce = Mathf.Lerp(willieForce, actions.Willie.ReadValue<float>(), 0.1f);

        animator.SetFloat("XVelocity", lateralVelocity);
        animator.SetFloat("YVelocity", Mathf.Lerp(animator.GetFloat("YVelocity"), forwardVelocity, 0.1f));
        animator.SetFloat("Willie", willieForce);

        //if (inputActions.Player2.Willie.IsPressed())
        //{
        //    Willie();
        //}
        //else
        //{
        //    NormalPosition();
        //}

        tireRenderer.sharedMaterial.SetFloat("_PatternOffset", tirePatternOffset);
    }

    void FixedUpdate()
    {
        forwardVelocity = Mathf.Clamp(Mathf.Round(Vector3.Dot(rigidbodyObject.velocity, transform.forward)), -1f, 1f);

    }

    //private void Willie()
    //{
    //    animator.Play("Willie");
    //    //animator.SetBool("Willy", true);
    //}

    //private void NormalPosition()
    //{
    //    animator.Play("Normal");
    //    //animator.SetBool("Willy", false);
    //}
}
