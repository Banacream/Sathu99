using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class HandleSlot : InventorySlot
{

       private HandleSlotSync slotSync;

    void Start()
    {
        Debug.Log("HandleSlot Started");
        // �� HandleSlotSync 㹩ҡ
        slotSync = FindObjectOfType<HandleSlotSync>();
        if (slotSync == null)
        {
            Debug.LogError("HandleSlotSync not found in scene!");
        }
        CheckShowText();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if(SceneManager.GetActiveScene().name != "Result")
        Inventory.Instance.HandToInventory(slotType);
    }

    public override void SetThisSlot(DataItem newItem, int amount)
    {
        // �ҡ newItem �� null ����ʴ���ͤ����Դ��Ҵ
        if (newItem == null)
        {
            Debug.LogError("SetThisSlot: newItem is null!"); // ����� newItem �����������١��˹�
            return;
        }

        // ��駤�� item ��� icon 㹪�ͧ�������� item ����
        item = newItem;
        icon.sprite = newItem.icon;

        // �ӹǳ�ӹǹ�������ö��� slot ������Թ maxStack �ͧ item
        int itemAmount = amount;
        int intInthisSlot = Mathf.Clamp(itemAmount, 0, newItem.maxStack); // �ӹǳ�ӹǹ�����������ö����
        stack = intInthisSlot; // �Ѿഷ stack ���¨ӹǹ���ӹǳ��

        // �ʴ���ͤ���㹤͹����� OutputSlot �١��駤�����º��������
        Debug.Log($"Handle slot set with item: {newItem.name}, amount: {stack}");

        // ���¡��ҹ CheckShowText �����Ѿഷ����ʴ��Ũӹǹ�������͵
        CheckShowText();

        // sync ��ѧ slot ���
        if (slotSync != null)
        {
            slotSync.SyncSlots(this, newItem, amount);
        }

    }


}
