using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour
{

    public Car car;
    public GameObject needle;
    public float startPosition;
    public float endPosition;
    public float desiredPosition;
    public float vehicleSpeed;
    public Text speedLabel; 

    private void Update ()
    {
        vehicleSpeed = car.KPH;
        udpateNeedle();
    }

    public void udpateNeedle()
    {
        speedLabel.text = ((int)vehicleSpeed) + " km/h";
        desiredPosition = startPosition - endPosition;
        float temp = vehicleSpeed /180;
     //   needle.transform.eulerAngles = new Vector3(0,0,(startPosition - temp * desiredPosition) );
        needle.transform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(startPosition, endPosition, vehicleSpeed / 180));
    }

}
