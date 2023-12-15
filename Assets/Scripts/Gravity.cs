using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Gravity : MonoBehaviour
{

    [SerializeField] private Transform visualTrans;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float gravityMagnitude;
    [SerializeField] private MeshCollider mesh;

    Ray ray;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {

        ray = new Ray(rb.position + transform.forward, -visualTrans.up);
        if (mesh.Raycast(ray, out RaycastHit hitInfo, 50f))
        {
            //rb.AddForce(-hitInfo.normal, ForceMode.VelocityChange);
            rb.velocity += -hitInfo.normal * Time.deltaTime * gravityMagnitude;
        }

        //Vector3 vectorToSurface = (mesh.ClosestPoint(rb.position) - rb.position).normalized;
        //Debug.Log("Vector " + vectorToSurface);
        //Debug.Log("Closest Point: " + mesh.ClosestPoint(rb.position));



        //rb.AddForce(vectorToSurface * gravityMagnitude, ForceMode.Acceleration);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray.origin, ray.direction);

        Gizmos.DrawSphere(mesh.ClosestPoint(rb.position), 0.1f);
    }
}
