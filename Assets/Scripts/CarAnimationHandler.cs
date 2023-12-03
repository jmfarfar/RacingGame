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

//ES-2023.11.25_EnemyBehavior
    public Car car;

    // Start is called before the first frame update
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
            if( car.isUser() ) //ES-2023.11.25_EnemyBehavior
            {
            parentTires[i].localRotation = Quaternion.Euler(0f, 45f * actions.Steer.ReadValue<float>(), 0f);
            }
            else
            {
            parentTires[i].localRotation = Quaternion.Euler(0f, 45f * car.horizontal, 0f);
            }
        }

        //var frontTires = tires.ToArray().TakeLast(2);
        //foreach (Transform tire in frontTires)
        //{
        //    tire.rotation = Quaternion.Euler(tire.rotation.x, actions.Steer.ReadValue<float>() * Mathf.Rad2Deg * Mathf.Atan(2.55f / (0.39f + (1.5f / 2))), tire.rotation.z);
        //}
    }
}
