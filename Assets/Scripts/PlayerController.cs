using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerController : MonoBehaviour
{

    

    [Header("Movement Physics")]
    public float horizontal_speed;




    //input actions
    private InputAction move;

    //componenets
    public Player_input PIAs;
    private Rigidbody2D rb;

    //key values
    private Vector2 directional_input;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Awake()
    {
        PIAs = new Player_input();
    }

    // Update is called once per frame
    void Update()
    {
        MoveCharacter();
    }

    private void MoveCharacter()
    {
        Vector2 raw_d_input = move.ReadValue<Vector2>();
        directional_input = new Vector2(System.Math.Sign(raw_d_input.x), System.Math.Sign(raw_d_input.y));
        rb.velocity = new Vector2(directional_input.x * horizontal_speed, rb.velocity.y);
    }

    private void OnEnable()
    {
        move = PIAs.Player.Move;
        move.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
    }
}
