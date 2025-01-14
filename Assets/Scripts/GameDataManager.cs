    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

public class GameDataManager : MonoBehaviour
    {
    public static GameDataManager Instance { get; private set; }
    private const string INVENTORY_SAVE_KEY = "InventoryData";
    private const string CURRENT_SCENE_KEY = "CurrentSceneInventory";


    private void Awake()
    {
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

    public void SaveInventory(Inventory inventory)
    {
        if (inventory != null)
        {
            try
            {
                string jsonData = inventory.SaveData();
                // �红������¡����ҡ
                string sceneKey = INVENTORY_SAVE_KEY + SceneManager.GetActiveScene().buildIndex;
                PlayerPrefs.SetString(sceneKey, jsonData);
                PlayerPrefs.SetString(CURRENT_SCENE_KEY, sceneKey); // �� key �ͧ�ҡ�Ѩ�غѹ
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
                // ��Ŵ�����ŵ���ҡ
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

            // �ѹ�֡��������ѧ�ҡ����
            string newSceneKey = INVENTORY_SAVE_KEY + SceneManager.GetActiveScene().buildIndex;
            PlayerPrefs.SetString(newSceneKey, jsonData);
            PlayerPrefs.SetString(CURRENT_SCENE_KEY, newSceneKey);
            PlayerPrefs.Save();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // �����ҡ��Ŵ���稡�͹���Ǥ�����Ŵ������
        StartCoroutine(LoadInventoryAfterDelay());
    }

    private IEnumerator LoadInventoryAfterDelay()
    {
        yield return new WaitForSeconds(0.2f);

        Inventory inventory = FindObjectOfType<Inventory>();
        if (inventory != null)
        {
            // �͹�������Ҩҡ�ҡ��͹˹�� (�����)
            TransferInventoryToNewScene();

            // ����� Inventory �����
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
        // ��˹����˹���Т�Ҵ�ͧ����
        // ���ҧ���� Save
        if (GUILayout.Button("Save"))
        {
            // ���¡�ѧ��ѹ SaveInventory
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





}
