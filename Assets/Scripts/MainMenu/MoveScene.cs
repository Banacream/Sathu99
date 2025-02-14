using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveScene : MonoBehaviour
{
    public static void LoadSceneByName(string sceneName)
    {
        // ตรวจสอบว่า Scene ที่เราจะโหลดมีอยู่หรือไม่
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
       

            SceneManager.LoadScene(sceneName);  // โหลดฉากที่มีชื่อที่ระบุ
        }
        else
        {
            Debug.LogError("Scene " + sceneName + " ไม่สามารถโหลดได้");
        }
    }
}
