using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemOnDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform originalParent;
    public Transform lastParent;
    public Inventory_SO myBagData;
    public GameObject bagGrid;
    private int currentItemID;

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        // lastParent = bagGrid.transform.GetChild(transform.childCount - 1);
        currentItemID = originalParent.GetComponent<SlotItem>().slotID;
        transform.SetParent(transform.parent.parent);
        transform.position = eventData.position;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        // Debug.Log(eventData.pointerCurrentRaycast.gameObject.name);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            if (eventData.pointerCurrentRaycast.gameObject.name == "Item Image")
            {
                transform.SetParent(eventData.pointerCurrentRaycast.gameObject.transform.parent.parent);
                transform.position = eventData.pointerCurrentRaycast.gameObject.transform.position;

                var temp = myBagData.itemList[currentItemID];
                myBagData.itemList[currentItemID] = myBagData.itemList[eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<SlotItem>().slotID];
                myBagData.itemList[eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<SlotItem>().slotID] = temp;

                eventData.pointerCurrentRaycast.gameObject.transform.parent.position = originalParent.position;
                eventData.pointerCurrentRaycast.gameObject.transform.parent.SetParent(originalParent);
                GetComponent<CanvasGroup>().blocksRaycasts = true;
                return;
            }
            // if(eventData.pointerCurrentRaycast.gameObject.name == "Grid")

            // transform.SetParent(lastParent);
            // transform.position = eventData.pointerCurrentRaycast.gameObject.transform.position;
            // lastParent.SetParent(originalParent);
            // lastParent.position = originalParent.position;
            // GetComponent<CanvasGroup>().blocksRaycasts = true;

            // var temp = myBagData.itemList[currentItemID];
            // myBagData.itemList[currentItemID] = myBagData.itemList[eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<SlotItem>().slotID];
            // myBagData.itemList[eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<SlotItem>().slotID] = temp;

            // eventData.pointerCurrentRaycast.gameObject.transform.parent.position = originalParent.position;
            // eventData.pointerCurrentRaycast.gameObject.transform.parent.SetParent(originalParent);
            // GetComponent<CanvasGroup>().blocksRaycasts = true;
        }

        //其它任何位置
        transform.SetParent(originalParent);
        transform.position = originalParent.position;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

}
