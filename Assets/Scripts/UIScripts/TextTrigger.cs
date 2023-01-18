using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            gameObject.SetActive(false);
        }
    }
}
