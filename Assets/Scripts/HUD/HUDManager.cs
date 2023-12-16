using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("Nitro")]
        [SerializeField] private Image nitroFill;
        [SerializeField] private float maxNitro = 100;
        float currentNitroLevel;
        public bool NitroDepleted { get; set; }
    [Header("Speedometer")]
        public TextMeshProUGUI speedLabel; 
    [Header("LapTimeManager")]
        public float time, timeLap;
        private int minutes, seconds, cents;
        public TextMeshProUGUI timeLabel; 
    [Header("LapCounter")]
        [SerializeField] private TextMeshProUGUI lapCounterText; 
        [SerializeField] private TextMeshProUGUI totalLapsText;
        public int lapsToFinish = 3;  
        [SerializeField] private TextMeshProUGUI currentLapTimeText;
        public float blinkFadeInTime = 0.5f;
        public float blinkStayTime = 0.8f;
        public float blinkFadeOutTime = 0.7f;
        public float timeChecker = 0;
        private Color color;
        public bool blinkTextFlag = false;
    [Header("KeysCounter")]
        [SerializeField] private int keyCounter, totalKeys=3;     
        [SerializeField] private TextMeshProUGUI keyCounterText; 
        [SerializeField] private TextMeshProUGUI totalKeyText; 
    [Header("WarningLight")]
        [SerializeField] private Image warningFill;

    void Start()
    {
        currentNitroLevel = maxNitro;
        totalLapsText.text = "/" + (lapsToFinish);
        color = currentLapTimeText.color;
        currentLapTimeText.enabled = false;
        keyCounter = 0;
        totalKeyText.text = "/" + (totalKeys);
        warningFill.enabled = false;
    }

    void Update()
    {
        if (currentNitroLevel <= 0)
            NitroDepleted = true;

        UpdateLapTime(); 
        if (blinkTextFlag)
        {
            ShowLapTime();
        }
    }

    public void ConsumeNitroBar(float consumeFactor)
    {
        currentNitroLevel -= consumeFactor * Time.deltaTime;
        nitroFill.fillAmount = currentNitroLevel / maxNitro;
    }


    public void UpdateSpeedometer(float speedometer)
    {
        speedLabel.text = ((int)speedometer) + "";
    }


    public void UpdateLapTime()
    {
        time += Time.deltaTime;
        timeLap += Time.deltaTime;
        minutes = (int) (time / 60f);
        seconds = (int) (time - minutes * 60f);
        cents = (int) ((time - (int) time) * 100f);
        timeLabel.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, cents);
    }

    public void UpdateLapCounter(int laps)
    {
        lapCounterText.text = (laps) + "";
    }


    public void ShowLapTime()
    {
        timeChecker +=Time.deltaTime;
        if(timeChecker < blinkFadeInTime)
        {
            currentLapTimeText.color = new Color (color.r, color.g, color.b, timeChecker / blinkFadeInTime);
        }
        else if (timeChecker < blinkFadeInTime + blinkStayTime)
        {
            currentLapTimeText.color = new Color (color.r, color.g, color.b, 1);
        }
        else if (timeChecker < blinkFadeInTime + blinkStayTime + blinkFadeOutTime)
        {
            currentLapTimeText.color = new Color (color.r, color.g, color.b, 1 - (timeChecker - (blinkFadeInTime + blinkStayTime)) / blinkFadeOutTime);
        }

        StartCoroutine(BlinkText());

    }

    IEnumerator BlinkText()
    {
        yield return new WaitForSeconds(3);
        currentLapTimeText.enabled = false;
        timeChecker =0;
        blinkTextFlag = false;  
        timeLap = 0;
    }

    public void ShowLap()
    {
         float minAux, secAux, centAux;

        minAux = (int) (timeLap / 60f);
        secAux = (int) (timeLap - minAux * 60f);
        centAux = (int) ((timeLap - (int) timeLap) * 100f);
        currentLapTimeText.enabled = true;
        blinkTextFlag = true;
        currentLapTimeText.text = string.Format("{0:00}:{1:00}:{2:00}", minAux, secAux, centAux);
    }

    public void AddKey()
    {
        keyCounter +=1;
        keyCounterText.text = (keyCounter) + "";
    }  

    public bool KeysComplete()
    {
        return keyCounter == totalKeys;
    }

    public void WarningLight(bool flag)
    {
        if (flag)  warningFill.enabled = true;
        else warningFill.enabled = false;
    }
 
}
