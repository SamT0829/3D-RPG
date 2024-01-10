using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOnWorld : MonoBehaviour
{
    public Item_SO thisItem;
    public Inventory_SO playerInventory;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameMenuUI.Instance.Inventory.AddNewItem(thisItem);
            Destroy(gameObject);
        }
    }

    // private void AddNewItem()
    // {
    //     if (!playerInventory.itemList.Contains(thisItem))
    //     {
    //         for (int i = 0; i < playerInventory.itemList.Count; i++)
    //         {
    //             if (playerInventory.itemList[i] == null)
    //             {
    //                 playerInventory.itemList[i]= thisItem;
    //                 break;
    //             }
    //         }
    //     }
    //     else {
    //         thisItem.itemHeld += 1;
    //     }

    // }
}
