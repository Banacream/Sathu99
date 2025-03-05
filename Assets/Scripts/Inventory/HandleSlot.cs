using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class HandleSlot : InventorySlot
{

       private HandleSlotSync slotSync;

    void Start()
    {
        Debug.Log("HandleSlot Started");
        slotSync = FindObjectOfType<HandleSlotSync>();
        if (slotSync == null)
        {
            Debug.LogError("HandleSlotSync not found in scene!");
        }
        CheckShowText();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if(SceneManager.GetActiveScene().name != "Result")
        Inventory.Instance.HandToInventory(slotType);
    }

    public override void SetThisSlot(DataItem newItem, int amount)
    {

        if (newItem == null)
        {
            Debug.LogError("SetThisSlot: newItem is null!"); 
            return;
        }


        item = newItem;
        icon.sprite = newItem.icon;

    
        int itemAmount = amount;
        int intInthisSlot = Mathf.Clamp(itemAmount, 0, newItem.maxStack); 
        stack = intInthisSlot; 
        Debug.Log($"Handle slot set with item: {newItem.name}, amount: {stack}");

        CheckShowText();

        if (slotSync != null)
        {
            slotSync.SyncSlots(this, newItem, amount);
        }

    }


}
