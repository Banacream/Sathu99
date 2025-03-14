using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using System.Linq; // สำหรับการใช้งาน List
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ResultManager : MonoBehaviour
{

    public int totalCoins = 0; 
    public int totalSell = 0;
    public float totalDebt= 0;
    public float withoutPaytotalDebt = 0;  
    public TextMeshProUGUI coinText; 
    public TextMeshProUGUI cookSellText; 
    public TextMeshProUGUI debtText;
    public TextMeshProUGUI daysWithoutPaymentText;
    public Inventory mainInventory;
    private List<int> slotPrices = new List<int>();


    private void Start()
    {
        GameDataManager.LoadPlayerData();
        cookSellText.text = $"{totalSell}";
        totalCoins = GameDataManager.playerData.coins;
        totalDebt = GameDataManager.playerData.debt;
        withoutPaytotalDebt = GameDataManager.playerData.daysWithoutPayment;
        UpdateCoinUI();
        if (GameDataManager.GetCurrentDay() == 5 && !GameDataManager.HasSceneChanged("CutscenesEnd"))
        {
            GameDataManager.Instance.ChangeScene("CutscenesEnd");
            GameDataManager.SetSceneChanged("CutscenesEnd");
        }
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
        cookSellText.text = $"{totalSell}";

    }

    public void NextDayInGameManager()
    {
        GameDataManager.NextDay();
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

    public void PayDebt(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogError("Payment amount must be positive.");
            return;
        }

        if (amount > totalCoins)
        {
            Debug.LogError("Not enough coins to pay the debt.");
            return;
        }

        GameDataManager.MakePayment(amount);
        totalDebt = GameDataManager.playerData.debt;
        SpendCoins(amount);
        Debug.Log($"Paid debt: {amount}. Remaining debt: {GameDataManager.GetCurrentDebt()}");
        UpdateCoinUI();
    }

    public void DebugPayDebt(int amount)
    {
      
        GameDataManager.DebugMakePayment(amount);
        totalDebt = GameDataManager.playerData.debt;
        UpdateCoinUI();
    }



    private void UpdateCoinUI()
    {
        if (coinText != null || debtText != null)
        {
            coinText.text = $"{totalCoins}"; // �ʴ��ӹǹ����­� UI
            debtText.text = $"หนี้ทั้งหมด : {totalDebt}"; // �ʴ��ӹǹ����­� UI
        }
        else
        {
            Debug.LogError("CoinText is not assigned in the GameManager!");
        }

        if (daysWithoutPaymentText != null)
        {
            int daysWithoutPayment = GameDataManager.GetDaysWithoutPayment();
            daysWithoutPaymentText.text = $"จำนวนที่ไม่ได้จ่าย : {withoutPaytotalDebt} / 3";
        }
        else
        {
            Debug.LogError("DaysWithoutPaymentText is not assigned in the ResultManager!");
        }
    }
}
