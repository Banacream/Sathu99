using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq; 


public class Inventory : MonoBehaviour
{

    public static Inventory Instance { get; private set; }

    [Header("Inventory")]
    public DataItem EMTRY_ITEM; 
    public Transform slotPrefab; 
    public Transform slotPrefabV4; 
    public Transform handleSlotPrefab;
    public Transform InventoryPanel;
    public Transform ToolInventoryPanel; 
    public Transform CookSellPanel; 
    public Transform[] HandleInventoryPanel; 
    [Header("Cook Inventory")]
    public CookingInventory cookingInventory; 
    public ResultManager resultManager; 
    public CookRecipe[] recipes;
    [Header("Tool")]
    public HandleSlot handleSlot;

    protected GridLayoutGroup gridLayoutGroup; // GridLayoutGroup 
    [Space(5)]
    public int slotAmount = 30; 
    public InventorySlot[] toolinventorySlots; 
    public InventorySlot[] iteminventorySlots; 
    public InventorySlot[] cookSellSlots; 

    [Header("Inventory Data")]
    [SerializeField]
    InventoryData invData;


    // Start is called before the first frame update


    void Start()
    {
        // �֧ GridLayoutGroup �ҡ InventoryPanel ������㹡�èѴ���§��ͧ
     
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // ����� Instance ���
            return;
        }

        Instance = this; // ��˹� Instance ���Ѻ����ͧ

        gridLayoutGroup = InventoryPanel.GetComponent<GridLayoutGroup>();

        //// ��Ŵ��������ѧ�ҡ���ҧ slots ����
        if (InventoryPanel.childCount == 0 && ToolInventoryPanel.childCount == 0 && CookSellPanel.childCount == 0)
        {
            CreateInventorySlots();
       
        }
        else
        {
            // ����� slots �������� ����� reference
            iteminventorySlots = InventoryPanel.GetComponentsInChildren<InventorySlot>();
            toolinventorySlots = ToolInventoryPanel.GetComponentsInChildren<InventorySlot>();
            cookSellSlots = CookSellPanel.GetComponentsInChildren<InventorySlot>();
           
        }


