using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    [SerializeField]
    private Transform firePoint;
    [SerializeField]
    private GameObject bulletPrefab;
    private GameObject bulletInstance;
    private float timeToDestroy = 0.8f;


    public void Shoot()
    {
        bulletInstance = (GameObject) Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Destroy(bulletInstance, timeToDestroy);
    }
}
