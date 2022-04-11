using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Item
{
    public string Name;
    public int Quantity;
    public int Price;
    public GameObject UIObject;

    public Item(string name, int price)
    {
        Name = name;
        Price = price;
        Quantity = 1;
    }

    public Item(string name, int price, int quantity)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
    }

    public Item(string name, int price, int quantity, GameObject uiObject)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
        UIObject = uiObject;
    }

    public static List<Item> Inventory = new List<Item>();

    static Dictionary<string, Item> ItemDictionary = new Dictionary<string, Item>()
    {
        {"test_cube", new Item("test_cube", 5)}
    };

    public static InventoryModule inventoryModule;

    public static int AddToInventory(string itemName, int quantity = 1)
    {
        Item itemPrefab = ItemDictionary[itemName];
        int uniqueItemCount = Item.Inventory.Count;

        if (quantity > 0 && itemPrefab != null)
        {
            Item newItem = new Item(itemPrefab.Name, itemPrefab.Price, quantity);

            foreach (Item item in Inventory)
            {
                if (item.Name == newItem.Name)
                {
                    item.Quantity += newItem.Quantity;
                    return uniqueItemCount;
                }
            }
            
            newItem.UIObject = inventoryModule.ConvertItemToUIItem(newItem);
            Inventory.Add(newItem);
            
            return uniqueItemCount + 1;
        }

        return uniqueItemCount;
    }

    public static int RemoveFromInventory(Item itemToRemove, int quantity)
    {
        int uniqueItemCount = Item.Inventory.Count;

        foreach (Item item in Inventory)
        {
            if (item.Name == itemToRemove.Name)
            {
                if (item.Quantity - quantity > 0)
                {
                    item.Quantity -= quantity;
                    return uniqueItemCount;
                }
                else
                {
                    Inventory.Remove(item);
                    return uniqueItemCount - 1;
                }
            }
        }

        return uniqueItemCount;
    }
}

public class InventoryModule : MonoBehaviour, IPointerClickHandler
{
    // Constants or Constant Objects (their properties might change, but they're always the same object)
    [SerializeField] RectTransform Canvas;
    [SerializeField] RectTransform UIItemHolder;
    [SerializeField] Vector2 UIItemOffset = new Vector2(25, 25);
    [SerializeField] GameObject DropMenu;
    [SerializeField] InputField DropMenuInputField;
    [SerializeField] Slider DropMenuSlider;
    [SerializeField] Text DropMenuMaxValueText;
    [SerializeField] Vector2 DropMenuOffset = new Vector2(0, -10);
    [SerializeField] GameObject UIItemTemplate;
    RectTransform UItemTemplateTransform;
    [SerializeField] GameObject WorldItemTemplate;

    // Variables
    int uniqueItemCount = 0;
    Item selectedItem;
    GameObject selectedUIItem;

    //---- UI Calculation ----\\
    Vector2 CalculateItemPosition(float itemIndex, Vector2 itemTransformSize)
    {
        float x = UIItemOffset.x + (itemTransformSize.x / 2) 
            + (itemIndex % 5) * (itemTransformSize.x + UIItemOffset.x);
        float y = -UIItemOffset.y - (itemTransformSize.y / 2)
            - Mathf.Floor(itemIndex / 5) * (itemTransformSize.y + UIItemOffset.y);

        return new Vector2(x, y);
    }

    Vector2 CalculateHolderSize()
    {
        float size = UIItemOffset.y + UItemTemplateTransform.sizeDelta.y + UIItemOffset.y;

        if (uniqueItemCount > 1)
        {
            if (uniqueItemCount % 5 != 0)
            {
                size += (UItemTemplateTransform.sizeDelta.y + UIItemOffset.y) * Mathf.Floor(uniqueItemCount / 5);
            }
            else
            {
                size += (UItemTemplateTransform.sizeDelta.y + UIItemOffset.y) * (Mathf.Floor(uniqueItemCount / 5) - 1);
            }
        }

        return new Vector2(0, size);
    }

