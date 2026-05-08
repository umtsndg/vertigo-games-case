using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    // This makes sure there is only ever ONE CurrencyManager in the game
    public static CurrencyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public int TotalCash
    {
        get => PlayerPrefs.GetInt("TotalCash", 300); // Gives you 300 starting cash
        set { PlayerPrefs.SetInt("TotalCash", value); PlayerPrefs.Save(); }
    }

    public int TotalGold
    {
        get => PlayerPrefs.GetInt("TotalGold", 50);  // Gives you 50 starting gold
        set { PlayerPrefs.SetInt("TotalGold", value); PlayerPrefs.Save(); }
    }
}