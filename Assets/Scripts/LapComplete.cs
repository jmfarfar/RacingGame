using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LapComplete : MonoBehaviour
{

    public GameObject lapCompleteTrig;
    public GameObject halfLapTrig;


    public int lapsDone;
    [SerializeField] private int lapsToFinish; 

    [SerializeField] private HUDManager hudManager;


    void Start()
    {
        lapsToFinish = hudManager.lapsToFinish;
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {

            lapsDone += 1;
            hudManager.UpdateLapCounter(lapsDone);
            hudManager.ShowLap();            


            halfLapTrig.SetActive(true);
            lapCompleteTrig.SetActive(false);
        }
    }



    public bool UnlockedNextScene()
    {
        if (lapsToFinish == lapsDone && hudManager.KeysComplete())
        {
            Debug.Log("Next Scene unlocked");
            return true;
        }
        else return false;
    }

}
