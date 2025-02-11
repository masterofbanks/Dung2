using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public enum State
    {
        idle,
        running,
        jumping,
        falling,
        crouching,
        cw,
        rolling
    }

    [Header("State")]
    public State state;

    [Header("Movement Physics")]
    public float norm_horizontal_speed;
    public float norm_falling_speed;


    [Header("Jump Physics")]
    public float jumpForce;
    public float up_vy_grav;
    public float down_vy_grav;

    [Header("Coyote Time")]
    public float coyoteTime;
    public float coy_Timer;

    [Header("Buffer Time")]
    public float bufferTime;
    public float buffer_Timer;

    [Header("Ground Stuff")]
    public Transform feetPosition;
    public bool grounded;
    public LayerMask groundMask;
    public float groundRadius;

    //input actions
    private InputAction move;
    private InputAction jump;

    //componenets
    public Player_input PIAs;
    public PlayerAbilities abil;
    private Rigidbody2D rb;
    private Animator anime;

    //key values
    private Vector2 directional_input;
    private float horizontal_speed;
    private float max_falling_speed;


    //random bools
    private bool IAB;
    private bool facingRight;
    public float lastFacingRight;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        abil = GetComponent<PlayerAbilities>();
        anime = GetComponent<Animator>();
        facingRight = true;
        rb.gravityScale = up_vy_grav;
        horizontal_speed = norm_horizontal_speed;
        IAB = false;
        lastFacingRight = 1;
        max_falling_speed = norm_falling_speed;
        state = State.idle;
    }

    private void Awake()
    {
        PIAs = new Player_input();
    }

    // Update is called once per frame
    void Update()
    {
        anime.SetInteger("state", (int)state);

        MoveCharacter();
        Flip();
        PhysicsController();
        Grounded();
        UpdateJump();


    }

    private void MoveCharacter()
    {
        Vector2 raw_d_input = move.ReadValue<Vector2>();
        directional_input = new Vector2(System.Math.Sign(raw_d_input.x), System.Math.Sign(raw_d_input.y));

        if (!IAB)
        {

            rb.velocity = new Vector2(directional_input.x * horizontal_speed, rb.velocity.y);
        }

        

        if (directional_input.x != 0)
        {
            lastFacingRight = directional_input.x;
        }

        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -max_falling_speed, float.MaxValue));
    }

    private void Flip()
    {
        if (facingRight && System.Math.Sign(directional_input.x) == -1.0f)
        {
            transform.localScale = new Vector3(-1 * transform.localScale.x, 1, 1);
            facingRight = !facingRight;
        }

        else if (!facingRight && System.Math.Sign(directional_input.x) == 1.0f)
        {
            transform.localScale = new Vector3(-1 * transform.localScale.x, 1, 1);
            facingRight = !facingRight;
        }
    }

    private void PhysicsController()
    {
        if (!grounded)
        {

            coy_Timer -= Time.deltaTime;


            if (!abil.GetBoosting())
            {
                //if you are in the air and moving upwards, set gravity to up_vy_grav
                if (rb.velocity.y > 0.01f)
                {
                    rb.gravityScale = up_vy_grav;
                }
                //if you are in the air and are moving downwards, set the gravity to down_vy_grav
                else
                {
                    rb.gravityScale = down_vy_grav;
                }
            }
            
        }

        else
        {
            coy_Timer = coyoteTime;
        }

        if (rb.velocity.y > 0.1f)
        {
            coy_Timer = 0;
        }

        if (buffer_Timer > 0)
        {
            buffer_Timer -= Time.deltaTime;

        }
    }

    private void Grounded()
    {
        grounded = Physics2D.OverlapCircle(feetPosition.position, groundRadius, groundMask);
    }

    private void UpdateJump()
    {
        if (coy_Timer > 0 && buffer_Timer > 0)
        {
            AddJumpForce();
            buffer_Timer = 0;
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {


        buffer_Timer = bufferTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Orb"))
        {
            collision.gameObject.GetComponent<Orb_Spawn_Behavior>().Disappear();
            GetComponent<PlayerAbilities>().Set_Cans_true();
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

    public void SetHorizontalSpeed(float s)
    {
        horizontal_speed = s;
    }

    public void Set_IAB(bool i)
    {
        IAB = i;
    }

    public void AddJumpForce()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    public void SetFallingSpeed(float s)
    {
        max_falling_speed = s;
    }

    public Vector2 GetDirectionalInput()
    {
        return directional_input;
    }
    
}
