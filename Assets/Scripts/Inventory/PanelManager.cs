using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject CookingPanel;
    public GameObject shopPanel;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
          

            //CookingPanel.SetActive(!CookingPanel.activeSelf);
        }


    }
    public void ActiveShopPanel()
    {
        if (shopPanel != null)
        {
            bool isActive = shopPanel.activeSelf; // ตรวจสอบสถานะปัจจุบันของ Panel
            shopPanel.SetActive(!isActive);      // สลับสถานะ (เปิด -> ปิด หรือ ปิด -> เปิด)
        }
        else
        {
            Debug.LogWarning("Panel is not assigned in the Inspector.");
        }
    }


}
    