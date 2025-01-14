using UnityEngine;
using UnityEngine.SceneManagement;

public static class ScenesController
{
    static int mainScene = 0;

    // ��Ŵ�ҡ��ѡ
    public static void LoadMainScene()
    {
        SaveCurrentSceneData();
        SceneManager.LoadScene(mainScene);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // ��Ŵ�ҡ�Ѵ�
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

    // ��Ŵ�ҡ��͹˹��
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

    // ��Ŵ�ҡ����Ѫ�շ���˹�
    public static void LoadScene(int index)
    {
        if (index >= 0 && index < SceneManager.sceneCountInBuildSettings)
        {
            SaveCurrentSceneData();
            SceneManager.LoadScene(index);
        }
    }

    // �ѹ�֡������㹩ҡ�Ѩ�غѹ
    private static void SaveCurrentSceneData()
    {
        Inventory currentInventory = GameObject.FindObjectOfType<Inventory>();
        if (currentInventory != null && GameDataManager.Instance != null)
        {
            // �ѹ�֡�����Ţͧ�ҡ�Ѩ�غѹ
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

    // ���¡������ͩҡ������Ŵ����
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadSceneData();
        SceneManager.sceneLoaded -= OnSceneLoaded; // ¡��ԡ�����Ѥ� event
    }

}
