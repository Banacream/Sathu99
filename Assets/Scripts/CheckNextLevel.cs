using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckNextLevel : MonoBehaviour
{
    public string requiredItemName; // ชื่อของไอเท็มที่ต้องการ
    public GameObject targetObject; // สิ่งของหรือประตูที่จะลบหรือเปิด
    public GameObject endGame;
    public HandleSlot handleSlot; // อ้างอิงถึง HandleSlot

    public float interactionDistance = 2.0f; // ระยะที่สามารถทำการเช็คได้
    private Transform playerTransform;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        endGame.SetActive(false);
    }

    private void Update()
    {
        // ตรวจสอบระยะห่างระหว่างผู้เล่นกับสิ่งของ
        if (Vector3.Distance(playerTransform.position, transform.position) <= interactionDistance)
        {
            // ทำการเช็คเมื่อผู้เล่นกดที่สิ่งของ
            if (Input.GetMouseButtonDown(0))
            {
                CheckItemInHand();
            }
        }
    }

    private void CheckItemInHand()
    {
        if (handleSlot == null || handleSlot.item == null)
        {
            Debug.Log("No item equipped in the handle slot.");
            return;
        }

        string equippedItemName = handleSlot.item.name; // Get item name from the HandleSlot

        if (equippedItemName == requiredItemName)
        {
            if (targetObject != null)
            {
                Destroy(targetObject); // ลบสิ่งของหรือประตู
                if (endGame != null)
                {
                    endGame.SetActive(true);
                }
                Debug.Log("Target object destroyed.");
            }
            else
            {
                Debug.LogWarning("Target object is not assigned in the Inspector.");
            }
        }
    }
}
