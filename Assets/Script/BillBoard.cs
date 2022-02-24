using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    private Camera cam;
    private Transform camPosition;
    private void Start()
    {
        cam = Camera.main;
        camPosition = cam.transform;
    }
    // Update is called once per frame
    void LateUpdate()
    {

        transform.LookAt(transform.position + camPosition.forward);
    }
}
