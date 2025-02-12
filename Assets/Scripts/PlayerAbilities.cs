using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerAbilities : MonoBehaviour
{
    public Player_input PIAs;
    public PlayerController controller;
    
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
    public bool floating;


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

    


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        can_IAB = false;
        canDJ = false;
        canFloat = false;
        boosting = false;
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
            can_IAB = false;
            StartCoroutine(BoostRoutine(IAB_Duration, IAB_Speed));
        }

        else if (controller.grounded && numRolls > 0 && controller.GetDirectionalInput().x != 0)
        {
            StartCoroutine(RollRoutine(rollingDuration, rollingSpeed));
        }
    }

    IEnumerator BoostRoutine(float d, float s)
    {
        boosting = true;
        controller.Set_IAB(boosting);
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0.0f;
        rb.velocity = new Vector2(controller.GetLastFacingRight() * s, 0);

        yield return new WaitForSeconds(d);
        boosting = false;
        controller.Set_IAB(boosting);
    }

    IEnumerator RollRoutine(float d, float s)
    {
        rolling = true;
        numRolls--;
        controller.SetHorizontalSpeed(s);
        yield return new WaitForSeconds(d);
        rolling = false;
        controller.SetHorizontalSpeed(controller.norm_horizontal_speed);

    }

    private void DoubleJump(InputAction.CallbackContext context)
    {
        if(!controller.grounded && canDJ && doubleJump)
        {
            canDJ = false;
            controller.AddJumpForce();
        }
    }

    private void Float(InputAction.CallbackContext context)
    {
        if(canFloat && !controller.grounded && floating)
        {
            StartCoroutine(FloatRoutine());
        }
    }

    IEnumerator FloatRoutine()
    {
        controller.SetFallingSpeed(floatingSpeed);
        yield return new WaitForSeconds(maxFloatDuration);
        controller.SetFallingSpeed(controller.norm_falling_speed);
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
        boost = PIAs.Player.Boost;
        boost.Enable();
        boost.performed += Boost;

        dj = PIAs.Player.Jump;
        dj.Enable();
        dj.performed += DoubleJump;

        f = PIAs.Player.Float;
        f.Enable();
        f.performed += Float;
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
}
