using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardEffect : MonoBehaviour
{
    private Camera targetCamera;

    private void Awake()
    {
        targetCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (targetCamera == null) return;

        transform.forward = targetCamera.transform.forward ;
    }
}

