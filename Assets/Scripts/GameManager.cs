using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool debug = true;

    public static Action OnGameModeSelected;

    // Start is called before the first frame update
    void Start()
    {
        if ((SceneManager.GetActiveScene().name.Equals("JR_VehicleSelection") || SceneManager.GetActiveScene().name.Equals("MotorcycleSetup")) &&
            PlayerPrefs.GetString("GameMode").Equals("2Players"))
        {
            OnGameModeSelected?.Invoke();
        }
    }

    // Update is called once per frame
    void Update()
    {


    }

    /// <summary>
    /// For Vehicle Selection 
    /// </summary>
    /// <param name="prefab"></param>
    public void Player1Selection(GameObject prefab)
    {
        PlayerPrefs.SetString("player1", prefab.name);
        if (debug) Debug.Log($"Player1 stored: {prefab.name}");
    }

    public void Player2Selection(GameObject prefab)
    {
        PlayerPrefs.SetString("player2", prefab.name);
        if (debug) Debug.Log($"Player2 stored: {prefab.name}");
    }

    /// For Main Menu
    /// 
    public void SelectNumberOfPlayers(TextMeshProUGUI mode)
    {
        PlayerPrefs.SetString("GameMode", mode.text.Replace(" ", ""));
        if (debug) Debug.Log($"Game Mode selected: {mode.text.Replace(" ", "")}");
        SceneManager.LoadScene("JR_VehicleSelection");
    }

}
