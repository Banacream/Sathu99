using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;  // เพิ่มการใช้งาน UI
using TMPro;


public class OutputSlot : InventorySlot
{
    //// Start is called before the first frame update
    public TextMeshProUGUI descriptionText;  // ตัวแปรที่เก็บ Text UI สำหรับแสดงคำอธิบาย
    public string itemDescription;  // ตัวแปรเก็บคำอธิบายของไอเทม

    void Start()
    {
        // แสดงข้อความในคอนโซลเมื่อ OutputSlot เริ่มทำงาน
        Debug.Log("OutputSlot Started");

        // เรียกใช้งาน CheckShowText เพื่ออัพเดทการแสดงผลจำนวนไอเทมในสลอต
        CheckShowText();

        // ตรวจสอบว่า inventory และ cookingInventory ถูกกำหนดค่าเรียบร้อยหรือยัง
        if (inventory == null)
            Debug.LogError("OutputSlot: inventory is not assigned!"); // หากยังไม่ได้กำหนดค่า จะให้แสดงข้อความผิดพลาด
        if (cookingInventory == null)
            Debug.LogError("OutputSlot: cookingInventory is not assigned!"); // หากยังไม่ได้กำหนดค่า จะให้แสดงข้อความผิดพลาด
    }

    // ฟังก์ชันตรวจสอบว่า OutputSlot ว่างหรือไม่
    public bool IsSlotEmpty()
    {
        // หาก item เป็น EMTRY_ITEM หรือ stack เป็น 0 ให้ถือว่าเป็นช่องว่าง
        return item == inventory.EMTRY_ITEM || stack == 0;
    }

    // ฟังก์ชันที่เรียกเมื่อมีการคลิกที่ OutputSlot (คลิกซ้าย)
    public override void OnPointerClick(PointerEventData eventData)
    {
        // ตรวจสอบว่าเป็นการคลิกซ้าย
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // หาก item ในช่องนี้เป็น EMTRY_ITEM (ช่องว่าง) ให้แสดงข้อความผิดพลาด
            if (item == cookingInventory.EMTRY_ITEM)
            {
                Debug.LogError("SetThisSlot: newItem is null!"); // แจ้งว่า item ที่จะใช้ไม่ได้ถูกกำหนด
                return;
            }
            else
            {
                // หาก item ในช่องไม่เป็นช่องว่าง ให้เรียกใช้ฟังก์ชัน UseCookMaterials ของ cookingInventory
                cookingInventory.UseCookMaterials();
            }
        }
    }

    // ฟังก์ชันที่ใช้ในการตั้งค่า item ใน OutputSlot
    public override void SetThisSlot(DataItem newItem, int amount)
    {
        // หาก newItem เป็น null ให้แสดงข้อความผิดพลาด
        if (newItem == null)
        {
            Debug.LogError("SetThisSlot: newItem is null!"); // แจ้งว่า newItem ที่จะใช้ไม่ได้ถูกกำหนด
            return;
        }

        // ตั้งค่า item และ icon ในช่องนี้ให้เป็น item ใหม่
        item = newItem;
        icon.sprite = newItem.icon;

        // คำนวณจำนวนที่สามารถเก็บใน slot โดยไม่เกิน maxStack ของ item
        int itemAmount = amount;
        int intInthisSlot = Mathf.Clamp(itemAmount, 0, newItem.maxStack); // คำนวณจำนวนไอเทมที่สามารถเก็บได้
        stack = intInthisSlot; // อัพเดท stack ด้วยจำนวนที่คำนวณได้
        itemDescription = item.description;
        descriptionText.text = itemDescription;  // แสดงคำอธิบายของไอเทม
        // แสดงข้อความในคอนโซลว่า OutputSlot ถูกตั้งค่าเรียบร้อยแล้ว
        Debug.Log($"Output slot set with item: {newItem.name}, amount: {stack}");

        // เรียกใช้งาน CheckShowText เพื่ออัพเดทการแสดงผลจำนวนไอเทมในสลอต
        CheckShowText();
    }
}
