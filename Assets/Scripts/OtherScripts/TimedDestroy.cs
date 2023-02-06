using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
    [SerializeField] private float time;

    [SerializeField] private bool destroy = true;

    void Start()
    {
        if (destroy)
        {
            Destroy(gameObject, time);
        }
        else
        {
            Invoke(nameof(Deactivate), time);
        }
    }

    void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
