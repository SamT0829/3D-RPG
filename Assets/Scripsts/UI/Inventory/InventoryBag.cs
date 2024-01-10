using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryBag : MonoBehaviour, IDragHandler
{
    private Dictionary<string, SlotItem> slotItemDic = new Dictionary<string, SlotItem>();
    public GameObject emptySlot;
    public Inventory_SO inventory;
    public GameObject slotGrid;
    // public SlotItem slotPrefab;
    public Text ItemInformation;

    //移動InventoryBag 參數
    RectTransform currentRect;

    private void Awake()
    {        
        currentRect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        LoadInventoryItem();
        ItemInformation.text = "";
    }

    public void UpdateItemInfo(string itemDescription)
    {
        ItemInformation.text = itemDescription;
    }

    public void AddNewItem(Item_SO item_so)
    {
        SlotItem item;
        if (!slotItemDic.TryGetValue(item_so.name, out item))
        {
            GameObject slot = Instantiate(emptySlot);
            slot.transform.SetParent(slotGrid.transform);

            item = slot.GetComponent<SlotItem>();
            item.SetupSlot(item_so);

            slotItemDic.Add(item.slotItem.name, item);
            inventory.itemList.Add(item_so);
            item.slotID = inventory.itemList.Count;
        }
        else
        {
            item_so.itemHeld++;
            item.slotNum.text = item_so.itemHeld.ToString();
        }
    }

    public void LoadInventoryItem()
    {
        for (int i = 0; i < inventory.itemList.Count; i++)
        {
            GameObject slot = Instantiate(emptySlot);
            slot.transform.SetParent(slotGrid.transform);

            SlotItem item = slot.GetComponent<SlotItem>();
            item.SetupSlot(inventory.itemList[i]);
            item.slotID = i;

            slotItemDic.Add(item.slotItem.name, item);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        currentRect.anchoredPosition += eventData.delta;
    }
}
