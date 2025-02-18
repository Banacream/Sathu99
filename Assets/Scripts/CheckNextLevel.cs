using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckNextLevel : MonoBehaviour
{
    public string requiredItemName; // ���ͧ͢���������ͧ���
    public GameObject targetObject; // ��觢ͧ���ͻ�еٷ���ź�����Դ
    public GameObject endGame;
    public HandleSlot handleSlot; // ��ҧ�ԧ�֧ HandleSlot

    public float interactionDistance = 2.0f; // ���з������ö�ӡ������
    private Transform playerTransform;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        endGame.SetActive(false);
    }

    private void Update()
    {
        // ��Ǩ�ͺ������ҧ�����ҧ�����蹡Ѻ��觢ͧ
        if (Vector3.Distance(playerTransform.position, transform.position) <= interactionDistance)
        {
            // �ӡ��������ͼ����蹡������觢ͧ
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
                Destroy(targetObject); // ź��觢ͧ���ͻ�е�
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
