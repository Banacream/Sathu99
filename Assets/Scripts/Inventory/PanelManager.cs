using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{

    public GameObject inventoryPanel;
    
    public GameObject shopPanel;
    public GameObject debtPanel;
    public GameObject debtWarnPanel;
    public GameObject cookBookPanel;
    public GameObject confirmPanel;
    public GameObject pausePanel;
    public GameObject lowHealthWarningPanel01;
    public GameObject lowHealthWarningPanel02;
    public GameObject lowHealthWarningPanel03;
    public GameObject tutorialPanel; // Panel สำหรับบทช่วยสอน
    public GameObject[] tutorialPages; // หน้าของบทช่วยสอน
    private int currentTutorialPage = 0; // หน้าปัจจุบันของบทช่วยสอน
    public CanvasGroup cookingPanelCanvasGroup; // CanvasGroup ของ Cooking Panel
    public CanvasGroup inventoryPanelCanvasGroup;
    public CanvasGroup cookSellPanelCanvasGroup; // CanvasGroup ของ Cook Sell Panel
    private float debtWarnPanelCooldown = 5f; // เวลาที่ต้องรอก่อนจะเปิดใหม่อีกครั้ง
    private float debtWarnPanelTimer = 0f;
    private const string TutorialShownKey = "TutorialShown";

    private void Start()
    {
        // แสดงบทช่วยสอนถ้ายังไม่เคยแสดงมาก่อน
        int tutorialShownCount = PlayerPrefs.GetInt(TutorialShownKey, 0);
        if (tutorialShownCount < 2)
        {
            ShowTutorial();
            PlayerPrefs.SetInt(TutorialShownKey, tutorialShownCount + 1);
            PlayerPrefs.Save();
        }
    }

    private void Update()
    {

        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            bool isinventoryPanelCanvasGroupActive = inventoryPanelCanvasGroup.alpha > 0;

            inventoryPanelCanvasGroup.alpha = isinventoryPanelCanvasGroupActive ? 0 : 1;
            inventoryPanelCanvasGroup.interactable = !isinventoryPanelCanvasGroupActive;
            inventoryPanelCanvasGroup.blocksRaycasts = !isinventoryPanelCanvasGroupActive;

            //CookingPanel.SetActive(!CookingPanel.activeSelf);
        }
        CheckDebtStatus();



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

    public void CheckDebtStatus()
    {
        if (debtWarnPanel != null)
        {
            if (GameDataManager.Instance.HasPaidToday())
            {
                debtWarnPanel.SetActive(false);
            }
            else
            {
                debtWarnPanelTimer = debtWarnPanelCooldown;
                debtWarnPanelTimer -= Time.deltaTime;
                    if (debtWarnPanelTimer <= 0f)
                    {
                    debtWarnPanel.SetActive(true);
                    }
 
            }
        }
  
    }


    public void ActiveDebtPanel()
    {
        if (debtPanel != null)
        {
            bool isActive = debtPanel.activeSelf;
            debtPanel.SetActive(!isActive);     
        }
        else
        {
            Debug.LogWarning("Panel is not assigned in the Inspector.");
        }
    }

    public void ActiveComfirmPanel()
    {
       confirmPanel.SetActive(true);
    }

    public void NotactiveComfirmPanel()
    {
        confirmPanel.SetActive(false);
    }

    public void ActivePausePanel()
    {
        pausePanel.SetActive(true);
    }

    public void NotactivePausePanel()
    {
        pausePanel.SetActive(false);
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

        if (cookBookPanel != null)
        {
            bool isActive = cookBookPanel.activeSelf;
            cookBookPanel.SetActive(!isActive);
        }
     
    }

    #region Tutorial
    public void ShowTutorial()
    {
        if (tutorialPanel != null && tutorialPages.Length > 0)
        {
            tutorialPanel.SetActive(true);
            ShowTutorialPage(0);
        }
    }

    public void ShowTutorialPage(int pageIndex)
    {
        if (pageIndex >= 0 && pageIndex < tutorialPages.Length)
        {
            for (int i = 0; i < tutorialPages.Length; i++)
            {
                tutorialPages[i].SetActive(i == pageIndex);
            }
            currentTutorialPage = pageIndex;
        }
    }

    public void NextTutorialPage()
    {
        if (currentTutorialPage < tutorialPages.Length - 1)
        {
            ShowTutorialPage(currentTutorialPage + 1);
        }
        else
        {
            tutorialPanel.SetActive(false);
        }
    }
    #endregion

    #region ShowLowHealth
    public void ShowLowHealthWarningLevel1()
    {
        if (lowHealthWarningPanel01 != null)
        {
            lowHealthWarningPanel01.SetActive(true);
        }
        if (lowHealthWarningPanel02 != null)
        {
            lowHealthWarningPanel02.SetActive(false);
        }
        if (lowHealthWarningPanel03 != null)
        {
            lowHealthWarningPanel03.SetActive(false);
        }
    }

    public void ShowLowHealthWarningLevel2()
    {
        if (lowHealthWarningPanel01 != null)
        {
            lowHealthWarningPanel01.SetActive(false);
        }
        if (lowHealthWarningPanel02 != null)
        {
            lowHealthWarningPanel02.SetActive(true);
        }
        if (lowHealthWarningPanel03 != null)
        {
            lowHealthWarningPanel03.SetActive(false);
        }
    }

    public void ShowLowHealthWarningLevel3()
    {
        if (lowHealthWarningPanel01 != null)
        {
            lowHealthWarningPanel01.SetActive(false);
        }
        if (lowHealthWarningPanel02 != null)
        {
            lowHealthWarningPanel02.SetActive(false);
        }
        if (lowHealthWarningPanel03 != null)
        {
            lowHealthWarningPanel03.SetActive(true);
        }
    }

    public void HideLowHealthWarning()
    {
        if (lowHealthWarningPanel01 != null)
        {
            lowHealthWarningPanel01.SetActive(false);
        }
        if (lowHealthWarningPanel02 != null)
        {
            lowHealthWarningPanel02.SetActive(false);
        }
        if (lowHealthWarningPanel03 != null)
        {
            lowHealthWarningPanel03.SetActive(false);
        }
    }
    #endregion

}
