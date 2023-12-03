using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarAnimationHandler : MonoBehaviour
{

    [SerializeField] private List<Transform> tires = new List<Transform>();
    [SerializeField] private List<Transform> parentTires = new List<Transform>();
    [SerializeField] private Rigidbody rigidBody;
    private float diameter = 0.78f;
    VehicleControls inputActions;
    VehicleControls.GameActions actions;

    void Awake()
    {
        inputActions = new VehicleControls();
        inputActions.Enable();
        actions = inputActions.Game;
    }

    void Update()
    {
        var speedForward = Vector3.Dot(rigidBody.velocity, transform.forward);

        for (int i = 0; i < tires.Count; i++)
        {
            tires[i].Rotate(speedForward * Time.deltaTime * 180f, 0f, 0f);

        }

        for (int i = 0; i < parentTires.Count; i++)
        {
            parentTires[i].localRotation = Quaternion.Euler(0f, 45f * actions.Steer.ReadValue<float>(), 0f);
        }
    }
}
