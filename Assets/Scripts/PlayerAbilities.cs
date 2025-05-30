using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerAbilities : MonoBehaviour
{
    
    
    [Header("Rolling Physics")]
    public float rollingSpeed;
    public float rollingDuration;
    public int numMaxRolls;
    public float recoveryTime;
    
    private float r_t;
    private int numRolls;

    [Header("In Air Boost Physics")]
    public float IAB_Speed;
    public float IAB_Duration;
    private bool boosting;


    [Header("Float Physics")]
    public float floatingSpeed;
    public float maxFloatDuration;

    private bool canFloat;
    private bool isFloating;

    [Header("Abilities")]
    public bool in_air_boost;
    public bool doubleJump;
    public bool in_air_float;

    [Header("Player Componenets")]
    public Player_input PIAs;
    private PlayerController controller;

    [Header("SFX")]
    public GameObject jump_two_sfx_obj;
    public GameObject dash_sfx_obj;

    //Input Actions
    private InputAction boost;
    private InputAction dj;
    private InputAction f;
    private InputAction restart;

    //components
    private Rigidbody2D rb;
    

    //canAbilitiy
    private bool can_IAB;
    private bool canDJ;



    private bool rolling;
    private bool floating;

    


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        can_IAB = false;
        canDJ = false;
        canFloat = false;
        boosting = false;
        floating = false;
        r_t = 0;
        numRolls = numMaxRolls;
    }

    private void Awake()
    {
        PIAs = new Player_input();
    }


    // Update is called once per frame
    void Update()
    {
        if(numRolls < numMaxRolls && r_t < recoveryTime)
        {
            r_t += Time.deltaTime;
            if(r_t >= recoveryTime)
            {
                numRolls++;
                r_t = 0;
            }
        }

        
    }

    

    private void Boost(InputAction.CallbackContext context)
    {
        

        if(!controller.grounded && in_air_boost && can_IAB)
        {
            if (!controller.walled)
            {
                can_IAB = false;
                StartCoroutine(BoostRoutine(IAB_Duration, IAB_Speed));
            }
        }

        else if (controller.grounded && numRolls > 0 && controller.GetDirectionalInput().x != 0 && !rolling)
        {
            StartCoroutine(RollRoutine(rollingDuration, rollingSpeed));
        }
    }

    IEnumerator BoostRoutine(float d, float s)
    {
        boosting = true; //set boosting bool to true
        Instantiate(dash_sfx_obj, transform.position, transform.rotation); //play the dash sound effect
        controller.Set_IAB(boosting); // set controller's IAB bool to true so you cannot put inputs in to move the player
        rb.velocity = Vector2.zero; // set velo to 0
        rb.gravityScale = 0.0f; // turn off gravity
        rb.velocity = new Vector2(controller.GetLastFacingRight() * s, 0); // set the rb's new velocity to the dash speed based on the player's last inputted direction

        yield return new WaitForSeconds(d);

        //undo all the above after d seconds
        boosting = false;
        controller.Set_IAB(boosting);
    }

    IEnumerator RollRoutine(float d, float s)
    {
        rolling = true;
        numRolls--;
        controller.Set_IAB(rolling);
        rb.velocity = Vector2.zero;
        rb.velocity = new Vector2(controller.GetLastFacingRight() * s, 0);

        yield return new WaitForSeconds(d);
        rolling = false;
        controller.Set_IAB(rolling);

    }

    private void DoubleJump(InputAction.CallbackContext context)
    {
        if(!controller.grounded && canDJ && doubleJump &&!controller.walled)
        {
            canDJ = false;
            Instantiate(jump_two_sfx_obj, transform.position, transform.rotation);

            controller.AddJumpForce();
        }
    }

    private void Float(InputAction.CallbackContext context)
    {
        if(canFloat && !controller.grounded && in_air_float)
        {
            Debug.Log("Floating?");
            StartCoroutine(FloatRoutine());
        }
    }

    IEnumerator FloatRoutine()
    {
        controller.SetFallingSpeed(floatingSpeed);
        canFloat = false;
        floating = true;
        yield return new WaitForSeconds(maxFloatDuration);
        controller.SetFallingSpeed(controller.norm_falling_speed);
        floating = false;
    }

    private void Restart(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(0);
    }

    

    private void OnEnable()
    {
        boost = PIAs.Player.Boost;
        boost.Enable();
        boost.performed += Boost;

        dj = PIAs.Player.Jump;
        dj.Enable();
        dj.performed += DoubleJump;

        f = PIAs.Player.Float;
        f.Enable();
        f.performed += Float;
        

        restart = PIAs.Player.Restart;
        restart.Enable();
        restart.performed += Restart;
    }

    private void OnDisable()
    {
        boost.Disable();
        dj.Disable();
        f.Disable();
        restart.Disable(); 
    }

    public void DisableAbilities()
    {
        boost.Disable();
        dj.Disable();
        f.Disable();
    }

    public void EnableAbilities()
    {
        boost.Enable();
        dj.Enable();
        f.Enable();
    }

    public void Set_Cans_true()
    {
        can_IAB = true;
        canDJ = true;
        canFloat = true;
    }

    public bool GetBoosting()
    {
        return boosting;
    }

    public bool GetRolling()
    {
        return rolling;
    }

    public bool getFloating()
    {
        return floating;
    }
}
