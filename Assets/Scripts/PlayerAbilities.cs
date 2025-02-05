using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Abilities")]
    public bool in_air_boost;

    private InputAction boost;
    private Rigidbody2D rb;
    
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();

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

        else if(in_air_boost)
        {
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

    private void OnEnable()
    {
        boost = PIAs.Player.Boost;
        boost.Enable();
        boost.performed += Boost;
    }

    private void OnDisable()
    {
        boost.Disable();
    }
}
