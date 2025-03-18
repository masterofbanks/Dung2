using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

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
        rolling,
        dead,
        wallSliding
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
    public LayerMask terrainMask;
    public float groundRadius;

    [Header("Wall Stuff")]
    public Transform wall_check_position;
    public bool walled;
    public float wallRadius;
    public Vector2 wallJumpingPower;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;


    [Header("Spike Death Physics")]
    public Vector2 spikeForce;

    [Header("SFX")]
    public GameObject jump_one_sfx_obj;
    public GameObject wj_sfx_obj;


    //input actions
    private InputAction move;
    private InputAction jump;

    //componenets

    private Player_input PIAs;
    private PlayerAbilities abil;
    private Rigidbody2D rb;
    private Animator anime;

    //key values
    private Vector2 directional_input;
    private float horizontal_speed;
    private float max_falling_speed;


    //random bools
    private bool IAB;
    private bool facingRight;
    private float lastFacingRight;
    private bool dead;

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
        if (!dead)
        {
            MoveCharacter();
            UpdateJump();
            Flip();

        }

        PhysicsController();
        Grounded();
        Walled();
        WallSlide();

    }

    private void MoveCharacter()
    {
        Vector2 raw_d_input = move.ReadValue<Vector2>();
        directional_input = new Vector2(System.Math.Sign(raw_d_input.x), System.Math.Sign(raw_d_input.y));

        if (!IAB && !isWallJumping)
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
        if (facingRight && System.Math.Sign(directional_input.x) == -1.0f && !IAB && !isWallJumping)
        {
            transform.localScale = new Vector3(-1 * transform.localScale.x, 1, 1);
            facingRight = !facingRight;
        }

        else if (!facingRight && System.Math.Sign(directional_input.x) == 1.0f && !IAB && !isWallJumping)
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
        grounded = Physics2D.OverlapCircle(feetPosition.position, groundRadius, terrainMask);
    }

    private void Walled()
    {
        walled = Physics2D.OverlapCircle(wall_check_position.position, wallRadius, terrainMask) && !grounded && directional_input.x != 0;
    }

    private void UpdateJump()
    {
        if (coy_Timer > 0 && buffer_Timer > 0)
        {
            AddJumpForce();
            Instantiate(jump_one_sfx_obj, transform.position, transform.rotation);

            buffer_Timer = 0;
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {


        buffer_Timer = bufferTime;
        if (walled)
        {
            isWallJumping = false;
            if (facingRight)
            {
                wallJumpingDirection = -1;
            }

            else
            {
                wallJumpingDirection = 1;
            }

            wallJumpingCounter = wallJumpingTime;
            CancelInvoke(nameof(StopWallJumping));
        }

        else
        {
            wallJumpingCounter -= Time.deltaTime;

        }

        if (wallJumpingCounter > 0f) //&& and can Wall jump
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            transform.localScale = new Vector3(-1 * transform.localScale.x, 1, 1);
            facingRight = !facingRight;
            Instantiate(wj_sfx_obj, transform.position, transform.rotation);
            
            wallJumpingCounter = 0f;

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void WallSlide()
    {
        if (walled)
        {
            SetFallingSpeed(abil.floatingSpeed);
        }
        else if (!abil.getFloating())
        {
            SetFallingSpeed(norm_falling_speed);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Orb"))
        {
            collision.gameObject.GetComponent<Orb_Spawn_Behavior>().Disappear();
            GetComponent<PlayerAbilities>().Set_Cans_true();
        }

        else if (collision.gameObject.CompareTag("Spikes") && !dead)
        {
            dead = true;
            Vector2 sF = new Vector2(-1* lastFacingRight * spikeForce.x, spikeForce.y);
            rb.velocity = sF;
        }
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && dead)
        {
            rb.velocity = Vector2.zero;
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
        DisableControls();
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

    public bool GetDeath()
    {
        return dead;
    }

    public void DisableControls()
    {
        move.Disable();
        jump.Disable();
    }

    public float GetLastFacingRight()
    {
        return lastFacingRight;
    }

    public bool CanGroundJump()
    {
        return coy_Timer > 0 && buffer_Timer > 0;
    }

    public bool Get_isWallJumping()
    {
        return isWallJumping;
    }
    
}