        Invoke(nameof(AssignSlotIndexes), 0.1f);
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.LoadInventory(this);
        }
    }

    #region Inventory Methods

    public void AssignSlotIndexes()
    {

        for(int i = 0; i < toolinventorySlots.Length; i++)
        {
            toolinventorySlots[i].AssignIndex(i);
        }

    }



    public virtual void CreateInventorySlots()
    {
        // ���ҧ��ͧ����Ѻ iteminventorySlots
        iteminventorySlots = new InventorySlot[slotAmount];
        for (int i = 0; i < slotAmount; i++)
        {
            Transform slot = Instantiate(slotPrefab, InventoryPanel);
            InventorySlot invSlot = slot.GetComponent<InventorySlot>();

            iteminventorySlots[i] = invSlot;
            invSlot.inventory = this;
            invSlot.SetThisSlot(EMTRY_ITEM, 0);
            invSlot.SetSlotType(InventorySlot.InventoryType.Item);
        }

        // ���ҧ��ͧ����Ѻ toolinventorySlots
        toolinventorySlots = new InventorySlot[4];
        for (int i = 0; i < 4; i++)
        {
            Transform slot = Instantiate(slotPrefab, ToolInventoryPanel);
            InventorySlot invSlot = slot.GetComponent<InventorySlot>();

            toolinventorySlots[i] = invSlot;
            invSlot.inventory = this;
            invSlot.SetThisSlot(EMTRY_ITEM, 0);
            invSlot.SetSlotType(InventorySlot.InventoryType.Tool);
        }

        cookSellSlots = new InventorySlot[10];
        for (int i = 0; i < 10; i++)
        {
            Transform slot = Instantiate(slotPrefabV4, CookSellPanel);
            InventorySlot invSlot = slot.GetComponent<InventorySlot>();

            cookSellSlots[i] = invSlot;
            invSlot.inventory = this;
            invSlot.SetThisSlot(EMTRY_ITEM, 0);
            invSlot.SetSlotType(InventorySlot.InventoryType.Food);
        }
    }



    public void InventoryToHand(int slotIndex, InventorySlot.InventoryType inventoryType)
    {
        // ตรวจสอบว่า inventoryType เป็น Tool หรือไม่
        if (inventoryType != InventorySlot.InventoryType.Tool)
        {
            Debug.Log("This slot is not a Tool inventory slot.");
            return;
        }

        // อ้างอิงถึงช่องใน Tool Inventory
        InventorySlot slot = toolinventorySlots[slotIndex];
        if (slot.item == EMTRY_ITEM)
        {
            Debug.Log("This slot is empty. No item to transfer to hand.");
            return;
        }

        // จาก handleSlot ไปยัง Tool Inventory
        if (handleSlot.item != EMTRY_ITEM)
        {
            // ค้นหาช่องว่างใน Tool Inventory
            for (int i = 0; i < toolinventorySlots.Length; i++)
            {
                // หากพบช่องว่างใน Tool Inventory
                if (toolinventorySlots[i].item == EMTRY_ITEM)
                {
                    toolinventorySlots[i].SetThisSlot(handleSlot.item, handleSlot.stack);
                    break;
                }
            }
        }

        // ย้ายไอเท็มจาก Tool Inventory ไปยัง handleSlot
        DataItem itemToEquip = slot.item;
        int itemAmount = slot.stack;

        // ตั้งค่าช่องใน Tool Inventory ให้เป็นว่าง
        slot.SetThisSlot(EMTRY_ITEM, 0);

        // ตั้งค่า handleSlot ด้วยไอเท็มที่ย้ายมา
        handleSlot.SetThisSlot(itemToEquip, itemAmount);

        Debug.Log($"Moved {itemToEquip.name} x{itemAmount} to hand.");
    }



    public void HandToInventory(InventorySlot.InventoryType inventoryType)
    {

        // ��Ǩ�ͺ��� handleSlot �����������������
        if (handleSlot.item == EMTRY_ITEM)
        {
            Debug.Log("No item in hand to transfer.");
            return; // ���������������� ����������
        }

        // �ҡ inventoryType �� Item ������������ҡ handleSlot ��ѧ Tool Inventory
        if (inventoryType == InventorySlot.InventoryType.Tool)
        {
            // ���Ҫ�ͧ��ҧ� Tool Inventory �������������ҡ handleSlot
            for (int i = 0; i < toolinventorySlots.Length; i++)
            {
                if (toolinventorySlots[i].item == EMTRY_ITEM) // ��Ҫ�ͧ��ҧ
                {
                    // ���������ҡ handleSlot ��ѧ��ͧ�����ҧ� Tool Inventory
                    toolinventorySlots[i].SetThisSlot(handleSlot.item, handleSlot.stack);

                    // ��駤�� handleSlot ����繪�ͧ��ҧ
                    handleSlot.SetThisSlot(EMTRY_ITEM, 0);

                    Debug.Log($"Moved {handleSlot.item.name} x{handleSlot.stack} to Tool Inventory.");
                    break; // ��ش�ٻ��ѧ�ҡ������������
                }
            }
        }

    }


    public InventorySlot IsEmptySlotLeft(DataItem itemChecker = null, InventorySlot itemSlot = null)
    {
        InventorySlot firstEmptySlot = null; // เก็บช่องว่างที่พบในครั้งแรก

        // เลือกช่องเก็บของตามประเภทของไอเท็ม
        List<InventorySlot> inventorySlotsToCheck;

        if (itemChecker.itemType == DataItem.ItemType.Tool)
        {
            inventorySlotsToCheck = toolinventorySlots.ToList(); // แปลงเป็น List<InventorySlot> สำหรับ Tool
        }
        else if (itemChecker.itemType == DataItem.ItemType.Food)
        {
            inventorySlotsToCheck = cookSellSlots.ToList(); // แปลงเป็น List<InventorySlot> สำหรับ Food
        }
        else
        {
            inventorySlotsToCheck = iteminventorySlots.ToList(); // แปลงเป็น List<InventorySlot> สำหรับ Item อื่นๆ
        }

        // ตรวจสอบช่องเก็บของที่เลือก
        foreach (InventorySlot slot in inventorySlotsToCheck)
        {
            // ข้ามช่องที่ตรงกับช่องปัจจุบันที่ตรวจสอบ
            if (slot == itemSlot)
                continue;

            // หากมีไอเท็มเดียวกันและยังสามารถเพิ่มจำนวนได้
            if (slot.item == itemChecker)
            {
                if (slot.stack < slot.item.maxStack)
                {
                    return slot; // คืนค่าช่องที่สามารถเพิ่มจำนวนได้
                }
            }
            // หากพบช่องว่าง ให้เก็บช่องว่างแรกที่พบ
            else if (slot.item == EMTRY_ITEM && firstEmptySlot == null)
            {
                firstEmptySlot = slot;
            }
        }

        return firstEmptySlot; // คืนค่าช่องว่าง (ถ้ามี)
    }

    // �ѧ��ѹ��������ŧ㹡�����
    public virtual void AddItem(DataItem item, int amount)
    {
   
        // ���Ҫ�ͧ�������ö����������
        InventorySlot slot = IsEmptySlotLeft(item);
        if (slot == null) // �����辺��ͧ�������ö���������� (���������)
        {
            DropItem(item, amount); // ��������͡�
            return;
        }

        // ����������������㹪�ͧ
        slot.MergeThisSlot(item, amount);
        cookingInventory.CheckAllCookRecipe(); // ��Ǩ�ͺ�ٵ�����÷������ö����
        if(resultManager != null)
        resultManager.CheckPriceFood();

    }

    // �ѧ��ѹ�����������������ö����
    public void DropItem(DataItem item, int amount)
    {
        // ���ҧ����������š�ͧ�� (�ҡ��÷������)
        SpawnItem.Instance.SpawnItemFromPlayer(item, amount);
    }

    // �ѧ��ѹź�����͡�ҡ��ͧ㹡�����
    public void RemoveItem(InventorySlot slot)
    {
        // ���絪�ͧ�繪�ͧ��ҧ
        slot.SetThisSlot(EMTRY_ITEM, 0);
    }

    #endregion

    #region Save Load
    public string SaveData()
    {
        if (iteminventorySlots == null || toolinventorySlots == null)
        {
            Debug.LogError("Inventory slots are not initialized!");
            return "";
        }

        invData = new InventoryData(this);
        return JsonUtility.ToJson(invData);
    }

    public virtual void SaveInventory()
    {

        string data = SaveData(); // ��ѧ��ѹ SaveData ���س����������
        PlayerPrefs.SetString("InventoryData", data); // �红������ PlayerPrefs
        PlayerPrefs.Save(); // �ѹ�֡������
    }

    public void LoadData(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            Debug.LogWarning("No data to load");
            return;
        }

        try
        {
            // ��Ǩ�ͺ����� slots ��������
            if (iteminventorySlots == null || toolinventorySlots == null)
            {
                // ����ѧ����� ���֧ slots ���������� panel
                iteminventorySlots = InventoryPanel.GetComponentsInChildren<InventorySlot>();
                toolinventorySlots = ToolInventoryPanel.GetComponentsInChildren<InventorySlot>();

                if (iteminventorySlots.Length == 0 || toolinventorySlots.Length == 0)
                {
                    CreateInventorySlots();
                }
                AssignSlotIndexes();
            }

            invData = JsonUtility.FromJson<InventoryData>(data);
            if (invData != null)
            {
                SetInventoryData(invData);
                cookingInventory.CheckAllCookRecipe();
                Debug.Log("Inventory loaded successfully");
            }
            else
            {
                Debug.LogError("Failed to parse inventory data");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading inventory: {e.Message}");
        }
    }

    public virtual void LoadInventory()
    {
        if (PlayerPrefs.HasKey("InventoryData"))
        {
            string data = PlayerPrefs.GetString("InventoryData");
            LoadData(data); // ��ѧ��ѹ LoadData ���س����������
        }
        else
        {
            Debug.LogWarning("No saved inventory data found.");
        }
    }

    public void SetInventoryData(InventoryData inventoryData)
    {
      if (inventoryData == null)
    {
        Debug.LogError("Inventory data is null!");
        return;
    }

    // ���ҧ slots ����ѧ�����
    if (iteminventorySlots == null || iteminventorySlots.Length == 0)
    {
        CreateInventorySlots();
    }

    // ��駤�Ң������ iteminventorySlots
    int itemSlotsLength = Mathf.Min(iteminventorySlots.Length, inventoryData.slotItemDatas.Length);
    for (int i = 0; i < itemSlotsLength; i++)
    {
        try 
        {
            if (inventoryData.slotItemDatas[i] != null)
            {
                string loadPath = "DataItem/" + inventoryData.slotItemDatas[i].itemFileName;
                DataItem item = Resources.Load<DataItem>(loadPath);

                if (item != null)
                {
                    iteminventorySlots[i].SetThisSlot(item, inventoryData.slotItemDatas[i].stack);
                }
                else
                {
                    iteminventorySlots[i].SetThisSlot(EMTRY_ITEM, 0);
                    Debug.LogWarning($"Item not found at path: {loadPath}");
                }
            }
            else
            {
                iteminventorySlots[i].SetThisSlot(EMTRY_ITEM, 0);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error setting item slot {i}: {e.Message}");
            iteminventorySlots[i].SetThisSlot(EMTRY_ITEM, 0);
        }
    }

    // ��駤�Ң������ toolinventorySlots
    int toolSlotsLength = Mathf.Min(toolinventorySlots.Length, inventoryData.slotToolDatas.Length);
    for (int i = 0; i < toolSlotsLength; i++)
    {
        try 
        {
            if (inventoryData.slotToolDatas[i] != null)
            {
                string loadPath = "DataItem/" + inventoryData.slotToolDatas[i].itemFileName;
                DataItem item = Resources.Load<DataItem>(loadPath);

                if (item != null)
                {
                    toolinventorySlots[i].SetThisSlot(item, inventoryData.slotToolDatas[i].stack);
                }
                else
                {
                    toolinventorySlots[i].SetThisSlot(EMTRY_ITEM, 0);
                    Debug.LogWarning($"Tool not found at path: {loadPath}");
                }
            }
            else
            {
                toolinventorySlots[i].SetThisSlot(EMTRY_ITEM, 0);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error setting tool slot {i}: {e.Message}");
            toolinventorySlots[i].SetThisSlot(EMTRY_ITEM, 0);
        }
    }
    }
    #endregion



    [System.Serializable]
    public class InventoryData
    {
        public InventorySlotData[] slotItemDatas;
        public InventorySlotData[] slotToolDatas;

        public InventoryData(Inventory inv)
        {
            if (inv.iteminventorySlots != null)
            {
                slotItemDatas = new InventorySlotData[inv.iteminventorySlots.Length];
                for (int i = 0; i < inv.iteminventorySlots.Length; i++)
                {
                    if (inv.iteminventorySlots[i] != null)
                    {
                        slotItemDatas[i] = new InventorySlotData(inv.iteminventorySlots[i]);
                    }
                    else
                    {
                        slotItemDatas[i] = new InventorySlotData(new InventorySlot { item = inv.EMTRY_ITEM, stack = 0 });
                    }
                }
            }

            if (inv.toolinventorySlots != null)
            {
                slotToolDatas = new InventorySlotData[inv.toolinventorySlots.Length];
                for (int i = 0; i < inv.toolinventorySlots.Length; i++)
                {
                    if (inv.toolinventorySlots[i] != null)
                    {
                        slotToolDatas[i] = new InventorySlotData(inv.toolinventorySlots[i]);
                    }
                    else
                    {
                        slotToolDatas[i] = new InventorySlotData(new InventorySlot { item = inv.EMTRY_ITEM, stack = 0 });
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class InventorySlotData
    {
        public string itemFileName;
        public int stack;

        public InventorySlotData(InventorySlot slot)
        {
            itemFileName = slot.item.name;
            stack = slot.stack;

            if (slot != null && slot.item != null)
            {
                itemFileName = slot.item.name;
                stack = slot.stack;
            }
            else
            {
                itemFileName = "";
                stack = 0;
            }

        }
    }
}
