using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfPointTrigger : MonoBehaviour
{

    public GameObject lapCompleteTrigger;
    public GameObject halfCompleteTrigger;

    void OnTriggerEnter()
    {
        lapCompleteTrigger.SetActive(true);
        halfCompleteTrigger.SetActive(false);
    }
}
