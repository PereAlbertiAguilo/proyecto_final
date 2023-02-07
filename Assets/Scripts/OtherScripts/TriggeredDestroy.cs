using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeredDestroy : MonoBehaviour
{
    [SerializeField] private GameObject Target;

    //When the player triggers with this gameobject destroys a given gameobject
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            Destroy(Target);
        }
    }
}
