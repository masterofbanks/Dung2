using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerController : MonoBehaviour
{

    

    [Header("Movement Physics")]
    public float horizontal_speed;

    [Header("Jump Physics")]
    public float jumpForce;


    [Header("Ground Stuff")]
    public Transform feetPosition;
    public bool grounded;

    //input actions
    private InputAction move;
    private InputAction jump;

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

    private void Jump(InputAction.CallbackContext context)
    {
        if (grounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        }
    }

    private void OnEnable()
    {
        move = PIAs.Player.Move;
        jump = PIAs.Player.Jump;    

        move.Enable();
        jump.Enable();

        jump.performed += Jump;
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
    }
}
