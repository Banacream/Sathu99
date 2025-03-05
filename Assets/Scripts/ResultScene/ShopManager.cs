using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{

    public ResultManager resultManager;
    public Inventory mainInventory;
    public int price = 0;
    [Header("Slot Detail")]
    public DataItem item;
    public Button buyButton; // ปุ่มซื้อ
    private bool isPurchased = false; // สถานะการซื้อ

    private void Start()
    {
        // โหลดสถานะการซื้อจาก PlayerPrefs
        isPurchased = PlayerPrefs.GetInt("IsPurchased_" + item.name, 0) == 1;

        if (buyButton != null)
        {
            buyButton.onClick.AddListener(() => BuyItem(price));
            SetButtonState(!isPurchased); // ตั้งค่าสถานะของปุ่มตามสถานะการซื้อ
        }
    }

    public void BuyItem(int itemPrice)
    {
        price = itemPrice;

        if (GameDataManager.playerData.coins >= price && !isPurchased)
        {
            resultManager.SpendCoins(itemPrice);
            isPurchased = true;
            SetButtonState(false);

            // บันทึกสถานะการซื้อใน PlayerPrefs
            PlayerPrefs.SetInt("IsPurchased_" + item.name, 1);
            PlayerPrefs.Save();

            //InventorySlot slot = mainInventory.IsEmptySlotLeft(item);
            //slot.SetThisSlot(item, 1);
        }
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
        else
            Debug.Log($"Can Not Adding tool");

    }

    private void SetButtonState(bool isEnabled)
    {
        if (buyButton != null)
        {
            buyButton.interactable = isEnabled;
            ColorBlock colors = buyButton.colors;
            colors.normalColor = isEnabled ? Color.white : Color.gray;
            colors.highlightedColor = isEnabled ? Color.white : Color.gray;
            colors.pressedColor = isEnabled ? Color.white : Color.gray;
            buyButton.colors = colors;
        }
    }
}
