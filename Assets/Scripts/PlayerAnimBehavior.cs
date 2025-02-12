using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimBehavior : MonoBehaviour
{
    private Rigidbody2D rb;

    private PlayerController controller;
    private PlayerAbilities abilityScript;

    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PlayerController>();
        abilityScript = GetComponent<PlayerAbilities>();    
    }

    

    // Update is called once per frame
    void Update()
    {
        

    }

    private void FixedUpdate()
    {
        StateController();
    }

    private void StateController()
    {
        //dead
        if (controller.GetDeath())
        {
            controller.state = PlayerController.State.dead;
            controller.DisableControls();
            abilityScript.DisableAbilities();
            if (controller.grounded)
            {
                rb.drag = 100;

            }
        }



        //rolling
        else if (abilityScript.GetRolling())
        {
            controller.state = PlayerController.State.rolling;
        }
        
        //on the ground
        else if (controller.grounded)
        {
            //moving
            if(Mathf.Abs(rb.velocity.x) > 0.1f)
            {
                
                //crouch walking
                if (controller.GetDirectionalInput().y == -1)
                {
                    controller.state = PlayerController.State.cw;
                }

                //normal running
                else
                {
                    controller.state = PlayerController.State.running;
                }
            }

            //still
            else
            {
                //crouching
                if(controller.GetDirectionalInput().y == -1)
                {
                    controller.state = PlayerController.State.crouching;
                }

                //idle
                else
                {
                    controller.state = PlayerController.State.idle;
                }
            }
        }

        //in the air
        else
        {
            //rising up
            if(rb.velocity.y > 0)
            {
                controller.state = PlayerController.State.jumping;
            }

            //falling down
            else
            {
                controller.state = PlayerController.State.falling;
            }
        }
    }

    
}
