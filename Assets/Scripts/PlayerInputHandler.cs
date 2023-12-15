using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerInputHandler : MonoBehaviour
{
    PlayerInputManager pim;
    [SerializeField] List<GameObject> players;
    int playerIndex = 0;
    InputDevice device;
    VehicleControls vehicleControls;
    InputAction joinAction;
    bool player2Selected;

    // Start is called before the first frame update
    void Start()
    {
        vehicleControls = new VehicleControls();
        vehicleControls.Enable();
        pim = GetComponent<PlayerInputManager>();
        pim.EnableJoining();
        InputUser.listenForUnpairedDeviceActivity = players.Count;

        //    joinAction = new InputAction();
        //    joinAction.started += OnJoinPressed;

        if (PlayerPrefs.GetString("GameMode").Equals("2Players"))
        {
            //for (int i = 0; i < 2; i++)
            //{
            //    PlayerVehicle selection = PlayerPrefs.GetString($"player{i + 1}") switch
            //    {
            //        "Cube" => PlayerVehicle.Car,
            //        "Sphere" => PlayerVehicle.Motorcycle,
            //        _ => throw new PlayerPrefsException("Data not recognized for P1")
            //    };

            //    pim.playerPrefab = players[(int)selection];
            //    pim.JoinPlayer(i, -1, null, /*InputSystem.devices[2]*/ pairWithDevice: null);
            //}


            // Code for testing only
            PlayerVehicle selection1P = PlayerPrefs.GetString("player1") switch
            {
                "Cube" => PlayerVehicle.Car,
                "Sphere" => PlayerVehicle.Motorcycle,
                _ => throw new PlayerPrefsException("Data not recognized for P1")
            };

            PlayerVehicle selection2P = PlayerPrefs.GetString("player2") switch
            {
                "Cube2" => PlayerVehicle.Car,
                "Sphere2" => PlayerVehicle.Motorcycle,
                _ => throw new PlayerPrefsException("Data not recognized for P2")
            };

            players[(int)selection1P].GetComponentInChildren<Camera>().rect = new Rect(0f, 0.5f, 1f, 1f);
            pim.playerPrefab = players[(int)selection1P];
            pim.JoinPlayer(0, -1, null, /*InputSystem.devices[2]*/ pairWithDevice: null);

            pim.playerPrefab = players[(int)selection2P];
            pim.JoinPlayer(1, -1, null, /*InputSystem.devices[2]*/ pairWithDevice: null);
        }
    }

    //void OnJoinPressed(InputAction.CallbackContext context)
    //{
    //    JoinPlayer(context.control.device);
    //}

    //void JoinPlayer(InputDevice device)
    //{
    //    this.device = device;
    //}

    // Update is called once per frame
    void Update()
    {

        //if (playerIndex < 2)
        //{


        //    //var user = InputUser.CreateUserWithoutPairedDevices();
        //    //user.AssociateActionsWithUser(vehicleControls);
        //    //user.ActivateControlScheme("xbox");
        //    //user.ActivateControlScheme("switch");
        //    //var gamepads = Gamepad.all;

        //    //foreach (var gamepad in gamepads)
        //    //{
        //    //    Debug.Log("Gamepad Display Name " + gamepad.displayName);
        //    //    Debug.Log("Gamepad Device " + gamepad.device);
        //    //    Debug.Log("Gamepad Name " + gamepad.name);
        //    //    Debug.Log("Gamepad All controls array " + gamepad.allControls.ToArray());
        //    //}

        //    Debug.Log(PlayerInputManager.instance.name);
        //    //Debug.Log("Device Enabled? " + device.enabled);

        //    //var deviceArray = device.allControls;

        //    //foreach (var c in deviceArray)
        //    //{
        //    //    Debug.Log("Device: " + c.device);
        //    //    Debug.Log("Display Name: " + c.displayName);
        //    //    Debug.Log("Aliases: " + c.aliases);
        //    //    Debug.Log("Name: " + c.name);
        //    //    Debug.Log("Path: " + c.path);
        //    //    Debug.Log("Short Display Name: " + c.shortDisplayName);
        //    //    Debug.Log("Usages: " + c.usages);
        //    //    Debug.Log("-------------------------------------------");
        //    //}

        //    Debug.Log("Out of loop with Device");

        //    PlayerVehicle selection = PlayerPrefs.GetString("player1") switch
        //    {
        //        "Cube" => PlayerVehicle.Car,
        //        "Sphere" => PlayerVehicle.Motorcycle,
        //        _ => throw new PlayerPrefsException("Data not recognized")
        //    };


        //    pim.playerPrefab = players[playerIndex];
        //    switch (playerIndex)
        //    {
        //        case 0:
        //            pim.JoinPlayer(playerIndex, -1, null, /*InputSystem.devices[2]*/ pairWithDevice: null); break;
        //        case 1:
        //            pim.JoinPlayer(playerIndex, -1, null, /*InputSystem.devices[0]*/ pairWithDevice: null); break;
        //        default:
        //            break;
        //    }
        //    //pim.JoinPlayer(playerIndex, , null, );
        //    //pim.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;
        //    playerIndex++;
        //}


    }
}