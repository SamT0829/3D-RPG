using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotItem : MonoBehaviour
{
    public Item_SO slotItem;
    public Image slotImage;
    public Text slotNum;
    public GameObject itemInSlot;
    public int slotID;
    public string slotInfo;
    private Button button;
    
    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {

    }

    public void ItemOnClicked()
    {
        GameMenuUI.Instance.Inventory.UpdateItemInfo(slotInfo);
    }

    public void SetupSlot(Item_SO item_SO)
    {   
        if (item_SO == null)
        {
            itemInSlot.SetActive(false);
            return;
        }

        itemInSlot.SetActive(true);
        slotItem = item_SO;
        slotImage.sprite = item_SO.itemImage;
        slotNum.text = item_SO.itemHeld.ToString();
        slotInfo = item_SO.itemInfo;
    }
}
