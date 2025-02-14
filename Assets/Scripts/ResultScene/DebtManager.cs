using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebtManager : MonoBehaviour
{
    public ResultManager resultManager;
    public TextMeshProUGUI debtAmountText;
    private int amountToPay;

    private void Start()
    {
        GenerateRandomDebtAmount();
        UpdateDebtAmountUI();
    }

    public void PayDebt()
    {
        if (GameDataManager.Instance.HasPaidToday())
        {
            Debug.LogError("You can only pay debt once per day.");
            return;
        }

        if (GameDataManager.playerData.coins >= amountToPay)
        {
            resultManager.PayDebt(amountToPay);
           GameDataManager.Instance.MarkDebtPaid();
            GenerateRandomDebtAmount();
            UpdateDebtAmountUI();
        }
        else
        {
            Debug.LogError("Not enough coins to pay the debt.");
        }
    }

    private void GenerateRandomDebtAmount()
    {
        amountToPay = Random.Range(1, 10); // ������ҷ���ͧ���������ҧ 100 �֧ 200
    }

    private void UpdateDebtAmountUI()
    {
        if (debtAmountText != null)
        {
            debtAmountText.text = $"Debt to Pay: {amountToPay}";
        }
        else
        {
            Debug.LogError("DebtAmountText is not assigned in the DebtManager!");
        }
    }
}
