using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField]
    private float bulletSpeed = 20f;
    [SerializeField]
    private Rigidbody2D bulletRb;

    [SerializeField]
    private int bulledDamage = 40;

    [SerializeField]
    private GameObject impactEffect;
    private GameObject bulletEffect;
    private float timeToDestroy = 0.2f;

    void Start()
    {
        bulletRb.velocity = transform.right * bulletSpeed;        
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        EnemyScript enemy = hitInfo.GetComponent<EnemyScript>();
        if(enemy != null)
        {
            enemy.TakeDamage(bulledDamage, transform.position);
        }
        bulletEffect = (GameObject) Instantiate(impactEffect, transform.position, transform.rotation);
        Destroy(bulletEffect, timeToDestroy);
        Destroy(gameObject);
    }

}