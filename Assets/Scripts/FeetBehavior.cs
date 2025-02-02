using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetBehavior : MonoBehaviour
{
    private GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.GetComponent<PlayerController>().feetPosition.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && player != null)
        {
            player.GetComponent<PlayerController>().grounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && player != null)
        {
            player.GetComponent<PlayerController>().grounded = false;
        }
    }
}
