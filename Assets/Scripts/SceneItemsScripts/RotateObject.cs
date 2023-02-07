using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private float speed;

    private void Update()
    {
        //Rotates the gameobject using the upward axis with a given spedd
        transform.Rotate(Vector3.up * Time.deltaTime * speed);
    }
}
