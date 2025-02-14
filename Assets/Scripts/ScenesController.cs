using UnityEngine;
using UnityEngine.SceneManagement;

public static class ScenesController
{

    static int mainScene = 0;

    // ��Ŵ�ҡ��ѡ
    public static void LoadMainScene()
    {
        PrepareForSceneTransition();
        LoadSceneByIndex(mainScene);
    }

    // ��Ŵ�ҡ�Ѵ�
    public static void LoadNextScene()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;

        if (currentScene < SceneManager.sceneCountInBuildSettings - 1)
        {
            PrepareForSceneTransition();
            LoadSceneByIndex(currentScene + 1);
        }
        else
        {
            Debug.LogWarning("No next scene available.");
        }
    }

    // ��Ŵ�ҡ��͹˹��
    public static void LoadPreviousScene()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;

        if (currentScene > 0)
        {
            PrepareForSceneTransition();
            LoadSceneByIndex(currentScene - 1);
        }
        else
        {
            Debug.LogWarning("No previous scene available.");
        }
    }

    // ��Ŵ�ҡ����Ѫ�շ���˹�
    public static void LoadScene(int index)
    {
        if (index >= 0 && index < SceneManager.sceneCountInBuildSettings)
        {
            PrepareForSceneTransition();
            LoadSceneByIndex(index);
        }
        else
        {
            Debug.LogWarning("Invalid scene index.");
        }
    }

    // �ѧ��ѹ����Ѻ����������š�͹����¹�ҡ
    private static void PrepareForSceneTransition()
    {
        Inventory.Instance.HandToInventory(InventorySlot.InventoryType.Tool);
        SaveCurrentSceneData();
    }

    // �ѧ��ѹ����Ѻ��Ŵ�ҡ���кشѪ��
    private static void LoadSceneByIndex(int index)
    {
        SceneManager.LoadScene(index);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // �ѹ�֡������㹩ҡ�Ѩ�غѹ
    private static void SaveCurrentSceneData()
    {
        Inventory currentInventory = GameObject.FindObjectOfType<Inventory>();
        if (currentInventory != null && GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SaveInventory(currentInventory);
            Debug.Log($"Saved data for scene {SceneManager.GetActiveScene().buildIndex} before transition");
        }
    }

    // ��Ŵ����������Ѻ�ҡ����
    private static void LoadSceneData()
    {
        if (GameDataManager.Instance != null)
        {
            Inventory newInventory = GameObject.FindObjectOfType<Inventory>();
            if (newInventory != null)
            {
                GameDataManager.Instance.LoadInventory(newInventory);
                Debug.Log("Scene data loaded.");
            }
            else
            {
                Debug.LogWarning("No Inventory found in the new scene.");
            }
        }
    }


    // ฟังก์ชันสำหรับย้ายไปยังฉากที่กำหนดชื่อ
   

    // ���¡������ͩҡ������Ŵ����
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadSceneData();
        SceneManager.sceneLoaded -= OnSceneLoaded; // ¡��ԡ�����Ѥ� event
    }

}
