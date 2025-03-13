using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookingInventory : Inventory
{
    [Header("Cooking")]
    public Inventory mainInventory; 
    public int cookSlotAmount = 7; 
    public OutputSlot outputSlot; 
    public Transform cookInventoryPanel; 
    public OutputSlot[] outputSlots; 

    //[Header("Cooking Recipes")]
    //public CookRecipe[] recipes;
    //public Text requiredIngredientsTextUI;

    void Start()
    {
        Debug.Log("CookingInventory Started");

        // ��Ǩ�ͺ��� mainInventory ��� outputSlot 
        if (mainInventory == null || outputSlot == null)
        {
            Debug.LogError("Required references not set in CookingInventory!"); 
        }


       
        iteminventorySlots = mainInventory.iteminventorySlots;
        iteminventorySlots = mainInventory.iteminventorySlots;
        CreateCookSlots();

    }

    #region Inventory Methods

    public void CreateCookSlots()
    {
        outputSlots = new OutputSlot[cookSlotAmount]; 
        for (int i = 0; i < cookSlotAmount; i++)
        {
            Transform slot = Instantiate(outputSlot.transform, cookInventoryPanel); 
            OutputSlot invSlot = slot.GetComponent<OutputSlot>(); 

            outputSlots[i] = invSlot; 
            invSlot.inventory = mainInventory; 
            invSlot.cookingInventory = this; 
            invSlot.SetThisSlot(EMTRY_ITEM, 0); 
        }
    }


  



    public void UseSpecificCookMaterials(CookRecipe recipe, OutputSlot clickedSlot)
    {
        Debug.Log($"Using materials for recipe: {recipe.name}");
        bool materialsUsed = false;

 
        foreach (var recipeItem in recipe.recipeItems)
        {
            foreach (InventorySlot slot in iteminventorySlots)
            {
                if (slot.item == recipeItem.ingredient && slot.stack >= recipeItem.quantity)
                {
                 
                    Debug.Log($"Using material: {slot.item.name}");
                    slot.UseItem(recipeItem.quantity);
                    materialsUsed = true;
                    break;
                }
            }
        }

        if (materialsUsed)
        {
          
            mainInventory.AddItem(recipe.outputItem, recipe.outputStack);
            Debug.Log($"Added {recipe.outputStack} of {recipe.outputItem.name} to inventory.");
         
            clickedSlot.ClearThisCookSlot();

          
            CheckAllCookRecipe();
        }
        else
        {
            Debug.LogError("Not enough materials to use this recipe.");
        }
    }

    // �ѧ��ѹ�����㹡�����ѵ�شԺ�ҡ�Թ�ǹ�������ͷ������
    //public void UseCookMaterials()
    //{
    //    Debug.Log("Using cooking materials...");
    //    bool materialsUsed = false;

    //    // �ٻ��ҹ�ٵ÷��������͵�Ǩ�ͺ������ѵ�شԺ�ú�������
    //    foreach (CookRecipe recipe in recipes)
    //    {
    //        // ��Ǩ�ͺ�ٵ�������ѵ�شԺ�ú�������
    //        if (IsRecipeMatch(recipe))
    //        {
    //            Debug.Log($"Recipe match found: {recipe.name}");

    //            // �ٻ��ҹ�ѵ�شԺ��ٵ�
    //            foreach (var recipeItem in recipe.recipeItems)
    //            {
    //                // ���ѵ�شԺ���ç��Թ�ǹ����
    //                foreach (InventorySlot slot in iteminventorySlots)
    //                {
    //                    if (slot.item == recipeItem.ingredient && slot.stack >= recipeItem.quantity)
    //                    {
    //                        // �������ҡ�ٵ�
    //                        Debug.Log($"Using material: {slot.item.name}");
    //                        slot.UseItem(recipeItem.quantity); // ����������ӹǹ����ͧ���
    //                        mainInventory.AddItem(recipe.outputItem, recipe.outputStack); // ����������������Թ�ǹ������ѡ
    //                        materialsUsed = true;
    //                        break;
    //                    }
    //                }
    //            }

    //            // ���������ʴؤú����������ٵ÷�����
    //            CheckAllCookRecipe();
    //            return;
    //        }
    //    }

    //    // �����辺��ʴط������
    //    if (!materialsUsed)
    //        Debug.Log("No material used.");
    //}

    public bool IsRecipeMatch(CookRecipe recipe)
    {
        Dictionary<DataItem, int> availableIngredients = new Dictionary<DataItem, int>();

        foreach (InventorySlot slot in iteminventorySlots)
        {
            if (slot.item != null && slot.item != EMTRY_ITEM)
            {
                if (availableIngredients.ContainsKey(slot.item))
                {
                    availableIngredients[slot.item] += slot.stack;
                }
                else
                {
                    availableIngredients.Add(slot.item, slot.stack);
                }
            }
        }

        foreach (var recipeItem in recipe.recipeItems)
        {
            if (!availableIngredients.ContainsKey(recipeItem.ingredient) ||
                availableIngredients[recipeItem.ingredient] < recipeItem.quantity)
            {
                Debug.Log($"Missing ingredient: {recipeItem.ingredient.name} " +
                         $"(Need: {recipeItem.quantity}, " +
                         $"Have: {(availableIngredients.ContainsKey(recipeItem.ingredient) ? availableIngredients[recipeItem.ingredient] : 0)})");
                return false; 
            }
        }

        return true;
    }

    // �ѧ��ѹ������Ǩ�ͺ�ٵ÷������������ö����

    //public void DisplayRequiredIngredients(CookRecipe recipe)
    //{
    //    Debug.Log($"Displaying required ingredients for recipe: {recipe.name}");

    //    // ���ҧ��ͤ����ʴ��ѵ�شԺ������
    //    string requiredIngredientsText = $"Recipe: {recipe.name}\nRequired Ingredients:\n";

    //    foreach (var recipeItem in recipe.recipeItems)
    //    {
    //        requiredIngredientsText += $"{recipeItem.ingredient.name} x {recipeItem.quantity}\n";
    //    }

    //    // �ʴ���ͤ���� UI (���������� Text ���� requiredIngredientsTextUI)
    //    requiredIngredientsTextUI.text = requiredIngredientsText;
    //}

    public void CheckAllCookRecipe()
    {
        Debug.Log("Checking all recipes...");

        List<CookRecipe> validRecipes = new List<CookRecipe>(); 

        
        foreach (CookRecipe recipe in recipes)
        {
            if (IsRecipeMatch(recipe))
            {
                validRecipes.Add(recipe);
            }
        }

        // outputSlots
        if (validRecipes.Count > 0)
        {
            Debug.Log($"Found {validRecipes.Count} valid recipes!");

            
            foreach (OutputSlot slot in outputSlots)
            {
                slot.ClearThisCookSlot();
            }

            // outputSlots
            int index = 0;
            foreach (CookRecipe validRecipe in validRecipes)
            {
                if (index < outputSlots.Length)
                {
                    outputSlots[index].SetThisSlot(validRecipe.outputItem, validRecipe.outputStack); 
                    Debug.Log($"Displaying recipe: {validRecipe.name} in slot {index + 1}");
                    index++;
                }
            }
        }
        else
        {
            Debug.Log("No valid recipes found. Clearing all output slots...");
  
            foreach (OutputSlot slot in outputSlots)
            {
                slot.ClearThisCookSlot();
            }
        }
    }


    public void ReturnAllToMainInventory()
    {
        Debug.Log("Returning all items to main inventory");
        foreach (InventorySlot slot in iteminventorySlots)
        {
            if (slot.item != null && slot.item != EMTRY_ITEM)
            {
                mainInventory.AddItem(slot.item, slot.stack); 
                slot.ClearThisCookSlot(); 
            }
        }
    }
    #endregion






}

