using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static DataItem;

public class SellManager : MonoBehaviour
{
    public Inventory mainInventory; // �Թ�ǹ������ѡ������ѵ�شԺ

    public ResultManager resultManager;
    //public Image icon; // �Ҿ�ͧCoin

    // �ѧ��ѹ����Ѻ������������������� food
    public void SellAllFoodItems()
    {


        int totalAmount = 0; // �纨ӹǹ�Թ����������������

        // �ٻ��ҹ��ͧ� inventory ���͵�Ǩ�ͺ����������� food
        foreach (InventorySlot slot in mainInventory.iteminventorySlots)
        {
            if (slot.item != null && slot.item.itemType == ItemType.Food) // ��Ǩ�ͺ�����������
            {
                int sellPrice = slot.item.sellPrice * slot.stack; // �ӹǳ�ҤҢ�·�����
                Debug.Log($"Selling {slot.stack} of {slot.item.name} for {sellPrice} coins.");

                totalAmount += sellPrice; // �����ҤҢ�����
                slot.ClearThisCookSlot(); // ź������ҡ inventory ��ѧ���
            }
        }

        // �����ӹǹ�Թ���������ѧ GameManager
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
