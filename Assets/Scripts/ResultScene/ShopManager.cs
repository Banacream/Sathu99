using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{

    public ResultManager resultManager;
    public Inventory mainInventory;
    public int price = 0;
    [Header("Slot Detail")]
    public DataItem item;

    void Start()
    {
        
    }

    public void BuyItem(int itemPrice)
    {
        price = itemPrice;

        if (GameDataManager.playerData.coins >= price)
        resultManager.SpendCoins(itemPrice);
    } 
    public void AddToolItem(DataItem item)
    {
        if (GameDataManager.playerData.coins >= price)
        {
            Debug.Log($"Adding tool item: {item.name}");

            // ค้นหา Tool Slot ใน Inventory
            InventorySlot slot = mainInventory.IsEmptySlotLeft(item);
            slot.SetThisSlot(item, 1);
        }
        //foreach (InventorySlot slot in mainInventory.toolinventorySlots)
        //{
        //    // if (slot.()) // ตรวจสอบว่า Slot นี้ว่างหรือไม่
        //    //{
        //    slot.SetThisSlot(item, 1); // เพิ่มไอเทมลงใน Slot (จำนวน 1)
        //    Debug.Log($"Added {item.name} to tool slot.");
        //    return; // ออกจากฟังก์ชันทันทีหลังเพิ่มไอเทมสำเร็จ
        //            // }
        //}
        //}
    }
}
