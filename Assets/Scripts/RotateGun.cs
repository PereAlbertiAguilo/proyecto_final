using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGun : MonoBehaviour
{
    private GrapplingController grapplingController;

    private void Start()
    {
        grapplingController = FindObjectOfType<GrapplingController>();
    }

    void Update()
    {
        if (grapplingController.IsGrappling())
        {
            Vector3 pos = grapplingController.GetGrapplePoint();
            transform.LookAt(new Vector3(pos.x, pos.y - 2.1f, pos.z));
        }
    }
}
