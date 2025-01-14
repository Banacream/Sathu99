using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CookingInventory : Inventory
{
    [Header("Cooking")]
    public Inventory mainInventory; // อินเวนทอรีหลักที่เก็บวัตถุดิบ
    public int cookSlotAmount = 30; // จำนวนช่องในอินเวนทอรีการทำอาหาร
    public OutputSlot outputSlot; // ช่องแสดงผลลัพธ์การทำอาหาร
    public Transform cookInventoryPanel; // แผงอินเวนทอรีการทำอาหาร
    public OutputSlot[] outputSlots; // ช่องสำหรับแสดงสูตรที่ทำได้

    [Header("Cooking Recipes")]
    public CookRecipe[] recipes; // สูตรอาหารที่สามารถทำได้
    //public Text requiredIngredientsTextUI;

    void Start()
    {
        Debug.Log("CookingInventory Started");

        // ตรวจสอบว่า mainInventory และ outputSlot ถูกตั้งค่าหรือไม่
        if (mainInventory == null || outputSlot == null)
        {
            Debug.LogError("Required references not set in CookingInventory!"); // แจ้งเตือนหากไม่มีการตั้งค่า
        }


        // ใช้ slot จาก Inventory หลัก
        iteminventorySlots = mainInventory.iteminventorySlots;
        CreateCookSlots(); // สร้างช่องสำหรับเก็บของในการทำอาหาร

    }

    #region Inventory Methods
    // ฟังก์ชันนี้ใช้ในการสร้างช่องสำหรับเก็บของในการทำอาหาร
    public void CreateCookSlots()
    {
        outputSlots = new OutputSlot[cookSlotAmount]; // สร้างอาร์เรย์สำหรับเก็บช่อง
        for (int i = 0; i < cookSlotAmount; i++)
        {
            Transform slot = Instantiate(outputSlot.transform, cookInventoryPanel); // สร้างช่องใหม่ใน UI
            OutputSlot invSlot = slot.GetComponent<OutputSlot>(); // ดึง Component ของ OutputSlot

            outputSlots[i] = invSlot; // เก็บช่องที่สร้างในอาร์เรย์
            invSlot.inventory = mainInventory; // ตั้งค่าการเชื่อมโยงกับอินเวนทอรีหลัก
            invSlot.cookingInventory = this; // ตั้งค่าการเชื่อมโยงกับอินเวนทอรีการทำอาหาร
            invSlot.SetThisSlot(EMTRY_ITEM, 0); // ตั้งค่าช่องเริ่มต้นเป็นช่องว่าง
        }
    }

    // ฟังก์ชันนี้ใช้ในการใช้วัตถุดิบจากอินเวนทอรีเพื่อทำอาหาร
    public void UseCookMaterials()
    {
        Debug.Log("Using cooking materials...");
        bool materialsUsed = false;

        // ลูปผ่านสูตรทั้งหมดเพื่อตรวจสอบว่าใช้วัตถุดิบครบหรือไม่
        foreach (CookRecipe recipe in recipes)
        {
            // ตรวจสอบสูตรว่าใช้วัตถุดิบครบหรือไม่
            if (IsRecipeMatch(recipe))
            {
                Debug.Log($"Recipe match found: {recipe.name}");

                // ลูปผ่านวัตถุดิบในสูตร
                foreach (var recipeItem in recipe.recipeItems)
                {
                    // หาวัตถุดิบที่ตรงในอินเวนทอรี
                    foreach (InventorySlot slot in iteminventorySlots)
                    {
                        if (slot.item == recipeItem.ingredient && slot.stack >= recipeItem.quantity)
                        {
                            // ใช้ไอเทมจากสูตร
                            Debug.Log($"Using material: {slot.item.name}");
                            slot.UseItem(recipeItem.quantity); // ใช้ไอเทมตามจำนวนที่ต้องการ
                            mainInventory.AddItem(recipe.outputItem, recipe.outputStack); // เพิ่มไอเทมที่ทำได้ในอินเวนทอรีหลัก
                            materialsUsed = true;
                            break;
                        }
                    }
                }

                // เมื่อใช้วัสดุครบแล้วให้เช็คสูตรทั้งหมด
                CheckAllCookRecipe();
                return;
            }
        }

        // ถ้าไม่พบวัสดุที่ใช้ได้
        if (!materialsUsed)
            Debug.Log("No material used.");
    }

    // ฟังก์ชันนี้ใช้ตรวจสอบว่าสูตรสามารถทำได้หรือไม่
    public bool IsRecipeMatch(CookRecipe recipe)
    {
        // สร้าง Dictionary เพื่อเก็บจำนวนวัตถุดิบที่มี
        Dictionary<DataItem, int> availableIngredients = new Dictionary<DataItem, int>();

        // รวบรวมวัตถุดิบที่มีในช่อง inventory
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

        // ตรวจสอบว่ามีวัตถุดิบครบตามสูตรหรือไม่
        foreach (var recipeItem in recipe.recipeItems)
        {
            if (!availableIngredients.ContainsKey(recipeItem.ingredient) ||
                availableIngredients[recipeItem.ingredient] < recipeItem.quantity)
            {
                Debug.Log($"Missing ingredient: {recipeItem.ingredient.name} " +
                         $"(Need: {recipeItem.quantity}, " +
                         $"Have: {(availableIngredients.ContainsKey(recipeItem.ingredient) ? availableIngredients[recipeItem.ingredient] : 0)})");
                return false; // ถ้าวัตถุดิบไม่ครบให้คืนค่า false
            }
        }

        return true; // ถ้าวัตถุดิบครบตามสูตร
    }

    // ฟังก์ชันนี้ใช้ตรวจสอบสูตรทั้งหมดที่สามารถทำได้

    //public void DisplayRequiredIngredients(CookRecipe recipe)
    //{
    //    Debug.Log($"Displaying required ingredients for recipe: {recipe.name}");

    //    // สร้างข้อความแสดงวัตถุดิบที่จำเป็น
    //    string requiredIngredientsText = $"Recipe: {recipe.name}\nRequired Ingredients:\n";

    //    foreach (var recipeItem in recipe.recipeItems)
    //    {
    //        requiredIngredientsText += $"{recipeItem.ingredient.name} x {recipeItem.quantity}\n";
    //    }

    //    // แสดงข้อความใน UI (สมมติว่ามี Text ชื่อ requiredIngredientsTextUI)
    //    requiredIngredientsTextUI.text = requiredIngredientsText;
    //}

    public void CheckAllCookRecipe()
    {
        Debug.Log("Checking all recipes...");

        List<CookRecipe> validRecipes = new List<CookRecipe>(); // รายการสูตรที่สามารถทำได้

        // ตรวจสอบทุกสูตรใน recipes ว่ามีวัตถุดิบครบหรือไม่
        foreach (CookRecipe recipe in recipes)
        {
            if (IsRecipeMatch(recipe))
            {
                validRecipes.Add(recipe); // เพิ่มสูตรที่ทำได้ในรายการ validRecipes
                //DisplayRequiredIngredients(recipe); // แสดงวัตถุดิบที่จำเป็นสำหรับสูตรนี้
            }
        }

        // หากมีสูตรที่ทำได้ให้แสดงใน outputSlots
        if (validRecipes.Count > 0)
        {
            Debug.Log($"Found {validRecipes.Count} valid recipes!");

            // เคลียร์ช่องทั้งหมดก่อน
            foreach (OutputSlot slot in outputSlots)
            {
                slot.ClearThisCookSlot();
            }

            // แสดงสูตรที่ทำได้ใน outputSlots
            int index = 0;
            foreach (CookRecipe validRecipe in validRecipes)
            {
                if (index < outputSlots.Length)
                {
                    outputSlots[index].SetThisSlot(validRecipe.outputItem, validRecipe.outputStack); // ตั้งค่าผลลัพธ์ในช่อง
                    Debug.Log($"Displaying recipe: {validRecipe.name} in slot {index + 1}");
                    index++;
                }
            }
        }
        else
        {
            Debug.Log("No valid recipes found. Clearing all output slots...");
            // หากไม่มีสูตรที่ทำได้ เคลียร์ทุกช่องใน outputSlots
            foreach (OutputSlot slot in outputSlots)
            {
                slot.ClearThisCookSlot();
            }
        }
    }

    // ฟังก์ชันนี้ใช้คืนของทั้งหมดกลับไปยังอินเวนทอรีหลัก
    public void ReturnAllToMainInventory()
    {
        Debug.Log("Returning all items to main inventory");
        foreach (InventorySlot slot in iteminventorySlots)
        {
            if (slot.item != null && slot.item != EMTRY_ITEM)
            {
                mainInventory.AddItem(slot.item, slot.stack); // เพิ่มไอเทมกลับไปยังอินเวนทอรีหลัก
                slot.ClearThisCookSlot(); // เคลียร์ช่องในการทำอาหาร
            }
        }
    }
    #endregion


    
    //// ฟังก์ชันนี้ใช้คืนของบางรายการกลับไปยังอินเวนทอรีหลัก
    //public void ReturnToMainInventory(DataItem item, int amount)
    //{
    //    Debug.Log($"Returning {amount} {item.name} to main inventory");
    //    mainInventory.AddItem(item, amount); // เพิ่มไอเทมกลับไปยังอินเวนทอรีหลัก
    //}

    //// ฟังก์ชันนี้จะถูกเรียกเมื่อวัตถุดิบทั้งหมดถูกคืนกลับไปยังอินเวนทอรีหลัก
    //private void OnDisable()
    //{
    //    ReturnAllToMainInventory(); // คืนของทั้งหมดเมื่ออินเวนทอรีการทำอาหารถูกปิด
    //}



}

