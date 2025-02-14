using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{


    private const string COINS_KEY = "TotalCoins";
    private const string DAY_KEY = "CurrentDay";
    private const string DAYS_WITHOUT_PAYMENT_KEY = "DaysWithoutPayment";
    private const string DEBT_KEY = "DEBT_KEY";

    public static void ResetPlayerData()
    {
        ResetSpecificKey(COINS_KEY);
        ResetSpecificKey(DAY_KEY);
        ResetSpecificKey(DAYS_WITHOUT_PAYMENT_KEY);
        ResetSpecificKey(DEBT_KEY);
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs data for coins, day, days without payment, and debt has been reset.");
    }


    public static void ResetPlayerDataAll()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("All PlayerPrefs data has been reset.");
    }



    public static void ResetSpecificKey(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
            Debug.Log($"PlayerPrefs key '{key}' has been reset.");
        }
        else
        {
            Debug.LogWarning($"PlayerPrefs key '{key}' does not exist.");
        }
    }
}
