using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslatePlatform : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private float lerpSpeed;

    private Vector3 startPos;
    private Vector3 targetStartPos;

    private void Start()
    {
        //stores the start pos of the gameobject and the target in 2 Vector3 variables
        if (target != null)
        {
            startPos = transform.position;
            targetStartPos = target.position;
        }
    }

    private void Update()
    {
        if(target != null)
        {
            //When the gameobejcts rounded magnitud position is equal to the target rounded magnitud position
            //the target positin toggles bettween the start pos of the gameobject or the start pos f it self
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

            //Lerps the position of the this transform to a target position
            transform.position = Vector3.Lerp(transform.position, target.position, lerpSpeed * Time.deltaTime);
        }
    }
}
