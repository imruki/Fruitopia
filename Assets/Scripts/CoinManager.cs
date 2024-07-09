using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public static class CoinManager
{
    private static int TotalCoins;

    public static void AddCoins(int amount)
    {
        TotalCoins += amount;
    }

    public static void ResetCoins()
    {
        TotalCoins = 0;
    }

    public static int GetTotalCoins()
    {
        return TotalCoins;
    }
}

