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
    private void Update()
    {
        if(PCScript.canMove)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * sensX * Time.smoothDeltaTime;
            float mouseY = Input.GetAxisRaw("Mouse Y") * sensY * Time.smoothDeltaTime;

            /*
            print(mouseY);

            orientation.rotation *= Quaternion.Euler(0f, mouseX, 0f);
            transform.rotation *= Quaternion.Euler(-mouseY, 0f, 0f);
            */
            yRotation += mouseX;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
            /*
            
            */
        }
    }
}
