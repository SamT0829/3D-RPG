using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/New Inventory")]
public class Inventory_SO : ScriptableObject
{
    public List<Item_SO> itemList = new List<Item_SO>();
}
