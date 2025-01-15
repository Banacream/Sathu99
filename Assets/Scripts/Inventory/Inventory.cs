using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq; // เพิ่มการใช้งาน LINQ
public class Inventory : MonoBehaviour
{

    public static Inventory Instance { get; private set; }

    [Header("Inventory")]
    public DataItem EMTRY_ITEM; // ไอเทมที่ใช้เป็นตัวแทนของช่องว่างในกระเป๋า
    public Transform slotPrefab; // Prefab ของช่องในกระเป๋า
    public Transform handleSlotPrefab; // Prefab ของช่องในกระเป๋า
    public Transform InventoryPanel; // พาเนลที่ใช้แสดงกระเป๋า
    public Transform ToolInventoryPanel; // พาเนลที่ใช้แสดงกระเป๋า
    public Transform[] HandleInventoryPanel; // พาเนลที่ใช้แสดงกระเป๋า
    [Header("Cook Inventory")]
    public CookingInventory cookingInventory; // กระเป๋าสำหรับเก็บวัตถุดิบในการทำอาหาร

    [Header("Tool")]
    public HandleSlot handleSlot;

    protected GridLayoutGroup gridLayoutGroup; // GridLayoutGroup สำหรับจัดเรียงช่องในกระเป๋า
    [Space(5)]
    public int slotAmount = 30; // จำนวนช่องในกระเป๋า
    public InventorySlot[] toolinventorySlots; // อาร์เรย์ของช่องในกระเป๋า
    public InventorySlot[] iteminventorySlots; // อาร์เรย์ของช่องในกระเป๋า

    [Header("Inventory Data")]
    [SerializeField]
    InventoryData invData;

    // Start is called before the first frame update


    void Start()
    {
        // ดึง GridLayoutGroup จาก InventoryPanel เพื่อใช้ในการจัดเรียงช่อง

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // ทำลาย Instance ซ้ำ
            return;
        }


       // cookingInventory.CheckAllCookRecipe();

        Instance = this; // กำหนด Instance ให้กับตัวเอง

        gridLayoutGroup = InventoryPanel.GetComponent<GridLayoutGroup>();
        //CreateInventorySlots(); // สร้างช่องในกระเป๋า
        ////CreateHandleInventorySlots();
        //AssignSlotIndexes();
        //// โหลดข้อมูลหลังจากสร้างช่องในกระเป๋า
        ////saveLoadData.LoadInventoryData(); // โหลดข้อมูลจากไฟล์
        //// โหลดข้อมูลหลังจากสร้าง slots เสร็จ
        if (InventoryPanel.childCount == 0 && ToolInventoryPanel.childCount == 0)
        {
            CreateInventorySlots();
            AssignSlotIndexes();
        }
        else
        {
            // ถ้ามี slots อยู่แล้ว ให้เก็บ reference
            iteminventorySlots = InventoryPanel.GetComponentsInChildren<InventorySlot>();
            toolinventorySlots = ToolInventoryPanel.GetComponentsInChildren<InventorySlot>();
            AssignSlotIndexes();
        }




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
        // สร้างช่องสำหรับ iteminventorySlots
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

