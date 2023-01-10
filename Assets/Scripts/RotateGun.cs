using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGun : MonoBehaviour
{
    private GrapplingController grapplingController;

    private Quaternion startRot;

    private void Start()
    {
        grapplingController = FindObjectOfType<GrapplingController>();
    }

    void Update()
    {
        if (grapplingController.IsGrappling())
        {
            Vector3 pos = grapplingController.GetGrapplePoint();

            //transform.rotation = grapplingController.gunTip.rotation;

            //transform.LookAt(new Vector3(pos.x, pos.y - 2.5f, pos.z));
        }
        else
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
        }
    }
}
