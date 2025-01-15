using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public int totalCoins = 0; // จำนวนเหรียญทั้งหมดในเกม
    public TextMeshProUGUI coinText; // อ้างอิงถึง UI Text

    private void Start()
    {
        GameDataManager.LoadPlayerData();
        totalCoins = GameDataManager.playerData.coins;
        UpdateCoinUI(); // อัปเดต UI ครั้งแรกเมื่อเริ่มเกม
    }

    public void AddCoin(int amount)
    {
        GameDataManager.Addcoins(amount);
        totalCoins = GameDataManager.playerData.coins;
        UpdateCoinUI(); // อัปเดต UI หลังเพิ่มเหรียญ
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = $"Coins: {totalCoins}"; // แสดงจำนวนเหรียญใน UI
        }
        else
        {
            Debug.LogError("CoinText is not assigned in the GameManager!");
        }
    }
}