        // สร้างช่องสำหรับ toolinventorySlots
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
    }



    public void InventoryToHand(int slotIndex, InventorySlot.InventoryType inventoryType)
    {
        // ดึงข้อมูลไอเทมจาก Tool Inventory
        InventorySlot slot = toolinventorySlots[slotIndex];
        if (slot.item == EMTRY_ITEM)
        {
            Debug.Log("This slot is empty. No item to transfer to hand.");
            return;
        }

        // หาก handleSlot มีไอเทมอยู่แล้ว ให้ส่งค่ากลับไปที่ Tool Inventory
        if (handleSlot.item != EMTRY_ITEM)
        {
            // ส่งค่าไอเทมที่มีอยู่ใน handleSlot กลับไปที่ Tool Inventory
            for (int i = 0; i < toolinventorySlots.Length; i++)
            {
                // ค้นหาช่องว่างใน Tool Inventory เพื่อนำไอเทมกลับไป
                if (toolinventorySlots[i].item == EMTRY_ITEM)
                {
                    toolinventorySlots[i].SetThisSlot(handleSlot.item, handleSlot.stack);
                    break;
                }
            }
        }

        // ย้ายไอเทมจาก Tool Inventory ไปยังมือ (handleSlot)
        DataItem itemToEquip = slot.item;
        int itemAmount = slot.stack;

        // ตั้งค่าช่องใน Tool Inventory ให้ว่างเปล่า
        slot.SetThisSlot(EMTRY_ITEM, 0);

        // ตั้งค่า handleSlot ด้วยไอเทมที่ย้ายมา
        handleSlot.SetThisSlot(itemToEquip, itemAmount);

        Debug.Log($"Moved {itemToEquip.name} x{itemAmount} to hand.");
    }



    public void HandToInventory(InventorySlot.InventoryType inventoryType)
    {

        // ตรวจสอบว่า handleSlot มีไอเทมอยู่หรือไม่
        if (handleSlot.item == EMTRY_ITEM)
        {
            Debug.Log("No item in hand to transfer.");
            return; // ถ้าไม่มีไอเทมในมือ ก็ไม่ทำอะไร
        }

        // หาก inventoryType เป็น Item ให้ย้ายไอเทมจาก handleSlot ไปยัง Tool Inventory
        if (inventoryType == InventorySlot.InventoryType.Tool)
        {
            // ค้นหาช่องว่างใน Tool Inventory เพื่อย้ายไอเทมจาก handleSlot
            for (int i = 0; i < toolinventorySlots.Length; i++)
            {
                if (toolinventorySlots[i].item == EMTRY_ITEM) // ถ้าช่องว่าง
                {
                    // ย้ายไอเทมจาก handleSlot ไปยังช่องที่ว่างใน Tool Inventory
                    toolinventorySlots[i].SetThisSlot(handleSlot.item, handleSlot.stack);

                    // ตั้งค่า handleSlot ให้เป็นช่องว่าง
                    handleSlot.SetThisSlot(EMTRY_ITEM, 0);

                    Debug.Log($"Moved {handleSlot.item.name} x{handleSlot.stack} to Tool Inventory.");
                    break; // หยุดลูปหลังจากย้ายไอเทมแล้ว
                }
            }
        }

    }

    //public virtual void CreateInventorySlots()
    //{
    //    iteminventorySlots = new InventorySlot[slotAmount]; // สร้างอาร์เรย์ของช่องในกระเป๋า
    //    for (int i = 0; i < slotAmount; i++)
    //    {
    //        // สร้างช่องใหม่จาก prefab และเพิ่มไปยัง InventoryPanel
    //        Transform slot = Instantiate(slotPrefab, InventoryPanel);
    //        InventorySlot invSlot = slot.GetComponent<InventorySlot>();

    //        iteminventorySlots[i] = invSlot; // กำหนดช่องที่สร้างให้กับอาร์เรย์
    //        invSlot.inventory = this; // กำหนดให้ช่องนี้รู้จักกับ Inventory
    //        invSlot.SetThisSlot(EMTRY_ITEM, 0); // กำหนดค่าเริ่มต้นให้กับช่อง (ช่องว่าง)
    //    }
    //}

    // ฟังก์ชันเช็คว่ามีช่องว่างในกระเป๋าหรือไม่ และคืนค่าช่องที่ว่าง
    public InventorySlot IsEmptySlotLeft(DataItem itemChecker = null, InventorySlot itemSlot = null)
    {
        InventorySlot firstEmptySlot = null; // เก็บช่องว่างที่พบในครั้งแรก

        // ตรวจสอบประเภทของไอเทม และเลือกช่องที่เหมาะสม
        List<InventorySlot> inventorySlotsToCheck = itemChecker.itemType == DataItem.ItemType.Tool
            ? toolinventorySlots.ToList() // แปลงเป็น List<InventorySlot> ถ้าเป็น Tool
            : iteminventorySlots.ToList(); // แปลงเป็น List<InventorySlot> ถ้าไม่ใช่ Tool

        // ลูปผ่านช่องที่เลือก
        foreach (InventorySlot slot in inventorySlotsToCheck)
        {
            // ข้ามช่องที่เป็นช่องเดียวกันกับที่ส่งมา
            if (slot == itemSlot)
                continue;

            // ถ้าไอเทมในช่องตรงกับไอเทมที่ต้องการ และยังไม่เต็ม
            if (slot.item == itemChecker)
            {
                if (slot.stack < slot.item.maxStack)
                {
                    return slot; // คืนค่าช่องที่สามารถเพิ่มไอเทมได้
                }
            }
            // ถ้าช่องว่างและยังไม่เจอช่องว่างที่แรก
            else if (slot.item == EMTRY_ITEM && firstEmptySlot == null)
                firstEmptySlot = slot;
        }

        return firstEmptySlot; // คืนค่าช่องที่ว่าง
    }

    // ฟังก์ชันเพิ่มไอเทมลงในกระเป๋า
    public virtual void AddItem(DataItem item, int amount)
    {
        // ค้นหาช่องที่สามารถเพิ่มไอเทมได้
        InventorySlot slot = IsEmptySlotLeft(item);
        if (slot == null) // ถ้าไม่พบช่องที่สามารถเพิ่มไอเทมได้ (กระเป๋าเต็ม)
        {
            DropItem(item, amount); // ทิ้งไอเทมออกไป
            return;
        }

        // รวมไอเทมที่มีอยู่ในช่อง
        slot.MergeThisSlot(item, amount);
        cookingInventory.CheckAllCookRecipe(); // ตรวจสอบสูตรอาหารที่สามารถทำได้

    }

    // ฟังก์ชันทิ้งไอเทมที่ไม่สามารถเก็บได้
    public void DropItem(DataItem item, int amount)
    {
        // สร้างไอเทมใหม่ในโลกของเกม (จากการทิ้งไอเทม)
        SpawnItem.Instance.SpawnItemFromPlayer(item, amount);
    }

    // ฟังก์ชันลบไอเทมออกจากช่องในกระเป๋า
    public void RemoveItem(InventorySlot slot)
    {
        // รีเซ็ตช่องเป็นช่องว่าง
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

        string data = SaveData(); // ใช้ฟังก์ชัน SaveData ที่คุณมีอยู่แล้ว
        PlayerPrefs.SetString("InventoryData", data); // เก็บข้อมูลใน PlayerPrefs
        PlayerPrefs.Save(); // บันทึกข้อมูล
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
            // ตรวจสอบว่ามี slots อยู่แล้ว
            if (iteminventorySlots == null || toolinventorySlots == null)
            {
                // ถ้ายังไม่มี ให้ดึง slots ที่มีอยู่ใน panel
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
            LoadData(data); // ใช้ฟังก์ชัน LoadData ที่คุณมีอยู่แล้ว
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

    // สร้าง slots ถ้ายังไม่มี
    if (iteminventorySlots == null || iteminventorySlots.Length == 0)
    {
        CreateInventorySlots();
    }

    // ตั้งค่าข้อมูลใน iteminventorySlots
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

    // ตั้งค่าข้อมูลใน toolinventorySlots
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
