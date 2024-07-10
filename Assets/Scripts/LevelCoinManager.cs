using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelCoinManager : MonoBehaviour
{
    private int levelCoins;
    [SerializeField] private TMP_Text levelCoinsText;

    void Start()
    {
        levelCoins = 0;
        levelCoinsText.text = "Fruits : " + levelCoins;
    }

    public void CollectCoin()
    {
        levelCoins++;
        levelCoinsText.text = "Fruits : " + levelCoins;
    }

    public void LoseLevel()
    {
        levelCoins = 0;
        levelCoinsText.text = "";
    }

    public void CompleteLevel()
    {
        CoinManager.AddCoins(levelCoins);
        levelCoins = 0;
    }

    public int GetLevelCoins()
    {
        return levelCoins;
    }
}
