using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType
{
    KEY,
    NITRO

}

public class Item : MonoBehaviour
{

    private HUDManager hudManager;
    public ItemType itemType;

    private void Start()
    {
        hudManager = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDManager>();
    }

   private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {

            switch (itemType)
            {
                case ItemType.KEY:
                    Debug.Log("Key collected");
                    hudManager.AddKey();
                    break;
                case ItemType.NITRO:
                    Debug.Log("Nitro");

                    break;
                default:
                    break;
            }

            SelfDestroy();
        }
    }

    public void SelfDestroy()

    {
        Destroy(gameObject);
    }

}
