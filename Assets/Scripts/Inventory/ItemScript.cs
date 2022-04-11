using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemScript : MonoBehaviour
{
    [Header("Set by InventoryModule")]
    public InventoryModule inventoryModule;
    public int quantity = 0;
    public Transform playerTransform;

    [Header("Constant objects")] // their properties might change, but they're always the same object
    [SerializeField] GameObject ItemObject;
    [SerializeField] GameObject ItemOutline;
    [SerializeField] GameObject ItemCanvas;
    [SerializeField] Text ItemText;

    [Header("Constants")]
    [SerializeField] float PickUpDistance = 1;
    [SerializeField] KeyCode PickUpKey = KeyCode.E;

    //==== Quality of Life functions ====\\
    bool IsItemNearFeet()
    {
        Vector2 feetPosition = playerTransform.position;
        float distanceFromFeet = Vector2.Distance(ItemObject.transform.position, feetPosition);

        if (distanceFromFeet <= PickUpDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void PickItemUp()
    {
        inventoryModule.AddItemToInventory(ItemObject.name, quantity);
        Destroy(ItemObject);
    }

    //==== Unity functions ====\\
    void Start()
    {
        ItemText.text = quantity.ToString();
    }

    void Update()
    {
        if (IsItemNearFeet())
        {
            ItemOutline.SetActive(true);

            if (Input.GetKeyUp(PickUpKey))
            {
                PickItemUp();
            }
        }
        else
        {
            ItemOutline.SetActive(false);
        }
    }

    //==== Event functions ====\\
    void OnMouseEnter()
    {
        ItemCanvas.SetActive(true);
    }

    void OnMouseExit()
    {
        ItemCanvas.SetActive(false);
    }

    void OnMouseDown()
    {
        if (IsItemNearFeet())
        {
            PickItemUp();
        }
    }
}
