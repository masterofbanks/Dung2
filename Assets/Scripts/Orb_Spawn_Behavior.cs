using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb_Spawn_Behavior : MonoBehaviour
{
    private SpriteRenderer sR;
    private BoxCollider2D boxColl;

    public float respawnTime;
    void Start()
    {
        sR = GetComponent<SpriteRenderer>();
        boxColl = GetComponent<BoxCollider2D>();
    }


    void Update()
    {
        
    }

    public void Disappear()
    {
        StartCoroutine(DisappearRoutine());
    }

    IEnumerator DisappearRoutine()
    {
        sR.enabled = false;
        boxColl.enabled = false;
        yield return new WaitForSeconds(respawnTime);
        sR.enabled = true;
        boxColl.enabled = true;

    }

}
