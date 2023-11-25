using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVehicle
{
    void Drift();
    void RecoverTraction();
    void ActivateTurbo();
    void Movement();
    void Steer();
}
