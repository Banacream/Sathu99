using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI dayText;
    private int daysWithoutPayment = 0;
    private bool hasPaidToday = false;
    public int currentDay = 0;


    private void Start()
    {
        currentDay = GameDataManager.playerData.day;
        daysWithoutPayment = GameDataManager.playerData.daysWithoutPayment;
        Debug.Log("Debyt : " + daysWithoutPayment);
        UpdateDayUI();
        CheckGameOver();
    }

    public void UpdateDayUI()
    {
        if (dayText != null)
        {
            dayText.text = $"Day: {currentDay}";
        }
        else
        {
            Debug.LogError("DayText is not assigned in the GameManager!");
        }
    }

    public void ResetAllData()
    {
        PlayerPrefsManager.ResetPlayerData();
        // Reload the scene or reset the game state as needed
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private void CheckGameOver()
    {
        daysWithoutPayment = GameDataManager.GetDaysWithoutPayment();
        if (daysWithoutPayment >= 3)
        {
            GameOver();
        }
    }


    private void GameOver()
    {
        Debug.LogError("Game Over! You failed to pay the debt for 3 consecutive days.");
        // Add logic to handle game over, such as loading a game over scene
    }


}
