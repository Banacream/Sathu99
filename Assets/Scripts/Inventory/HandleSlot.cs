using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandleSlot : InventorySlot
{

       private HandleSlotSync slotSync;

    void Start()
    {
        Debug.Log("HandleSlot Started");
        // หา HandleSlotSync ในฉาก
        slotSync = FindObjectOfType<HandleSlotSync>();
        if (slotSync == null)
        {
            Debug.LogError("HandleSlotSync not found in scene!");
        }
        CheckShowText();

    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        Inventory.Instance.HandToInventory(slotType);
    }

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

        // แสดงข้อความในคอนโซลว่า OutputSlot ถูกตั้งค่าเรียบร้อยแล้ว
        Debug.Log($"Handle slot set with item: {newItem.name}, amount: {stack}");

        // เรียกใช้งาน CheckShowText เพื่ออัพเดทการแสดงผลจำนวนไอเทมในสลอต
        CheckShowText();

        // sync ไปยัง slot อื่น
        if (slotSync != null)
        {
            slotSync.SyncSlots(this, newItem, amount);
        }

    }


}
