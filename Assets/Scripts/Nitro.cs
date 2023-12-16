using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Nitro : MonoBehaviour
{
    [SerializeField] private Image nitroFill;
    [SerializeField] private float maxNitro = 100;
    float currentNitroLevel;
    //float consumeFactor = 20f;

    public bool NitroDepleted { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        currentNitroLevel = maxNitro;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentNitroLevel <= 0)
            NitroDepleted = true;
    }

    public void ConsumeNitroBar(float consumeFactor)
    {

        currentNitroLevel -= consumeFactor * Time.deltaTime;
        nitroFill.fillAmount = currentNitroLevel / maxNitro;


    }
}
