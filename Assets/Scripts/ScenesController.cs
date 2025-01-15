using UnityEngine;
using UnityEngine.SceneManagement;

public static class ScenesController
{
    private static HandleSlot handleSlot;
    private static Inventory inventory;
    static int mainScene = 0;

    // โหลดฉากหลัก
    public static void LoadMainScene()
    {
        PrepareForSceneTransition();
        LoadSceneByIndex(mainScene);
    }

    // โหลดฉากถัดไป
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

    // โหลดฉากก่อนหน้า
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

    // โหลดฉากตามดัชนีที่กำหนด
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

    // ฟังก์ชันสำหรับเตรียมข้อมูลก่อนเปลี่ยนฉาก
    private static void PrepareForSceneTransition()
    {
        Inventory.Instance.HandToInventory(InventorySlot.InventoryType.Tool);
        SaveCurrentSceneData();
    }

    // ฟังก์ชันสำหรับโหลดฉากโดยระบุดัชนี
    private static void LoadSceneByIndex(int index)
    {
        SceneManager.LoadScene(index);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // บันทึกข้อมูลในฉากปัจจุบัน
    private static void SaveCurrentSceneData()
    {
        Inventory currentInventory = GameObject.FindObjectOfType<Inventory>();
        if (currentInventory != null && GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SaveInventory(currentInventory);
            Debug.Log($"Saved data for scene {SceneManager.GetActiveScene().buildIndex} before transition");
        }
    }

    // โหลดข้อมูลสำหรับฉากใหม่
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

    // เรียกใช้เมื่อฉากใหม่โหลดเสร็จ
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadSceneData();
        SceneManager.sceneLoaded -= OnSceneLoaded; // ยกเลิกการสมัคร event
    }

}
