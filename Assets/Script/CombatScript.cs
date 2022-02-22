using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatScript : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 5;
    private int currentHealth;

    [SerializeField]
    private CharacterController2D controller;
    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private Animator playerAnimator;
    [SerializeField]
    private Animator weaponAnimator;
    [SerializeField]
    private Animator weaponArmAnimator;

    //Sword
    [SerializeField]
    private GameObject sword;
    [SerializeField]
    private GameObject swordArm;
    [SerializeField]
    private int swordDamage = 20;
    [SerializeField]
    private Transform swordAttackPoint;
    [SerializeField]
    private float swordAttackRange = 0.5f;
    public bool isAirAttacking = false;

    //Gun
    [SerializeField]
    private GameObject gun;
    [SerializeField]
    private GameObject gunArm;
    private bool isHoldingWeapon = false;


    //Attack rate
    [SerializeField]
    private float attackRate = 1f;
    private float nextAttackTime = 0f;


    //Enemy Layers
    [SerializeField]
    private LayerMask enemyLayers;

    //Damage
    public bool isReceivingAirDamage = false;
    [SerializeField]
    private float immunedTime = 2f;
    private float isImmune = 0f;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if(isImmune > 0)
        {
            isImmune -= Time.deltaTime;
        }

        if (GetComponent<PlayerMovimentScript>().isFalling == true)
        {
            gun.SetActive(false);
            gunArm.SetActive(false);
            playerAnimator.SetBool("GunAttack", false);
        }
        
    }


    //Called when the attack button is pressed
    public void ButtonAttack(InputAction.CallbackContext context)
    {

        if (context.performed)
        {            
            if (Time.time >= nextAttackTime)
            {
                //Attack Rate
                nextAttackTime = Time.time + 1f / attackRate;

                if (isHoldingWeapon && controller.m_Grounded)
                {
                    GetComponent<GunScript>().Shoot();
                }
                else
                {
                    //Enemy Identifier
                    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(swordAttackPoint.position, swordAttackRange, enemyLayers);
                    foreach (Collider2D enemy in hitEnemies)
                    {
                        //if an enemy is found, call the function that damage it
                        enemy.GetComponent<EnemyScript>().TakeDamage(swordDamage, transform.position);
                    }

                    //Animations
                    //Air sword attack if the player is not grounded
                    if (!controller.m_Grounded)
                    {
                        //Sword
                        sword.SetActive(true);
                        swordArm.SetActive(true);
                        isAirAttacking = true;
                        playerAnimator.SetBool("IsJumping", false);
                        playerAnimator.SetTrigger("AirAttack");
                        weaponAnimator.SetTrigger("Attack");
                        weaponArmAnimator.SetTrigger("Attack");
                    }
                    else
                    {
                        //Enable the sword attack if the player isnt holding a gun
                        if (!isHoldingWeapon)
                        {
                            //Sword
                            sword.SetActive(true);
                            swordArm.SetActive(true);
                            playerAnimator.SetTrigger("Attack");
                            weaponAnimator.SetTrigger("Attack");
                            weaponArmAnimator.SetTrigger("Attack");
                        }
                    }
                }                   
            } 
        } 
    }

    //Button to Activate the weapon
    public void ButtonHoldWeapon(InputAction.CallbackContext context)
    {
        //Enable the weapon if the player is grounded
        if (context.performed)
        {
            if (controller.m_Grounded)
            {
                ActivateWeapon();
                isHoldingWeapon = true;
            }
            else
            {
                isHoldingWeapon = true;
            }
        }
        //Disable the weapon if the button is released
        if (context.canceled)
        {
            gun.SetActive(false);
            gunArm.SetActive(false);
            playerAnimator.SetBool("GunAttack", false);
            isHoldingWeapon = false;
        }
    }

    //Activate the weapon
    public void ActivateWeapon()
    {
            gun.SetActive(true);
            gunArm.SetActive(true);
            playerAnimator.SetBool("GunAttack", true);
    }

    //Is holding gun check variable
    public bool IsHoldingGun()
    {
        return isHoldingWeapon;
    }

    public void AirDamageTaken()
    {
        isReceivingAirDamage = false;
    }

    //Called to Disable the Sword
    public void DisableSword()
    {
        isAirAttacking = false;
        sword.SetActive(false);
        swordArm.SetActive(false);
    }

    //To be called when the player takes damage
    public void TakeHealth(int damageTaken)
    {
        
        
        if(currentHealth > 0)
        {
            if (isImmune <= 0)
            {
                Debug.Log("Current health is: " + currentHealth);
                currentHealth -= damageTaken;
                isImmune = immunedTime;
                if (controller.m_Grounded)
                {
                    playerAnimator.SetTrigger("HitFromTheSide");
                }
                else
                {
                    Debug.Log("Damage from down");
                    playerAnimator.SetBool("IsJumping", false);
                    isReceivingAirDamage = true;
                    playerAnimator.SetTrigger("HitFromBottom");
                }
            }            
        }
            
        if(currentHealth == 0)
        {
            Debug.Log("Player is Dead");
            playerAnimator.SetBool("IsDead", true);
        }
    }

    //To be called when the player gains health
    public void GiveHealth(int healthGained)
    {
        if (currentHealth < maxHealth)
            currentHealth += healthGained;
    }

    //To reset the players life
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}
