using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    //Enemy attributes
    [SerializeField]
    private int maxHealth = 100;
    private int currentHealth;
    [SerializeField]
    private HealthBarScript healthBar;
    private SpriteRenderer enemySprite;
    private Rigidbody2D enemyRb;    

    //Effects
    [SerializeField]
    private GameObject deathEffect;
    private GameObject deathEffectInstance;
    private float timeToDestroy = 0.2f;

    //Damage related
    public int damageDone = 1;
    private float knockbackStrength = 50f;


    void Start()
    {
        //Get the enemy initial components
        enemyRb = GetComponent<Rigidbody2D>();
        enemySprite = GetComponent<SpriteRenderer>();

        //Set the enemy inital health
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    //Called when the enemy takes damage
    public void TakeDamage(int damage, Vector3 playerPosition, bool changeDirection)
    {
        currentHealth -= damage;

        //Get the position of the hit and apply a knockback force
        Vector2 hitDirection = transform.position - playerPosition;
        enemyRb.AddForce(hitDirection * knockbackStrength, ForceMode2D.Impulse);

        //Change the direction if hit from behind
        if (changeDirection == true)
        {
            GetComponent<EnemyMovimentScript>().ChangeDirection();
        }

        healthBar.SetHealth(currentHealth);
        Debug.Log("Currente health is: " + currentHealth);

        //Effect when receiving damage
        enemySprite.color = Color.red;
        StartCoroutine(HitEffect());

        //Call death when life get to zero or below
        if (currentHealth <= 0)
        {
            Die();
        }        
    }

    //Damage effect
    IEnumerator HitEffect()
    {        
        yield return new WaitForSeconds(.2f);
        enemySprite.color = Color.white;
    }

    //Die effect
    public void Die()
    {
        //Call the effect at the enemy position
        deathEffectInstance = (GameObject) Instantiate(deathEffect, transform.position, transform.rotation);
        //Destroy the effect after some time
        Destroy(deathEffectInstance, timeToDestroy);
        //Destroy the enemy prefab
        Destroy(gameObject);
    }
}
