using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ResetLevel : MonoBehaviour
{
    public Transform levelStart;

    private CameraMovement cameraMovement;

    private void Start()
    {
        cameraMovement = FindObjectOfType<CameraMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("Player"))
        {
            other.transform.position = levelStart.position;
            cameraMovement.gameObject.transform.rotation = levelStart.transform.rotation;
        }
    }
}
