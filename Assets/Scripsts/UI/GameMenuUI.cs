using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuUI : Singleton<GameMenuUI>
{
    [Header("背包UI")]
    public InventoryBag Inventory;
    bool isOpen;
  

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            OpenMyInventory();
        }
    }

    public void OpenMyInventory()
    {
        isOpen = !isOpen;
        Inventory.gameObject.SetActive(isOpen);
    }

}
