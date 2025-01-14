using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject CookingPanel;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            //CookingPanel.SetActive(!CookingPanel.activeSelf);
        }

    }

  
}
    