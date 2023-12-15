using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor : MonoBehaviour
{
    const float G = 67.4f;
    public Rigidbody rb;
    public static List<Attractor> attractors;

    void Attract(Attractor objToAttract)
    {
        Rigidbody rbToAttract = objToAttract.rb;

        Vector3 direction = rb.position - rbToAttract.position;
        float distance = direction.magnitude;

        if (distance == 0f)
        {
            return;
        }

        float forceMagnitude = G * (rb.mass * rbToAttract.mass) / Mathf.Pow(distance, 2);
        Vector3 force = direction.normalized * forceMagnitude;

        rbToAttract.AddForce(force);
    }

    private void FixedUpdate()
    {
        foreach (Attractor attractor in attractors)
        {
            if (attractor != this)
            {
                Attract(attractor);

            }
        }
    }

    private void OnEnable()
    {
        if (attractors == null)
            attractors = new List<Attractor>();

        attractors.Add(this);
    }
    private void OnDisable()
    {
        attractors.Remove(this);
    }
}
