using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookingInventory : Inventory
{
    [Header("Cooking")]
    public Inventory mainInventory; // �Թ�ǹ������ѡ������ѵ�شԺ
    public int cookSlotAmount = 30; // �ӹǹ��ͧ��Թ�ǹ���ա�÷������
    public OutputSlot outputSlot; // ��ͧ�ʴ����Ѿ���÷������
    public Transform cookInventoryPanel; // ἧ�Թ�ǹ���ա�÷������
    public OutputSlot[] outputSlots; // ��ͧ����Ѻ�ʴ��ٵ÷�����

    [Header("Cooking Recipes")]
    public CookRecipe[] recipes; // �ٵ�����÷������ö����
    //public Text requiredIngredientsTextUI;

    void Start()
    {
        Debug.Log("CookingInventory Started");

        // ��Ǩ�ͺ��� mainInventory ��� outputSlot �١��駤���������
        if (mainInventory == null || outputSlot == null)
        {
            Debug.LogError("Required references not set in CookingInventory!"); // ����͹�ҡ����ա�õ�駤��
        }


        // �� slot �ҡ Inventory ��ѡ
        iteminventorySlots = mainInventory.iteminventorySlots;
        CreateCookSlots(); // ���ҧ��ͧ����Ѻ�红ͧ㹡�÷������

    }

    #region Inventory Methods
    // �ѧ��ѹ�����㹡�����ҧ��ͧ����Ѻ�红ͧ㹡�÷������
    public void CreateCookSlots()
    {
        outputSlots = new OutputSlot[cookSlotAmount]; // ���ҧ������������Ѻ�纪�ͧ
        for (int i = 0; i < cookSlotAmount; i++)
        {
            Transform slot = Instantiate(outputSlot.transform, cookInventoryPanel); // ���ҧ��ͧ����� UI
            OutputSlot invSlot = slot.GetComponent<OutputSlot>(); // �֧ Component �ͧ OutputSlot

            outputSlots[i] = invSlot; // �纪�ͧ������ҧ���������
            invSlot.inventory = mainInventory; // ��駤�ҡ��������§�Ѻ�Թ�ǹ������ѡ
            invSlot.cookingInventory = this; // ��駤�ҡ��������§�Ѻ�Թ�ǹ���ա�÷������
            invSlot.SetThisSlot(EMTRY_ITEM, 0); // ��駤�Ҫ�ͧ��������繪�ͧ��ҧ
        }
    }

    // �ѧ��ѹ�����㹡�����ѵ�شԺ�ҡ�Թ�ǹ�������ͷ������
    public void UseCookMaterials()
    {
        Debug.Log("Using cooking materials...");
        bool materialsUsed = false;

        // �ٻ��ҹ�ٵ÷��������͵�Ǩ�ͺ������ѵ�شԺ�ú�������
        foreach (CookRecipe recipe in recipes)
        {
            // ��Ǩ�ͺ�ٵ�������ѵ�شԺ�ú�������
            if (IsRecipeMatch(recipe))
            {
                Debug.Log($"Recipe match found: {recipe.name}");

                // �ٻ��ҹ�ѵ�شԺ��ٵ�
                foreach (var recipeItem in recipe.recipeItems)
                {
                    // ���ѵ�شԺ���ç��Թ�ǹ����
                    foreach (InventorySlot slot in iteminventorySlots)
                    {
                        if (slot.item == recipeItem.ingredient && slot.stack >= recipeItem.quantity)
                        {
                            // �������ҡ�ٵ�
                            Debug.Log($"Using material: {slot.item.name}");
                            slot.UseItem(recipeItem.quantity); // ����������ӹǹ����ͧ���
                            mainInventory.AddItem(recipe.outputItem, recipe.outputStack); // ����������������Թ�ǹ������ѡ
                            materialsUsed = true;
                            break;
                        }
                    }
                }

                // ���������ʴؤú����������ٵ÷�����
                CheckAllCookRecipe();
                return;
            }
        }

        // �����辺��ʴط������
        if (!materialsUsed)
            Debug.Log("No material used.");
    }

    // �ѧ��ѹ������Ǩ�ͺ����ٵ�����ö�����������
    public bool IsRecipeMatch(CookRecipe recipe)
    {
        // ���ҧ Dictionary �����纨ӹǹ�ѵ�شԺ�����
        Dictionary<DataItem, int> availableIngredients = new Dictionary<DataItem, int>();

        // �Ǻ����ѵ�شԺ�����㹪�ͧ inventory
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

        // ��Ǩ�ͺ������ѵ�شԺ�ú����ٵ��������
        foreach (var recipeItem in recipe.recipeItems)
        {
            if (!availableIngredients.ContainsKey(recipeItem.ingredient) ||
                availableIngredients[recipeItem.ingredient] < recipeItem.quantity)
            {
                Debug.Log($"Missing ingredient: {recipeItem.ingredient.name} " +
                         $"(Need: {recipeItem.quantity}, " +
                         $"Have: {(availableIngredients.ContainsKey(recipeItem.ingredient) ? availableIngredients[recipeItem.ingredient] : 0)})");
                return false; // ����ѵ�شԺ���ú���׹��� false
            }
        }

        return true; // ����ѵ�شԺ�ú����ٵ�
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

        List<CookRecipe> validRecipes = new List<CookRecipe>(); // ��¡���ٵ÷������ö����

        // ��Ǩ�ͺ�ء�ٵ�� recipes ������ѵ�شԺ�ú�������
        foreach (CookRecipe recipe in recipes)
        {
            if (IsRecipeMatch(recipe))
            {
                validRecipes.Add(recipe); // �����ٵ÷��������¡�� validRecipes
                //DisplayRequiredIngredients(recipe); // �ʴ��ѵ�شԺ����������Ѻ�ٵù��
            }
        }

        // �ҡ���ٵ÷���������ʴ�� outputSlots
        if (validRecipes.Count > 0)
        {
            Debug.Log($"Found {validRecipes.Count} valid recipes!");

            // �������ͧ��������͹
            foreach (OutputSlot slot in outputSlots)
            {
                slot.ClearThisCookSlot();
            }

            // �ʴ��ٵ÷������ outputSlots
            int index = 0;
            foreach (CookRecipe validRecipe in validRecipes)
            {
                if (index < outputSlots.Length)
                {
                    outputSlots[index].SetThisSlot(validRecipe.outputItem, validRecipe.outputStack); // ��駤�Ҽ��Ѿ��㹪�ͧ
                    Debug.Log($"Displaying recipe: {validRecipe.name} in slot {index + 1}");
                    index++;
                }
            }
        }
        else
        {
            Debug.Log("No valid recipes found. Clearing all output slots...");
            // �ҡ������ٵ÷����� ������ء��ͧ� outputSlots
            foreach (OutputSlot slot in outputSlots)
            {
                slot.ClearThisCookSlot();
            }
        }
    }

    // �ѧ��ѹ�����׹�ͧ��������Ѻ��ѧ�Թ�ǹ������ѡ
    public void ReturnAllToMainInventory()
    {
        Debug.Log("Returning all items to main inventory");
        foreach (InventorySlot slot in iteminventorySlots)
        {
            if (slot.item != null && slot.item != EMTRY_ITEM)
            {
                mainInventory.AddItem(slot.item, slot.stack); // ����������Ѻ��ѧ�Թ�ǹ������ѡ
                slot.ClearThisCookSlot(); // �������ͧ㹡�÷������
            }
        }
    }
    #endregion


    
    //// �ѧ��ѹ�����׹�ͧ�ҧ��¡�á�Ѻ��ѧ�Թ�ǹ������ѡ
    //public void ReturnToMainInventory(DataItem item, int amount)
    //{
    //    Debug.Log($"Returning {amount} {item.name} to main inventory");
    //    mainInventory.AddItem(item, amount); // ����������Ѻ��ѧ�Թ�ǹ������ѡ
    //}

    //// �ѧ��ѹ���ж١���¡������ѵ�شԺ�������١�׹��Ѻ��ѧ�Թ�ǹ������ѡ
    //private void OnDisable()
    //{
    //    ReturnAllToMainInventory(); // �׹�ͧ������������Թ�ǹ���ա�÷�����ö١�Դ
    //}



}

