using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static DataItem;

public class SellManager : MonoBehaviour
{
    public Inventory mainInventory; // อินเวนทอรีหลักที่เก็บวัตถุดิบ

    public ResultManager resultManager;
    //public Image icon; // ภาพของCoin

    // ฟังก์ชันสำหรับขายไอเท็มทั้งหมดที่เป็น food
    public void SellAllFoodItems()
    {


        int totalAmount = 0; // เก็บจำนวนเงินรวมทั้งหมดที่ขายได้

        // ลูปผ่านช่องใน inventory เพื่อตรวจสอบไอเท็มประเภท food
        foreach (InventorySlot slot in mainInventory.iteminventorySlots)
        {
            if (slot.item != null && slot.item.itemType == ItemType.Food) // ตรวจสอบประเภทไอเท็ม
            {
                int sellPrice = slot.item.sellPrice * slot.stack; // คำนวณราคาขายทั้งหมด
                Debug.Log($"Selling {slot.stack} of {slot.item.name} for {sellPrice} coins.");

                totalAmount += sellPrice; // เพิ่มราคาขายรวม
                slot.ClearThisCookSlot(); // ลบไอเท็มจาก inventory หลังขาย
            }
        }

        // เพิ่มจำนวนเงินที่ขายได้ไปยัง GameManager
        if (totalAmount > 0)
        {
            Debug.Log($"Total coins earned: {totalAmount}");
            resultManager.AddCoin(totalAmount);
        }
        else
        {
            Debug.Log("No food items to sell.");
        }
    }

}
