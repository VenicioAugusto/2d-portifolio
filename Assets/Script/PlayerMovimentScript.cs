using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovimentScript : MonoBehaviour
{
    [SerializeField]
    private CharacterController2D controller;
    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private Animator playerAnimator;
    private Rigidbody2D m_Rigidbody2;

    private SpriteRenderer playerSprite;

    private float horizontal = 0f;

    [SerializeField]
    private float runSpeedValue = 20f;
    private float runSpeed = 1f;
    [SerializeField]
    private float normalSpeed = 40f;

    private bool isRunning = false;
    private bool isCrouching = false;
    public bool isFalling = false;
    private bool jump = false;
    private bool crouch = false;
    private bool hasDoubleJump = false;
    private bool runAnimationCheck = false;

    //Is holding a weapon
    private bool isHoldingWeapon = false;

    //Damage related
    private float knockbackStrength = 20f;

    private void Awake()
    {
        m_Rigidbody2 = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
    }
    private void FixedUpdate()
    {
        isHoldingWeapon = GetComponent<CombatScript>().IsHoldingGun();

        Animations();
        if (isHoldingWeapon && controller.m_Grounded)
        {
            controller.Move(horizontal * 0 * Time.fixedDeltaTime, crouch, jump, hasDoubleJump);
            //player is facing left...
            if (horizontal > 0 && !GetComponent<CharacterController2D>().m_FacingRight)
            {
                GetComponent<CharacterController2D>().Flip();
            }
            //player is facing right...
            else if (horizontal < 0 && GetComponent<CharacterController2D>().m_FacingRight)
            {
                // ... flip the player.
                GetComponent<CharacterController2D>().Flip();
            }
        }
        else
        {
            controller.Move(horizontal * (normalSpeed + runSpeed) * Time.fixedDeltaTime, crouch, jump, hasDoubleJump);
        }        
        jump = false;

        //Disable run if Idle
        if (horizontal == 0)
        {            
            isRunning = false;
        }
        //Run if moving and holding Run Button
        if (runAnimationCheck && horizontal != 0)
        {
            isRunning = true;
        }
        
        //Set falling animation if not grounded, not jumping and not attacking in the air...
        if (controller.m_Grounded == false && playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Player_Jump") == false && GetComponent<CombatScript>().isAirAttacking == false && GetComponent<CombatScript>().isReceivingAirDamage == false)
        {
            playerAnimator.SetBool("IsFalling", true);
            isFalling = true;
        }// ...disable it else.
        else
        {
            playerAnimator.SetBool("IsFalling", false);
            isFalling = false;
        }

    }

    //Get the input to move
    public void ButtonMove(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }

    //Get the run button
    public void ButtonRun(InputAction.CallbackContext context)
    {
        //Start running parameters and the button is pressed.
        isRunning = true;
        runSpeed = runSpeedValue;
        runAnimationCheck = true;

        //Stop running parameters if button is released.
        if (context.canceled)
        {
            isRunning = false;
            runAnimationCheck = false;
            runSpeed = 0f;
        }
    }

    //Get jump button
    public void ButtonJump(InputAction.CallbackContext context)
    {
        //Using performed to guarantee the jump is called only once
        if (context.performed)
        {            
            jump = true;
            playerAnimator.SetBool("IsJumping", true);
            playerAnimator.SetBool("IsGrounded", false);
        }
    }

    //Button crouch
    public void ButtonCrouch(InputAction.CallbackContext context)
    {
        //Start crouch parameters and the button is pressed
        isCrouching = true;
        if (controller.m_Grounded)
        {
            crouch = true;
        }
        //Stop crouch parameters if button is released.
        if (context.canceled)
        {
            crouch = false;
            isCrouching = false;
        }

    }

    //Animations
    public void Animations()
    {      
        //Set walk animation...
        playerAnimator.SetFloat("Speed", Mathf.Abs(horizontal * (normalSpeed + runSpeed)));

        // ... or running animation if button is pressed.
        if (isRunning)
        {
            playerAnimator.SetBool("IsRunning", true);
        } else
        {
            playerAnimator.SetBool("IsRunning", false);
        }
    }

    //Method to be called when the player lands on a "Grounded" surface
    public void OnLanding()
    {
        playerAnimator.SetBool("IsGrounded", true);
        playerAnimator.SetBool("IsJumping", false);
        playerAnimator.SetBool("IsAirAttacking", false);
        playerAnimator.SetBool("IsFalling", false);

        //Called when the player hit the ground if the player hold the "crouch button" in the air
        if (isCrouching)
        {
            crouch = true;
            OnCrouching(isCrouching);
        }
        //Called when the player hit the ground if the player hold the "Hold weapon button" in the air
        if (isHoldingWeapon)
        {
            GetComponent<CombatScript>().ActivateWeapon();
        }
    }

    //Called if the player doesnt have a ceiling above his head.
    public void OnCrouching(bool isCrouching)
    {
        playerAnimator.SetBool("IsCrouching", isCrouching);
    }
    //Called if the player collides with something...
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //... if the something is an enemy
        if(collision.gameObject.tag == "Enemy")
        {
            playerSprite.color = Color.red;
            StartCoroutine(HitEffect());

            Vector2 hitDirection = transform.position - collision.gameObject.transform.position;

            m_Rigidbody2.AddForce(hitDirection * knockbackStrength, ForceMode2D.Impulse);

            int damageReceived = collision.gameObject.GetComponent<EnemyScript>().damageDone;
            GetComponent<CombatScript>().TakeHealth(damageReceived);            
        }
        //... if the something is health
        if (collision.gameObject.tag == "Health")
        {
            //int healthGain = collision.gameObject.GetComponent<EnemyScript>().damageDone;
            //GetComponent<CombatScript>().GiveHealth(healthGain);
        }

    }

    IEnumerator HitEffect()
    {
        yield return new WaitForSeconds(.2f);
        playerSprite.color = Color.white;
    }
}
