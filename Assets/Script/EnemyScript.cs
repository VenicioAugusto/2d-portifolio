using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 100;

    private int currentHealth;

    private SpriteRenderer enemySprite;

    private Rigidbody2D enemyRb;
    private float knockbackStrength = 50f;

    [SerializeField]
    private GameObject deathEffect;
    private GameObject deathEffectInstance;
    private float timeToDestroy = 0.2f;

    public int damageDone = 1;


    void Start()
    {
        enemyRb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        enemySprite = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage, Vector3 playerPosition, bool changeDirection)
    {
        currentHealth -= damage;

        Vector2 hitDirection = transform.position - playerPosition;

        enemyRb.AddForce(hitDirection * knockbackStrength, ForceMode2D.Impulse);

        if (changeDirection == true)
        {
            GetComponent<EnemyMovimentScript>().ChangeDirection();
        }
        

        Debug.Log("Currente health is: " + currentHealth);

        enemySprite.color = Color.red;
        StartCoroutine(HitEffect());

        if (currentHealth <= 0)
        {
            Die();
        }        
    }
    IEnumerator HitEffect()
    {        
        yield return new WaitForSeconds(.2f);
        enemySprite.color = Color.white;
    }
    public void Die()
    {
        deathEffectInstance = (GameObject) Instantiate(deathEffect, transform.position, transform.rotation);
        Destroy(deathEffectInstance, timeToDestroy);
        Destroy(gameObject);
    }
}
