using UnityEngine;
using UnityEngine.SceneManagement;

public static class ScenesController
{
    static int mainScene = 0;

    // โหลดฉากหลัก
    public static void LoadMainScene()
    {
        SaveCurrentSceneData();
        SceneManager.LoadScene(mainScene);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // โหลดฉากถัดไป
    public static void LoadNextScene()
    {
        SaveCurrentSceneData();
        int currentScene = SceneManager.GetActiveScene().buildIndex;

        if (currentScene < SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(currentScene + 1);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Debug.LogWarning("No next scene available.");
        }
    }

    // โหลดฉากก่อนหน้า
    public static void LoadPreviousScene()
    {
        SaveCurrentSceneData();
        int currentScene = SceneManager.GetActiveScene().buildIndex;

        if (currentScene > 0)
        {
            SceneManager.LoadScene(currentScene - 1);
            SceneManager.sceneLoaded += OnSceneLoaded;
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
            SaveCurrentSceneData();
            SceneManager.LoadScene(index);
        }
    }

    // บันทึกข้อมูลในฉากปัจจุบัน
    private static void SaveCurrentSceneData()
    {
        Inventory currentInventory = GameObject.FindObjectOfType<Inventory>();
        if (currentInventory != null && GameDataManager.Instance != null)
        {
            // บันทึกข้อมูลของฉากปัจจุบัน
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
