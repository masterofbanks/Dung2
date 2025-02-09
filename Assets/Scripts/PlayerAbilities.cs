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

    [Header("In Air Boost Physics")]
    public float IAB_Speed;
    public float IAB_Duration;

    [Header("Float Physics")]
    public float floatingSpeed;

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
    private bool canFloat;
    
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        can_IAB = false;
        canDJ = false;
        canFloat = false;

    }

    private void Awake()
    {
        PIAs = new Player_input();
    }


    // Update is called once per frame
    void Update()
    {
        
    }


    private void Boost(InputAction.CallbackContext context)
    {
        if (controller.grounded)
        {
            StartCoroutine(BoostRoutine(rollingDuration, rollingSpeed, false));
        }

        else if(in_air_boost && can_IAB)
        {
            can_IAB = false;
            StartCoroutine(BoostRoutine(IAB_Duration, IAB_Speed, true));
        }
    }

    IEnumerator BoostRoutine(float d, float s, bool cancel_grav)
    {
        controller.SetHorizontalSpeed(s);
        controller.Set_IAB(cancel_grav);

        float temp1 = controller.down_vy_grav;
        float temp2 = controller.up_vy_grav;

        if (cancel_grav)
        {
            controller.down_vy_grav = 0;
            controller.up_vy_grav = 0;
        }

        yield return new WaitForSeconds(d);

        if (cancel_grav)
        {
            controller.down_vy_grav = temp1;
            controller.up_vy_grav = temp2;
        }
        controller.Set_IAB(false);
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
            controller.SetFallingSpeed(floatingSpeed);
        }
    }

    private void EndFloat(InputAction.CallbackContext context)
    {
        canFloat = false;
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
        f.started += Float;
        f.canceled+= EndFloat;

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

    public void Set_Cans_true()
    {
        can_IAB = true;
        canDJ = true;
        canFloat = true;
    }
}
