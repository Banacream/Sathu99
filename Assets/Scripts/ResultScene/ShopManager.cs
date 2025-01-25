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

            // ���� Tool Slot � Inventory
            InventorySlot slot = mainInventory.IsEmptySlotLeft(item);
            slot.SetThisSlot(item, 1);
        }
        //foreach (InventorySlot slot in mainInventory.toolinventorySlots)
        //{
        //    // if (slot.()) // ��Ǩ�ͺ��� Slot �����ҧ�������
        //    //{
        //    slot.SetThisSlot(item, 1); // ��������ŧ� Slot (�ӹǹ 1)
        //    Debug.Log($"Added {item.name} to tool slot.");
        //    return; // �͡�ҡ�ѧ��ѹ�ѹ����ѧ�������������
        //            // }
        //}
        //}
    }
}
