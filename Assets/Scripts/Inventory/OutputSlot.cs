using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;  // ���������ҹ UI
using TMPro;


public class OutputSlot : InventorySlot
{
    //// Start is called before the first frame update
    public TextMeshProUGUI descriptionText;  // ����÷���� Text UI ����Ѻ�ʴ���͸Ժ��
    public string itemDescription;  // ������纤�͸Ժ�¢ͧ����

    void Start()
    {
        // �ʴ���ͤ���㹤͹������� OutputSlot ������ӧҹ
        Debug.Log("OutputSlot Started");

        // ���¡��ҹ CheckShowText �����Ѿഷ����ʴ��Ũӹǹ�������͵
        CheckShowText();

        // ��Ǩ�ͺ��� inventory ��� cookingInventory �١��˹�������º���������ѧ
        if (inventory == null)
            Debug.LogError("OutputSlot: inventory is not assigned!"); // �ҡ�ѧ������˹���� ������ʴ���ͤ����Դ��Ҵ
        if (cookingInventory == null)
            Debug.LogError("OutputSlot: cookingInventory is not assigned!"); // �ҡ�ѧ������˹���� ������ʴ���ͤ����Դ��Ҵ
    }

    // �ѧ��ѹ��Ǩ�ͺ��� OutputSlot ��ҧ�������
    public bool IsSlotEmpty()
    {
        // �ҡ item �� EMTRY_ITEM ���� stack �� 0 ���������繪�ͧ��ҧ
        return item == inventory.EMTRY_ITEM || stack == 0;
    }

    // �ѧ��ѹ������¡������ա�ä�ԡ��� OutputSlot (��ԡ����)
    public override void OnPointerClick(PointerEventData eventData)
    {
        // ��Ǩ�ͺ����繡�ä�ԡ����
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // �ҡ item 㹪�ͧ����� EMTRY_ITEM (��ͧ��ҧ) ����ʴ���ͤ����Դ��Ҵ
            if (item == cookingInventory.EMTRY_ITEM)
            {
                Debug.LogError("SetThisSlot: newItem is null!"); // ����� item �����������١��˹�
                return;
            }
            else
            {
                // �ҡ item 㹪�ͧ����繪�ͧ��ҧ ������¡��ѧ��ѹ UseCookMaterials �ͧ cookingInventory
                bool isMatched = false;

                foreach (CookRecipe recipe in cookingInventory.recipes)
                {
                    if (recipe.outputItem == item) // ��Ǩ�ͺ��������ç�Ѻ���Ѿ��ͧ�ٵ�
                    {
                        isMatched = true;

                        // ����ʴ���ٵ�
                        Debug.Log($"Using item: {item.name} from slot.");
                        cookingInventory.UseSpecificCookMaterials(recipe, this); // ������੾��㹪�ͧ���
                        break;
                    }
                    if (!isMatched)
                    {
                        Debug.LogError("The clicked item does not match any recipe output.");
                    }
                }
               // cookingInventory.UseCookMaterials();
            }
        }
    }

    // �ѧ��ѹ�����㹡�õ�駤�� item � OutputSlot
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
        itemDescription = item.description;
        descriptionText.text = itemDescription;  // �ʴ���͸Ժ�¢ͧ����
        // �ʴ���ͤ���㹤͹����� OutputSlot �١��駤�����º��������
        Debug.Log($"Output slot set with item: {newItem.name}, amount: {stack}");

        // ���¡��ҹ CheckShowText �����Ѿഷ����ʴ��Ũӹǹ�������͵
        CheckShowText();
    }
}
