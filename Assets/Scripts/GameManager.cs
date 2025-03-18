using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("UI Stuff")]
    public TextMeshProUGUI CoinsText;
    public int num_coins;
    
    // Start is called before the first frame update
    void Start()
    {
        num_coins = 0;
    }

    // Update is called once per frame
    void Update()
    {
        CoinsText.text = "Coins: " + num_coins.ToString();
    }
}
