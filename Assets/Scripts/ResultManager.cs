using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    public int totalCoins = 0; // �ӹǹ����­���������
    public TextMeshProUGUI coinText; // ��ҧ�ԧ�֧ UI Text

    private void Start()
    {
        GameDataManager.LoadPlayerData();
        totalCoins = GameDataManager.playerData.coins;
        UpdateCoinUI(); // �ѻവ UI �����á������������
    }

    public void AddCoin(int amount)
    {
        GameDataManager.Addcoins(amount);
        totalCoins = GameDataManager.playerData.coins;
        UpdateCoinUI(); // �ѻവ UI ��ѧ��������­
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
