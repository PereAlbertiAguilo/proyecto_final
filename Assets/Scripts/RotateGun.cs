using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGun : MonoBehaviour
{
    private GrapplingController grapplingController;

    private Quaternion desiredRotation;
    private float rotationSpeed = 5f;

    private void Start()
    {
        grapplingController = FindObjectOfType<GrapplingController>();
    }

    void Update()
    {
        if (!grapplingController.IsGrappling())
        {
            desiredRotation = transform.parent.rotation;
        }
        else
        {
            desiredRotation = Quaternion.LookRotation(grapplingController.GetGrapplePoint() - transform.position);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
    }
}
