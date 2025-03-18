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
        Instantiate(coin_sfx, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
