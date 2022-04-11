using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    [SerializeField] GameObject ItemObject;
    KeyCode PickUpKey = KeyCode.E;
    public InventoryModule inventoryModule;
    public int quantity = 1;

    void Update()
    {
        if (Input.GetKeyUp(PickUpKey))
        {
            inventoryModule.AddItemToInventory(ItemObject.name, quantity);
            Destroy(ItemObject);
        }
    }
}
