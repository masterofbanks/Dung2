using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimBehavior : MonoBehaviour
{
    private Rigidbody2D rb;

    private PlayerController controller;
    private PlayerAbilities abilityScript;
    private CapsuleCollider2D capsuleCollider;
    private BoxCollider2D deathCollider;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PlayerController>();
        abilityScript = GetComponent<PlayerAbilities>();   
        capsuleCollider = GetComponent<CapsuleCollider2D>();    
        deathCollider = GetComponent<BoxCollider2D>();
        capsuleCollider.enabled = true;
        deathCollider.enabled = false;
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
            capsuleCollider.enabled = false;
            deathCollider.enabled = true;
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
            //on da wall
            if (controller.walled)
            {
                controller.state = PlayerController.State.wallSliding;
            }

            //off the wall and rising up
            else if (rb.velocity.y > 0)
            {
                controller.state = PlayerController.State.jumping;
            }

            // off the wall and falling down
            else if(rb.velocity.y < 0)
            {
                controller.state = PlayerController.State.falling;

            }




            
        }
    }

    
}
