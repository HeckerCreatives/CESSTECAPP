using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLookAtCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraPlayer;


    private void Update()
    {
        transform.LookAt(cameraPlayer);
    }
}
