using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public Transform orientation;

    [Header("Sensibility\n")]

    public float sensX;
    public float sensY;

    float xRotation;
    float yRotation;

    private PlayerController PCScript;

    private void Start()
    {
        PCScript = FindObjectOfType<PlayerController>();
    }
    private void LateUpdate()
    {
        if (PCScript.canMove)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * sensX * Time.smoothDeltaTime;
            float mouseY = Input.GetAxisRaw("Mouse Y") * sensY * Time.smoothDeltaTime;

            yRotation += mouseX;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(xRotation, transform.rotation.eulerAngles.y, 0f);
            orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);

            /*
            if(transform.localEulerAngles.x >= 90)
            {
                transform.rotation = Quaternion.Euler(90, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            }

            if (transform.localEulerAngles.x <= -90)
            {
                transform.rotation = Quaternion.Euler(-90, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            }
            */
            //transform.rotation = Quaternion.Euler(Mathf.Clamp(transform.localEulerAngles.x, -90, 90), transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            /*
            float mouseX = Input.GetAxisRaw("Mouse X") * sensX;
            float mouseY = Input.GetAxisRaw("Mouse Y") * sensY;

            orientation.rotation *= Quaternion.Euler(0f, mouseX, 0f);
            transform.rotation *= Quaternion.Euler(-mouseY, 0f, 0f);
            */
            /*
            
            /*
            */
        }
    }
}
