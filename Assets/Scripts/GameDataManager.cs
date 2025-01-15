    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

[System.Serializable] public class PlayerData
{
    public int coins = 0;
}

public class GameDataManager : MonoBehaviour
    {
    public static PlayerData playerData = new PlayerData();
    public static GameDataManager Instance { get; private set; }
    private const string INVENTORY_SAVE_KEY = "InventoryData";
    private const string CURRENT_SCENE_KEY = "CurrentSceneInventory";


    private void Awake()
    {
        SavePlayerData();
        LoadPlayerData();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region Medthod Inventory
    public void SaveInventory(Inventory inventory)
    {
        if (inventory != null)
        {
            try
            {
                string jsonData = inventory.SaveData();
                // เก็บข้อมูลแยกตามฉาก
                string sceneKey = INVENTORY_SAVE_KEY + SceneManager.GetActiveScene().buildIndex;
                PlayerPrefs.SetString(sceneKey, jsonData);
                PlayerPrefs.SetString(CURRENT_SCENE_KEY, sceneKey); // เก็บ key ของฉากปัจจุบัน
                PlayerPrefs.Save();
                Debug.Log($"Saved inventory data for scene {SceneManager.GetActiveScene().buildIndex}: {jsonData}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error saving inventory: {e.Message}");
            }
        }
    }

    public void LoadInventory(Inventory inventory)
    {
        if (inventory != null)
        {
            try
            {
                // โหลดข้อมูลตามฉาก
                string sceneKey = INVENTORY_SAVE_KEY + SceneManager.GetActiveScene().buildIndex;
                if (PlayerPrefs.HasKey(sceneKey))
                {
                    string jsonData = PlayerPrefs.GetString(sceneKey);
                    Debug.Log($"Loading inventory data for scene {SceneManager.GetActiveScene().buildIndex}: {jsonData}");
                    inventory.LoadData(jsonData);
                }
                else
                {
                    Debug.Log($"No saved data found for scene {SceneManager.GetActiveScene().buildIndex}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error loading inventory: {e.Message}");
            }
        }
    }

    public void TransferInventoryToNewScene()
    {
        if (PlayerPrefs.HasKey(CURRENT_SCENE_KEY))
        {
            string previousSceneKey = PlayerPrefs.GetString(CURRENT_SCENE_KEY);
            string jsonData = PlayerPrefs.GetString(previousSceneKey);

            // บันทึกข้อมูลไปยังฉากใหม่
            string newSceneKey = INVENTORY_SAVE_KEY + SceneManager.GetActiveScene().buildIndex;
            PlayerPrefs.SetString(newSceneKey, jsonData);
            PlayerPrefs.SetString(CURRENT_SCENE_KEY, newSceneKey);
            PlayerPrefs.Save();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // รอให้ฉากโหลดเสร็จก่อนแล้วค่อยโหลดข้อมูล
        StartCoroutine(LoadInventoryAfterDelay());
    }

    private IEnumerator LoadInventoryAfterDelay()
    {
        yield return new WaitForSeconds(0.2f);

        Inventory inventory = FindObjectOfType<Inventory>();
        if (inventory != null)
        {
            // โอนข้อมูลมาจากฉากก่อนหน้า (ถ้ามี)
            TransferInventoryToNewScene();

            // รอให้ Inventory พร้อม
            while (inventory.iteminventorySlots == null || inventory.toolinventorySlots == null)
            {
                yield return null;
            }

            LoadInventory(inventory);
            Debug.Log("Inventory loaded after scene transition");
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnGUI()
    {
        // กำหนดตำแหน่งและขนาดของปุ่ม
        // สร้างปุ่ม Save
        if (GUILayout.Button("Save"))
        {
            // เรียกฟังก์ชัน SaveInventory
            Inventory currentInventory = GameObject.FindObjectOfType<Inventory>();
            if (currentInventory != null)
            {
                GameDataManager.Instance.SaveInventory(currentInventory);
                Debug.Log("Inventory saved!");
            }
            else
            {
                Debug.LogWarning("No Inventory found to save.");
            }
        }
        if (GUILayout.Button("Load"))
        {
            Inventory currentInventory = GameObject.FindObjectOfType<Inventory>();
            if (currentInventory != null)
            {
                GameDataManager.Instance.LoadInventory(currentInventory);
                Debug.Log("Inventory loaded!");
            }
            else
            {
                Debug.LogWarning("No Inventory found to load.");
            }
        }
    }
    #endregion

    #region Medthos PlayerData

    public static int GetCoins()
    {
        return playerData.coins;
    }
    public static void Addcoins(int amount)
    {
        playerData.coins += amount;
        SavePlayerData();
    }

    public static bool CanSpendAddcoins(int amount)
    {
        return (playerData.coins >= amount);
    }

    public static void SpendCoins(int amount)
    {
        if(CanSpendAddcoins(amount))
        playerData.coins -= amount;

    }

    public static void SavePlayerData()
    {
        PlayerPrefs.SetInt("TotalCoins", playerData.coins); // เซฟจำนวนเหรียญ
        PlayerPrefs.Save(); // บันทึกลงในระบบ
        Debug.Log("Game Saved!");
    }
    public static void LoadPlayerData()
    {
        if (PlayerPrefs.HasKey("TotalCoins"))
        {
            playerData.coins = PlayerPrefs.GetInt("TotalCoins"); // โหลดจำนวนเหรียญ
            Debug.Log($"Game Loaded! Total Coins: {playerData.coins}");
        }
        else
        {
            Debug.Log("No saved data found.");
        }
    }




    #endregion




}
