using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugReturnMaterials : MonoBehaviour
{
    public Inventory inventory; // อ้างอิงถึง Inventory
    public ResultManager resultManager;

    public void ReturnAllMaterialsToInventory()
    {
        foreach (CookRecipe recipe in inventory.recipes)
        {
            foreach (var recipeItem in recipe.recipeItems)
            {
                ReturnUsedMaterials(recipeItem.ingredient, recipeItem.quantity);
            }
        }
    }

    public void ReturnUsedMaterials(DataItem item, int quantity)
    {
        Debug.Log($"Returning {quantity}x {item.name} to main inventory.");

        // เพิ่มไอเทมเข้าไปใน Inventory
        inventory.AddItem(item, quantity);
    }

    public void ClearDebt()
    {
        resultManager.DebugPayDebt(10000);
        //GameDataManager.Instance.SaveDebt();
        Debug.Log("Debt cleared for debugging purposes.");

        // ตรวจสอบสถานะการชนะเกม
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.CheckWinCondition();
        }
    }
}
