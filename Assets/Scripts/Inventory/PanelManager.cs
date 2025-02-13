using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject shopPanel;
    public CanvasGroup cookingPanelCanvasGroup; // CanvasGroup ของ Cooking Panel
    public CanvasGroup cookSellPanelCanvasGroup; // CanvasGroup ของ Cook Sell Panel


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
            bool isActive = shopPanel.activeSelf; 
            shopPanel.SetActive(!isActive);     
        }
        else
        {
            Debug.LogWarning("Panel is not assigned in the Inspector.");
        }
    } 
    public void ActiveCookingPanel()
    {

        // Toggle สถานะของ Cooking Panel
        bool isCookingPanelActive = cookingPanelCanvasGroup.alpha > 0;

        // ปรับ alpha และการโต้ตอบของ Cooking Panel
        cookingPanelCanvasGroup.alpha = isCookingPanelActive ? 0 : 1;
        cookingPanelCanvasGroup.interactable = !isCookingPanelActive;
        cookingPanelCanvasGroup.blocksRaycasts = !isCookingPanelActive;

        // ปรับ alpha และการโต้ตอบของ Cook Sell Panel
        bool isCookSellPanelActive = cookSellPanelCanvasGroup.alpha > 0;

        cookSellPanelCanvasGroup.alpha = isCookSellPanelActive ? 0 : 1;
        cookSellPanelCanvasGroup.interactable = !isCookSellPanelActive;
        cookSellPanelCanvasGroup.blocksRaycasts = !isCookSellPanelActive;


    }


}
    