    //---- UI Manipulation ----\\
    void ShowUIItemQuantityIfMoreThanOne(RectTransform itemTransform, int quantity)
    {
        if (quantity > 1)
        {
            GameObject quantityText = itemTransform.Find("Quantity").gameObject;

            quantityText.SetActive(true);
            quantityText.GetComponent<Text>().text = quantity.ToString();
        }
    }

    void PlaceDropMenuUnderCursor(PointerEventData mouse)
    {
        Vector2 cursorPosition;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            Canvas, mouse.position, mouse.pressEventCamera, out cursorPosition))
            return;

        DropMenu.transform.localPosition = cursorPosition + DropMenuOffset;
        DropMenu.SetActive(true);
    }

    public GameObject ConvertItemToUIItem(Item item, int itemIndex = -1)
    {
        if (itemIndex == -1)
        {
            itemIndex = uniqueItemCount;
        }

        GameObject newItem = Instantiate(UIItemTemplate, UIItemHolder);
        item.UIObject = newItem;
        RectTransform newTransform = newItem.GetComponent<RectTransform>();

        newTransform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/" + item.Name);
        ShowUIItemQuantityIfMoreThanOne(newTransform, item.Quantity);
        newTransform.localPosition = CalculateItemPosition(itemIndex, newTransform.sizeDelta);
        
        return newItem;
    }

    //---- Inventory Manipulation ----\\
    Item GetItem(GameObject uiObject)
    {
        foreach (Item item in Item.Inventory)
        {
            if (item.UIObject == uiObject)
            {
                return item;
            }
        }
        
        return null;
    }

    void RefreshInventory()
    {
        foreach (Transform child in UIItemHolder.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < uniqueItemCount; i++)
        {
            ConvertItemToUIItem(Item.Inventory[i], i);
        }
    }

    public void AddItemToInventory(string itemName, int quantity = 1)
    {
        uniqueItemCount = Item.AddToInventory(itemName, quantity);

        RefreshInventory();
    }

    void MoveSelectedItemFromInventoryToWorld()
    {
        string name = selectedItem.Name;
        int quantity = int.Parse(DropMenuInputField.text);

        GameObject worldItem = Instantiate(WorldItemTemplate, this.transform.parent);
        worldItem.name = name;
        ItemScript worldItemScript = worldItem.GetComponent<ItemScript>();
        worldItemScript.inventoryModule = this;
        worldItemScript.quantity = quantity;
        SpriteRenderer worldItemSpriteRenderer = worldItem.GetComponent<SpriteRenderer>();
        worldItemSpriteRenderer.sprite = Resources.Load<Sprite>("Images/" + name);

        uniqueItemCount = Item.RemoveFromInventory(selectedItem, quantity);
        DropMenu.SetActive(false);

        RefreshInventory();
    }

    //---- Unity Functions ----\\
    void Awake()
    {
        UItemTemplateTransform = UIItemTemplate.GetComponent<RectTransform>();
        Item.inventoryModule = this;
    }

    void Start()
    {
        AddItemToInventory("test_cube", 6);
    }

    //---- Event Functions ----\\
    public void OnPointerClick(PointerEventData mouse)
    {
        // checks if the Drop Menu is not active, or is active but not clicked on
        if (!DropMenu.activeSelf || (DropMenu.activeSelf && !mouse.hovered.Contains(DropMenu)))
        {
            // checks if an item has been clicked on and adjusts the Drop Menu to it...
            foreach (GameObject element in mouse.hovered)
            {
                if (element.transform.parent == UIItemHolder)
                {
                    selectedItem = GetItem(element);
                    selectedUIItem = element;

                    PlaceDropMenuUnderCursor(mouse);
                    DropMenuSlider.maxValue = selectedItem.Quantity;
                    DropMenuMaxValueText.GetComponent<Text>().text = DropMenuSlider.maxValue.ToString();
                    return;
                }
            }

            // ...or makes it disappear when no item has been clicked on
            DropMenu.SetActive(false);
        }
    }

    public void OnInputChange(string value)
    {
        DropMenuSlider.value = int.Parse(value);
    }

    public void OnSliderChange(Single value)
    {
        DropMenuInputField.text = value.ToString();
    }

    public void OnDropButtonClick()
    {
        MoveSelectedItemFromInventoryToWorld();
    }
}
