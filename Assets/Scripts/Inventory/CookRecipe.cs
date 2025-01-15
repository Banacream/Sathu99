using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new Recepi", menuName = "Cooking/Create New Recipe", order = 0)]
public class CookRecipe : ScriptableObject
{
    //[Header("Recipe")]
    //public DataItem[] recipeItems = new DataItem[0];
    [Header("Output Item")]
    public DataItem outputItem;
    public int outputStack = 1;

    //[Header("Recipe")]
    [System.Serializable]
    public struct RecipeIngredient
    {
        public DataItem ingredient; // วัตถุดิบที่ใช้
        public int quantity; // จำนวนที่ต้องการ

        public RecipeIngredient(DataItem ingredient, int quantity)
        {
            this.ingredient = ingredient;
            this.quantity = quantity;
        }
    }

    [SerializeField] public RecipeIngredient[] recipeItems = new RecipeIngredient[0]; // รายการวัตถุดิบที่ต้องใช้

    // จำนวนวัตถุดิบทั้งหมดในสูตร
    public int NumberOfIngredients => recipeItems.Length;

    // ดึงข้อมูลวัตถุดิบจากตำแหน่งที่กำหนด
    public DataItem GetIngredientAt(int index)
    {
        if (index >= recipeItems.Length)
        {
            Debug.LogError("There is no ingredient at that index. Recipe: " + name);
            return null;
        }

        return recipeItems[index].ingredient;
    }

    // ดึงจำนวนที่ต้องการของวัตถุดิบจากตำแหน่งที่กำหนด
    public int GetQuantityAt(int index)
    {
        if (index >= recipeItems.Length)
        {
            Debug.LogError("There is no ingredient at that index. Recipe: " + name);
            return -1;
        }

        return recipeItems[index].quantity;
    }

    // เพิ่มวัตถุดิบใหม่ลงในสูตร
    public void AddIngredient(DataItem ingredient, int quantity)
    {
        RecipeIngredient[] copy = new RecipeIngredient[recipeItems.Length + 1];
        recipeItems.CopyTo(copy, 0);

        copy[recipeItems.Length] = new RecipeIngredient(ingredient, quantity);

        recipeItems = copy;
    }



}
