using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static DataItem;

public class SellManager : MonoBehaviour
{
    public Inventory mainInventory; 
    public ResultManager resultManager;
    //public Image icon; 

    public void SellAllFoodItems()
    {

      
        int totalAmount = 0; 

       
        foreach (InventorySlot slot in mainInventory.cookSellSlots)
        {
            if (slot.item != null && slot.item.itemType == ItemType.Food) 
            {
                int sellPrice = slot.item.sellPrice * slot.stack; 
                Debug.Log($"Selling {slot.stack} of {slot.item.name} for {sellPrice} coins.");

                totalAmount += sellPrice; 
                slot.ClearThisCookSlot(); 
                resultManager.CheckPriceFood();
            }
        }

       
        if (totalAmount > 0)
        {
            Debug.Log($"Total coins earned: {totalAmount}");
            resultManager.AddCoin(totalAmount);
        }
        else
        {
            Debug.Log("No food items to sell.");
        }
    }

}
