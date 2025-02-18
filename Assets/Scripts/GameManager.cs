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
    public GameObject gameOver;
    public GameObject gameWin;


    private void Start()
    {
        gameOver.SetActive(false); // ปิด gameOver เมื่อเริ่มเกม
        gameWin.SetActive(false); // ปิด gameWin เมื่อเริ่มเกม
        currentDay = GameDataManager.playerData.day;
        daysWithoutPayment = GameDataManager.playerData.daysWithoutPayment;
        Debug.Log("Debyt : " + daysWithoutPayment);
        UpdateDayUI();
        CheckGameOver();
        CheckWinCondition();
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
        gameOver.SetActive(false); // ปิด gameOver เมื่อรีเซ็ตเกม
        gameWin.SetActive(false); // ปิด gameWin เมื่อรีเซ็ตเกม
    }

    public void RestartGame()
    {
        // รีเซ็ตค่าต่างๆ
        GameDataManager.playerData.coins = 250;
        GameDataManager.playerData.debt = 10000f;
        GameDataManager.playerData.day = 1;
        GameDataManager.playerData.daysWithoutPayment = 0;
        //GameDataManager.Instance.SavePlayerData();

        // รีเซ็ตสถานะเกม
        hasPaidToday = false;
        currentDay = GameDataManager.playerData.day;
        daysWithoutPayment = GameDataManager.playerData.daysWithoutPayment;

        // ปิด gameOver และ gameWin
        gameOver.SetActive(false);
        gameWin.SetActive(false);

        // อัปเดต UI
        UpdateDayUI();

        // รีโหลดฉากเพื่อเริ่มเกมใหม่
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }


    //public void ResetAllData()
    //{
    //    PlayerPrefsManager.ResetPlayerData();
    //    // Reload the scene or reset the game state as needed
    //    UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    //}


    private void CheckGameOver()
    {
        daysWithoutPayment = GameDataManager.GetDaysWithoutPayment();
        if (daysWithoutPayment >= 3)
        {
            GameOver();
        }
    }
    public void CheckWinCondition()
    {
        if (GameDataManager.GetCurrentDebt() <= 0)
        {
            WinGame();
        }
    }

    private void WinGame()
    {
        Debug.Log("You Win! You have paid off all your debt.");
        if (gameWin != null)
        {
            bool isActive = gameWin.activeSelf;
            gameWin.SetActive(!isActive);
        }
        else
        {
            Debug.LogWarning("Panel is not assigned in the Inspector.");
        }
        // Add logic to handle win condition, such as loading a win scene or disabling gameplay
    }

    private void GameOver()
    {
        Debug.LogError("Game Over! You failed to pay the debt for 3 consecutive days.");
        if (gameOver != null)
        {
            gameOver.SetActive(true);
        }
        // Add logic to handle game over, such as disabling gameplay
    }


}
