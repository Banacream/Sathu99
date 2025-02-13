using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using System.Linq; // สำหรับการใช้งาน List
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public int totalCoins = 0; 
    public int totalSell = 0;
    public TextMeshProUGUI coinText; 
    public TextMeshProUGUI cookSellText; 
    public Inventory mainInventory;
    private List<int> slotPrices = new List<int>();

    private void Start()
    {
        GameDataManager.LoadPlayerData();
        cookSellText.text = $"Sell: {totalSell}";
        totalCoins = GameDataManager.playerData.coins;
        UpdateCoinUI(); 
    }

    public void CheckPriceFood()
    {

        int totalSell = 0;

        // เช็คว่าแต่ละ slot ใน cookSellSlots มีไอเทมอยู่หรือไม่
        foreach (InventorySlot slot in mainInventory.cookSellSlots)
        {
            if (slot.item != null && slot.stack > 0) // ตรวจสอบว่าไอเทมไม่เป็น null และมีจำนวนไอเทม
            {
                int sellPrice = slot.item.sellPrice * slot.stack; // คำนวณราคาขายของไอเทมใน slot นี้
                totalSell += sellPrice; // เพิ่มราคาไอเท็มเข้าไปใน totalSell

                // เก็บราคาล่าสุดของไอเทมใน List
                if (slotPrices.Count <= mainInventory.cookSellSlots.ToList().IndexOf(slot))
                {
                    slotPrices.Add(sellPrice); // ถ้าไม่มีราคาก็เพิ่มใหม่
                }
                else
                {
                    slotPrices[mainInventory.cookSellSlots.ToList().IndexOf(slot)] = sellPrice; // ถ้ามีแล้วก็อัปเดตราคาใหม่
                }
            }
            else
            {
                // ถ้าไม่มีไอเทม หรือมีจำนวนไอเท็มเท่ากับ 0 ให้ลบราคานี้ออกจาก List
                if (slotPrices.Count > mainInventory.cookSellSlots.ToList().IndexOf(slot))
                {
                    slotPrices.RemoveAt(mainInventory.cookSellSlots.ToList().IndexOf(slot));
                }
            }
        }

        // แสดงราคาทั้งหมดบน UI
        cookSellText.text = $"Sell: {totalSell}";

    }

    public void AddCoin(int amount)
    {
        GameDataManager.Addcoins(amount);
        totalCoins = GameDataManager.playerData.coins;
        UpdateCoinUI(); 
    }

    public void SpendCoins(int amount)
    {
        GameDataManager.SpendCoins(amount);
        totalCoins = GameDataManager.playerData.coins;
        UpdateCoinUI(); 
    }


    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = $"Coins: {totalCoins}"; // �ʴ��ӹǹ����­� UI
        }
        else
        {
            Debug.LogError("CoinText is not assigned in the GameManager!");
        }
    }
}
