using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    [Header("Inventory Detail")]
    public Inventory inventory; // ตัวแปรสำหรับเก็บการอ้างอิงถึงอินเวนทอรีหลัก

    [Header("Slot Detail")]
    public DataItem item; // ไอเทมที่อยู่ในช่องนี้
    public int stack; // จำนวนของไอเทมในช่องนี้
    //public float weight; // น้ำหนักของไอเทม (ถ้าจำเป็นให้ใช้)

    [Header("Cook Inventory")]
    public CookingInventory cookingInventory; // อ้างอิงถึงการทำอาหาร (ถ้ามี)

    [Header("UI")]
    public Image icon; // ภาพของไอเทมในช่อง
    public TextMeshProUGUI stackText; // ข้อความที่แสดงจำนวนไอเทมในช่อง
    public InventoryType slotType;
    //public TextMeshProUGUI weightText; // ข้อความที่แสดงน้ำหนัก (ถ้าจำเป็นให้ใช้)

    public int slotIndex;

    public enum InventoryType
    {
        Item ,
        Tool
    }

    void Start()
    {
        // ฟังก์ชันเริ่มต้น (สามารถใช้สำหรับการตั้งค่าเริ่มต้นในอนาคต)
    }

    public void SetSlotType(InventoryType type)
    {
        slotType = type;  
    }


    // ฟังก์ชันที่ถูกเรียกเมื่อมีการคลิกที่ช่อง (เช่น การคลิกเพื่อใช้ไอเทม)
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        // ตรวจสอบว่าเป็นการคลิกปุ่มซ้ายของเมาส์
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // ถ้าช่องนี้ว่างเปล่า (ไม่มีไอเทม) ให้ไม่ทำอะไร
            if (item == inventory.EMTRY_ITEM)
                return;
         
            Inventory.Instance.InventoryToHand(slotIndex, slotType);
        }

    }

    public void AssignIndex(int slotIndex)
    {
        this.slotIndex = slotIndex;
    }

    // ฟังก์ชันใช้ไอเทมจากช่องนี้
    public virtual void UseItem(int quantity)
    {
        // ตรวจสอบว่า quantity ที่ใช้ไม่เกินจำนวนที่มีใน stack
        if (quantity <= 0)
        {
            Debug.LogError("Cannot use a non-positive quantity!"); // แจ้งเตือนถ้า quantity ไม่ถูกต้อง
            return;
        }

        // ลดจำนวนของไอเทมใน stack โดยไม่ให้ต่ำกว่าศูนย์
        stack = Mathf.Clamp(stack - quantity, 0, item.maxStack);

        // หากยังมีไอเทมใน stack ให้แสดงข้อความ
        if (stack > 0)
        {
            CheckShowText(); // อัพเดตข้อความแสดงจำนวน
        }
        else
        {
            // ถ้า stack เป็น 0 ให้ลบไอเทมออกจาก inventory
            inventory.RemoveItem(this); // ลบช่องนี้ออกจากอินเวนทอรี
        }
    }

    // ฟังก์ชันตั้งค่าไอเทมในช่อง
    public virtual void SetThisSlot(DataItem newItem, int amount)
    {
        item = newItem; // ตั้งค่าไอเทมในช่อง
        icon.sprite = newItem.icon; // ตั้งค่าภาพของไอเทม
        int itemAmount = amount;
        // ตรวจสอบว่าไอเทมไม่เกินจำนวนที่กำหนดใน maxStack
        int intInthisSlot = Mathf.Clamp(itemAmount, 0, newItem.maxStack);
        stack = intInthisSlot; // ตั้งค่าจำนวนของไอเทมในช่อง

        CheckShowText(); // อัพเดตข้อความแสดงจำนวน

        int amountLeft = itemAmount - intInthisSlot;
        // ถ้ามีไอเทมเหลือจากการตั้งค่าช่องนี้
        if (amountLeft > 0)
        {
            // ค้นหาช่องว่างในอินเวนทอรีเพื่อเก็บไอเทมที่เหลือ
            InventorySlot slot = inventory.IsEmptySlotLeft(newItem, this);
            if (slot == null) // ถ้าไม่มีช่องว่างให้ทำการทิ้งไอเทม
            {
                return;
            }
            else // ถ้ามีช่องว่างให้ตั้งค่าไอเทมที่เหลือในช่องนั้น
            {
                slot.SetThisSlot(newItem, amountLeft);
            }
        }
    }

    // ฟังก์ชันผสานไอเทมในช่องนี้กับไอเทมอื่น
    public void MergeThisSlot(DataItem mergeItem, int mergeAmount)
    {
        item = mergeItem; // ตั้งค่าไอเทมในช่อง
        icon.sprite = mergeItem.icon; // ตั้งค่าภาพของไอเทม

        int itemAmount = stack + mergeAmount;
        // ตรวจสอบจำนวนของไอเทมในช่องไม่เกิน maxStack
        int intInThisSlot = Mathf.Clamp(itemAmount, 0, item.maxStack);
        stack = intInThisSlot; // ตั้งค่าจำนวนของไอเทมในช่อง
        CheckShowText(); // อัพเดตข้อความแสดงจำนวน

        int amountLeft = itemAmount - intInThisSlot;
        // ถ้ามีไอเทมเหลือจากการผสาน
        if (amountLeft > 0)
        {
            // ค้นหาช่องว่างในอินเวนทอรีเพื่อเก็บไอเทมที่เหลือ
            InventorySlot slot = inventory.IsEmptySlotLeft(mergeItem, this);
            if (slot == null) // ถ้าไม่มีช่องว่างให้ทำการทิ้งไอเทม
            {
                inventory.DropItem(mergeItem, amountLeft);
                return;
            }
            else // ถ้ามีช่องว่างให้ตั้งค่าไอเทมที่เหลือในช่องนั้น
            {
                slot.MergeThisSlot(mergeItem, amountLeft);
            }
        }
    }

    // ฟังก์ชันล้างช่องในอินเวนทอรีการทำอาหาร
    public void ClearThisCookSlot()
    {
        SetThisSlot(inventory.EMTRY_ITEM, 0); // ตั้งค่าเป็นช่องว่าง
    }

    // ฟังก์ชันตรวจสอบและแสดงข้อความจำนวนไอเทมในช่อง
    public void CheckShowText()
    {
        stackText.text = stack.ToString(); // แสดงจำนวนไอเทมในช่อง

        // ถ้า maxStack ของไอเทมต่ำกว่า 2 ไม่ต้องแสดงข้อความจำนวน
        if (item.maxStack < 2)
        {
            stackText.gameObject.SetActive(false);
        }
        else
        {
            // ถ้ามีไอเทมมากกว่า 1 ให้แสดงข้อความ
            if (stack > 1)
                stackText.gameObject.SetActive(true);
            else
                stackText.gameObject.SetActive(false);
        }
    }
}
