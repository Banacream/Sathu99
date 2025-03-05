using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    [Header("Inventory Detail")]
    public Inventory inventory; 

    [Header("Slot Detail")]
    public DataItem item; 
    public int stack; 
    

    [Header("Cook Inventory")]
    public CookingInventory cookingInventory; 

    [Header("UI")]
    public Image icon; 
    public TextMeshProUGUI stackText; 
    public InventoryType slotType;
    public Color emptyColor;
    public Color itemColor;
    //public TextMeshProUGUI weightText; 

    public int slotIndex;

    public enum InventoryType
    {
        Item ,
        Tool ,
        Food
    }

    void Start()
    {
   
    }

    public void SetSlotType(InventoryType type)
    {
        slotType = type;  
    }


 
    public virtual void OnPointerClick(PointerEventData eventData)
    {
      
        if (eventData.button == PointerEventData.InputButton.Left)
        {
           
            if (item == inventory.EMTRY_ITEM)
                return;
            if (SceneManager.GetActiveScene().name != "Result")
            { 
                 Inventory.Instance.InventoryToHand(slotIndex, slotType);
            }

            if (item is DataItem dataItem && dataItem.itemType == DataItem.ItemType.Food)
            {


                UseItem(1);
                foreach (CookRecipe recipe in inventory.recipes)
                {
                    if (recipe.outputItem == dataItem)
                    {
                        DisplayRecipeDetails(recipe);

                        // คืนวัตถุดิบที่ใช้ทำอาหารกลับไปที่ Main Inventory
                        foreach (var recipeItem in recipe.recipeItems)
                        {
                            // เรียกคืนวัตถุดิบที่ใช้ โดยส่ง ingredient และ quantity ที่ใช้ในสูตร
                            ReturnUsedMaterials(recipeItem.ingredient, recipeItem.quantity);
                        }
                    }
                }



            }

        }

    }

    public void DisplayRecipeDetails(CookRecipe recipe)
    {
        if (recipe == null)
        {
            return;
        }

        // แสดงชื่อเมนู
        Debug.Log($"Menu: {recipe.outputItem.name} (Produces {recipe.outputStack})");

        // วนลูปแสดงวัตถุดิบที่ใช้ในสูตร
        Debug.Log("Ingredients:");
        foreach (var ingredient in recipe.recipeItems)
        {
            Debug.Log($"- {ingredient.ingredient.name}: {ingredient.quantity}");
        }
    }

    public void AssignIndex(int slotIndex)
    {
        this.slotIndex = slotIndex;
    }


    public virtual void UseItem(int quantity)
    {
   
        if (quantity <= 0)
        {
            return;
        }


        stack = Mathf.Clamp(stack - quantity, 0, item.maxStack);


        if (stack > 0)
        {
            CheckShowText(); 
        }
        else
        {
           
            inventory.RemoveItem(this); 
        }
    }

    public void ReturnUsedMaterials(DataItem item, int quantity)
    {
        Debug.Log($"Returning {quantity}x {item.name} to main inventory.");

        // ขั้นตอนที่ 1: เช็คหาช่องที่มีไอเทมตรงกันและยังมีพื้นที่ว่าง
        foreach (InventorySlot slot in inventory.iteminventorySlots)
        {
            // ตรวจสอบว่าไอเทมตรงกันและยังไม่เต็ม
            if (slot.item == item && slot.stack < item.maxStack)
            {
                inventory.AddItem(item, quantity);
                return;
            }
            else
            {
                inventory.AddItem(item, quantity);
                return;
            }
        }
   
    }


    public virtual void SetThisSlot(DataItem newItem, int amount)
    {
        item = newItem; 
        icon.sprite = newItem.icon; 
        int itemAmount = amount;
        
        int intInthisSlot = Mathf.Clamp(itemAmount, 0, newItem.maxStack);
        stack = intInthisSlot;

        CheckShowText(); 

        int amountLeft = itemAmount - intInthisSlot;
       
        if (amountLeft > 0)
        {
            
            InventorySlot slot = inventory.IsEmptySlotLeft(newItem, this);
            if (slot == null) 
            {
                return;
            }
            else 
            {
                slot.SetThisSlot(newItem, amountLeft);
            }
        }
    }


    public void MergeThisSlot(DataItem mergeItem, int mergeAmount)
    {
        item = mergeItem; 
        icon.sprite = mergeItem.icon; 

        int itemAmount = stack + mergeAmount;
       
        int intInThisSlot = Mathf.Clamp(itemAmount, 0, item.maxStack);
        stack = intInThisSlot; 
        CheckShowText(); 

        int amountLeft = itemAmount - intInThisSlot;
      
        if (amountLeft > 0)
        {
  
            InventorySlot slot = inventory.IsEmptySlotLeft(mergeItem, this);
            if (slot == null) 
            {
                inventory.DropItem(mergeItem, amountLeft);
                return;
            }
            else 
            {
                slot.MergeThisSlot(mergeItem, amountLeft);
            }
        }
    }

 
    public void ClearThisCookSlot()
    {
        SetThisSlot(inventory.EMTRY_ITEM, 0); 
    }

  
    public void CheckShowText()
    {
        UpdateColorSlot();
        stackText.text = stack.ToString(); 

       
        if (item.maxStack < 2)
        {
            stackText.gameObject.SetActive(false);
        }
        else
        {
        
            if (stack > 1)
                stackText.gameObject.SetActive(true);
            else
                stackText.gameObject.SetActive(false);
        }
    }

    public void UpdateColorSlot()
    {
        if (item == inventory.EMTRY_ITEM)
            icon.color = emptyColor;
        else
            icon.color = itemColor;
    }
}
