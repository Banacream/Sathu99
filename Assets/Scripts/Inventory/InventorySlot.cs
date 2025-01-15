using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    [Header("Inventory Detail")]
    public Inventory inventory; // ���������Ѻ�纡����ҧ�ԧ�֧�Թ�ǹ������ѡ

    [Header("Slot Detail")]
    public DataItem item; // �����������㹪�ͧ���
    public int stack; // �ӹǹ�ͧ����㹪�ͧ���
    //public float weight; // ���˹ѡ�ͧ���� (��Ҩ��������)

    [Header("Cook Inventory")]
    public CookingInventory cookingInventory; // ��ҧ�ԧ�֧��÷������ (�����)

    [Header("UI")]
    public Image icon; // �Ҿ�ͧ����㹪�ͧ
    public TextMeshProUGUI stackText; // ��ͤ�������ʴ��ӹǹ����㹪�ͧ
    public InventoryType slotType;
    //public TextMeshProUGUI weightText; // ��ͤ�������ʴ����˹ѡ (��Ҩ��������)

    public int slotIndex;

    public enum InventoryType
    {
        Item ,
        Tool
    }

    void Start()
    {
        // �ѧ��ѹ������� (����ö������Ѻ��õ�駤����������͹Ҥ�)
    }

    public void SetSlotType(InventoryType type)
    {
        slotType = type;  
    }


    // �ѧ��ѹ���١���¡������ա�ä�ԡ����ͧ (�� ��ä�ԡ����������)
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        // ��Ǩ�ͺ����繡�ä�ԡ�������¢ͧ�����
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // ��Ҫ�ͧ�����ҧ���� (���������) �����������
            if (item == inventory.EMTRY_ITEM)
                return;
         
            Inventory.Instance.InventoryToHand(slotIndex, slotType);
        }

    }

    public void AssignIndex(int slotIndex)
    {
        this.slotIndex = slotIndex;
    }

    // �ѧ��ѹ�������ҡ��ͧ���
    public virtual void UseItem(int quantity)
    {
        // ��Ǩ�ͺ��� quantity ���������Թ�ӹǹ������ stack
        if (quantity <= 0)
        {
            Debug.LogError("Cannot use a non-positive quantity!"); // ����͹��� quantity ���١��ͧ
            return;
        }

        // Ŵ�ӹǹ�ͧ����� stack ���������ӡ����ٹ��
        stack = Mathf.Clamp(stack - quantity, 0, item.maxStack);

        // �ҡ�ѧ������� stack ����ʴ���ͤ���
        if (stack > 0)
        {
            CheckShowText(); // �Ѿവ��ͤ����ʴ��ӹǹ
        }
        else
        {
            // ��� stack �� 0 ���ź�����͡�ҡ inventory
            inventory.RemoveItem(this); // ź��ͧ����͡�ҡ�Թ�ǹ����
        }
    }

    // �ѧ��ѹ��駤������㹪�ͧ
    public virtual void SetThisSlot(DataItem newItem, int amount)
    {
        item = newItem; // ��駤������㹪�ͧ
        icon.sprite = newItem.icon; // ��駤���Ҿ�ͧ����
        int itemAmount = amount;
        // ��Ǩ�ͺ�����������Թ�ӹǹ����˹�� maxStack
        int intInthisSlot = Mathf.Clamp(itemAmount, 0, newItem.maxStack);
        stack = intInthisSlot; // ��駤�Ҩӹǹ�ͧ����㹪�ͧ

        CheckShowText(); // �Ѿവ��ͤ����ʴ��ӹǹ

        int amountLeft = itemAmount - intInthisSlot;
        // �������������ͨҡ��õ�駤�Ҫ�ͧ���
        if (amountLeft > 0)
        {
            // ���Ҫ�ͧ��ҧ��Թ�ǹ����������������������
            InventorySlot slot = inventory.IsEmptySlotLeft(newItem, this);
            if (slot == null) // �������ժ�ͧ��ҧ���ӡ�÷������
            {
                return;
            }
            else // ����ժ�ͧ��ҧ����駤��������������㹪�ͧ���
            {
                slot.SetThisSlot(newItem, amountLeft);
            }
        }
    }

    // �ѧ��ѹ��ҹ����㹪�ͧ���Ѻ�������
    public void MergeThisSlot(DataItem mergeItem, int mergeAmount)
    {
        item = mergeItem; // ��駤������㹪�ͧ
        icon.sprite = mergeItem.icon; // ��駤���Ҿ�ͧ����

        int itemAmount = stack + mergeAmount;
        // ��Ǩ�ͺ�ӹǹ�ͧ����㹪�ͧ����Թ maxStack
        int intInThisSlot = Mathf.Clamp(itemAmount, 0, item.maxStack);
        stack = intInThisSlot; // ��駤�Ҩӹǹ�ͧ����㹪�ͧ
        CheckShowText(); // �Ѿവ��ͤ����ʴ��ӹǹ

        int amountLeft = itemAmount - intInThisSlot;
        // �������������ͨҡ��ü�ҹ
        if (amountLeft > 0)
        {
            // ���Ҫ�ͧ��ҧ��Թ�ǹ����������������������
            InventorySlot slot = inventory.IsEmptySlotLeft(mergeItem, this);
            if (slot == null) // �������ժ�ͧ��ҧ���ӡ�÷������
            {
                inventory.DropItem(mergeItem, amountLeft);
                return;
            }
            else // ����ժ�ͧ��ҧ����駤��������������㹪�ͧ���
            {
                slot.MergeThisSlot(mergeItem, amountLeft);
            }
        }
    }

    // �ѧ��ѹ��ҧ��ͧ��Թ�ǹ���ա�÷������
    public void ClearThisCookSlot()
    {
        SetThisSlot(inventory.EMTRY_ITEM, 0); // ��駤���繪�ͧ��ҧ
    }

    // �ѧ��ѹ��Ǩ�ͺ����ʴ���ͤ����ӹǹ����㹪�ͧ
    public void CheckShowText()
    {
        stackText.text = stack.ToString(); // �ʴ��ӹǹ����㹪�ͧ

        // ��� maxStack �ͧ������ӡ��� 2 ����ͧ�ʴ���ͤ����ӹǹ
        if (item.maxStack < 2)
        {
            stackText.gameObject.SetActive(false);
        }
        else
        {
            // ����������ҡ���� 1 ����ʴ���ͤ���
            if (stack > 1)
                stackText.gameObject.SetActive(true);
            else
                stackText.gameObject.SetActive(false);
        }
    }
}
