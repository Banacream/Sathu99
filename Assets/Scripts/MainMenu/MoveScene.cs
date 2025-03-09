using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveScene : MonoBehaviour
{

    public float changTime;
    public string sceneNameTime;



    private void Update()
    {
        if (changTime != 0)
        {
            changTime -= Time.deltaTime;
            if (changTime <= 0 && sceneNameTime != null)
            {
                SceneManager.LoadScene(sceneNameTime);
            }
            else
                return;
        }
    }

    public static void LoadSceneByName(string sceneName)
    {
        // ตรวจสอบว่า Scene ที่เราจะโหลดมีอยู่หรือไม่
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
       
            SceneManager.LoadScene(sceneName);  // โหลดฉากที่มีชื่อที่ระบุ
            Time.timeScale = 1;
        }
        else
        {
            Debug.LogError("Scene " + sceneName + " ไม่สามารถโหลดได้");
        }
    }
}
