using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
    [SerializeField] private float time;

    [SerializeField] private bool destroy = true;

    //At the start of the scene the gameobject that has this script destroys or deactivates itself in a certain time
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
