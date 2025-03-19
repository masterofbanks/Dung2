using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin_Behavior : MonoBehaviour
{
    public GameObject coin_effect;
    public GameObject coin_sfx;
    
    public void DestroyCoin()
    {
        Instantiate(coin_effect, transform.position, transform.rotation);
        GameObject[] all_coin_sfxs = GameObject.FindGameObjectsWithTag("CoinSFX");
        if(all_coin_sfxs.Length < 3)
            Instantiate(coin_sfx, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
