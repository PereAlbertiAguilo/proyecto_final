using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslatePlatform : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float distance;

    private bool canMove = true;

    private void Update()
    {
        if (canMove)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);

            StartCoroutine(MoveForward());
        }
        else
        {
            transform.Translate(Vector3.back * Time.deltaTime * speed);

            StartCoroutine(MoveBack());
        }
    }

    IEnumerator MoveForward()
    {
        yield return new WaitForSeconds(distance);

        canMove = false;
    }

    IEnumerator MoveBack()
    {
        yield return new WaitForSeconds(distance);

        canMove = true;
    }
}
