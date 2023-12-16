using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarModel : MonoBehaviour
{
    [SerializeField] private GameObject[] carModel;


    public void Awake()
    {
        ChooseCarModel(SaveManager_Car.instance.currentCar);
    }

    private void ChooseCarModel(int _index)
    {
        Instantiate(carModel[_index], transform.position, Quaternion.identity, transform);
    }
}
