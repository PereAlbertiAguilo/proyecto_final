using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTrigger : MonoBehaviour
{
    //When the player triggers the collider of this gameobject sets true or false a given gameobject
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            gameObject.SetActive(false);
        }
    }
}
