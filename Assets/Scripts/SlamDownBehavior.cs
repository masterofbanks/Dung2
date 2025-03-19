using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TerrainUtils;

/*
 Script to control the movement of a slam down object;
 1. Slam down should first receded to a receded position to signal the player that its about to slam down
 2. Should then go at a high speed until the slam down contacts a piece of terrain
 3. Once it does, should then go back up to its normal position at a different, slower speed
 */

public class SlamDownBehavior : MonoBehaviour
{
    [Header("Targets")]
    public Transform normalPosition; // normal position of the slam down
    public Transform recededPosition; // slam down recedes to this position to signal the player that its about to slam down

    [Header("Speeds")]
    public float slamDownSpeed; // speed the slam down goes at when slamming down to the ground
    public float setUpSpeed; // speed the slam down goes at when its receding to its launch position
    public float moveUpSpeed; //speed the slam down goes at then moving back up to the normal position

    [Header("Wait Times")]
    public float timeBetween_idle_to_receding;
    public float timeBetween_receding_to_slamming;
    public float timeBetween_slamming_to_resetting;

    [Header("Ground Stuff")]
    public Transform groundCheckPosition;
    public LayerMask terrainMask;
    public enum State //FSM that dictates which state the slam down obj is in
    {
        idle,
        reciding,
        slamming,
        resetting
    }

    public State state;

    private float speed;
    private Vector3 target;
    private Rigidbody2D rb;
    public bool grounded;
    private float t;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        state = State.idle;
        speed = 0;
        target = normalPosition.position; //in scene view, slamdown's intial position should be at its normalPosition
        t = 0;
    }

    // Update is called once per frame
    void Update()
    {
        StateController(); //update the state -> update target and speed fields
        Grounded();
    }

    private void StateController()
    {
        if(state == State.idle)
        {
            if(t > timeBetween_idle_to_receding)
            {
                t = 0;
                state = State.reciding;
            }
            else
            {
                t += Time.deltaTime;

            }
        }

        else if(state == State.reciding)
        {
            if (transform.position == recededPosition.position)
            {
                if (t > timeBetween_receding_to_slamming)
                {
                    t = 0;
                    state = State.slamming;

                }
                else
                {
                    t += Time.deltaTime;

                }
            }
            else
            {
                var step = setUpSpeed * Time.deltaTime; // calculate distance to move
                transform.position = Vector3.MoveTowards(transform.position, recededPosition.position, step);

            }
        }

        else if(state == State.slamming)
        {
            rb.AddForce(new Vector2(0, -slamDownSpeed), ForceMode2D.Force);
        }

        else if(state == State.resetting)
        {
            
            if(transform.position == normalPosition.position)
            {
                rb.velocity = Vector2.zero;
                state = State.idle;
            }

            else if (t > timeBetween_slamming_to_resetting)
            {
                t = -0.0001f;

            }
            else if(t < timeBetween_slamming_to_resetting && t >= 0)
            {
                t += Time.deltaTime;

            }

            else
            {
                var step = moveUpSpeed * Time.deltaTime; // calculate distance to move
                transform.position = Vector3.MoveTowards(transform.position, normalPosition.position, step);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground") )
        {
            if (state == State.slamming)
            {

                state = State.resetting;

            }


        }
    }
    IEnumerator GoBackUpRoutine()
    {
        yield return new WaitForSeconds(timeBetween_slamming_to_resetting);
        
    }

    private void Grounded()
    {
        grounded = Physics2D.OverlapCircle(groundCheckPosition.position, 0.5f, terrainMask);
        
    }

    
}
