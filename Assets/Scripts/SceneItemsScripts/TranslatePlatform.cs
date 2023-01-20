using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslatePlatform : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private float speed;

    private Vector3 startPos;
    private Vector3 targetStartPos;

    private void Start()
    {
        if (target != null)
        {
            startPos = transform.position;
            targetStartPos = target.position;
        }
        else
        {
            print("target has not been assigned");
        }
    }

    private void Update()
    {
        if(target != null)
        {
            if (Mathf.Round(transform.position.magnitude) == Mathf.Round(target.position.magnitude))
            {
                if (target.position == targetStartPos)
                {
                    target.position = startPos;
                }
                else
                {
                    target.position = targetStartPos;
                }
            }

            transform.position = Vector3.Lerp(transform.position, target.position, speed * Time.deltaTime);
        }
    }
}
