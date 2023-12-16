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

    bool p1Ready = true;
    bool p2Ready = true;


    
    // Start is called before the first frame update
    void Start()
    {
        if ((SceneManager.GetActiveScene().name.Equals("JR_VehicleSelection") || 
            SceneManager.GetActiveScene().name.Equals("TubularTrack") ||
            SceneManager.GetActiveScene().name.Equals("DesertTerrainTests") ||
            SceneManager.GetActiveScene().name.Equals("MotorcycleSetup")) &&
            PlayerPrefs.GetString("GameMode").Equals("2Players"))
        {
            p1Ready = false;
            p2Ready = false;
            OnGameModeSelected?.Invoke();
        }
        else
        {
            p1Ready = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    /// For Main Menu to select the number of players.
    /// 
    public void SelectNumberOfPlayers(TextMeshProUGUI mode)
    {
        PlayerPrefs.SetString("GameMode", mode.text.Replace(" ", ""));
        if (debug) Debug.Log($"Game Mode selected: {mode.text.Replace(" ", "")}");
        SceneManager.LoadScene("JR_VehicleSelection");
    }

    /// <summary>
    /// For Vehicle Selection scene where player selects their vehicle (car, motorcycle)
    /// </summary>
    /// <param name="prefab"></param>
    public void Player1Selection(GameObject prefab)
    {
        p1Ready = true;
        PlayerPrefs.SetString("player1", prefab.name);
        if (debug) Debug.Log($"Player1 stored: {prefab.name}");
    }

    public void Player2Selection(GameObject prefab)
    {
        p2Ready = true;
        PlayerPrefs.SetString("player2", prefab.name);
        if (debug) Debug.Log($"Player2 stored: {prefab.name}");
    }

    public void SubmitPlayersSelection()
    {
        if (p1Ready && p2Ready)
        {
            SceneManager.LoadScene("JR_StageSelection");
        }
    }


    // From Stage Selection Scene
    public void SelectedDesertStage()
    {
        SceneManager.LoadScene("MotorcycleSetup");
    }

    public void SelectedTubularStage()
    {
        SceneManager.LoadScene("MotorcycleSetup");
    }

}
