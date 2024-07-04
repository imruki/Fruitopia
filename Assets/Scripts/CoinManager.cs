using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CoinManager : MonoBehaviour
{
    static public int coins;
    public TextMeshProUGUI CoinText;
    void Update()
    {
        CoinText.text = "Coins : " + coins.ToString();
    }
}
