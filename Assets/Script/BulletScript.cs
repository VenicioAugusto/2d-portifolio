using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    //Bullet attributes
    [SerializeField]
    private float bulletSpeed = 20f;
    [SerializeField]
    private Rigidbody2D bulletRb;
    [SerializeField]
    private int bulledDamage = 40;

    //Effects
    [SerializeField]
    private GameObject impactEffect;
    private GameObject bulletEffect;
    private float timeToDestroy = 0.2f;

    //Enemy related
    private bool changeEnemyDirection;

    void Start()
    {
        //Set the bullet speed one it is created
        bulletRb.velocity = transform.right * bulletSpeed;        
    }

    //Called when the bullet hit something
    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        
        //Try to get the EnemyScript...
        EnemyScript enemy = hitInfo.GetComponentInParent<EnemyScript>();

        //...If the script is found
        if (enemy != null)
        {
            //Change the enemy direction if hit from the back
            if (hitInfo.name == "EnemyBack")
            {
                changeEnemyDirection = true;
            }
            else
            {
                changeEnemyDirection = false;
            }
            //Cause damage if hit an enemy            
            enemy.TakeDamage(bulledDamage, transform.position, changeEnemyDirection);
        }
        

        //Instantiate the bullet effect
        bulletEffect = (GameObject)Instantiate(impactEffect, transform.position, transform.rotation);
        //Destroy the bullet effect after some time
        Destroy(bulletEffect, timeToDestroy);
        //Destroy the bullet prefab
        Destroy(gameObject);
    }

}
