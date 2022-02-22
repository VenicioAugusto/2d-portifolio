using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordScript : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    public void SwordDisable()
    {
        player.GetComponent<CombatScript>().DisableSword();
    }
}
