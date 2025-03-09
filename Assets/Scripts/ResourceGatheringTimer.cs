using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class ResourceGatheringTimer : MonoBehaviour
{
    public float gatheringTime = 300f; // ���ҷ����㹡�����ѵ�شԺ (�Թҷ�)
    public TextMeshProUGUI timerText; // UI ����Ѻ�ʴ�����
    public string targetSceneName; // ���ͧ͢�չ����ͧ���������������������شŧ

    private float remainingTime;

    private void Start()
    {
        remainingTime = gatheringTime;
        UpdateTimerUI();

    }

    private void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime < 0)
            {
                remainingTime = 0;
            }
            UpdateTimerUI();

            if (remainingTime == 0)
            {
                OnTimeUp();
            }
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            Debug.LogError("TimerText is not assigned in the ResourceGatheringTimer!");
        }
    }

    private void OnTimeUp()
    {
        Debug.Log("Time is up! Moving to the target scene.");
        ScenesController.LoadNextScene();
    }
}
