using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider))]
public class ResetLevel : MonoBehaviour
{
    public Transform levelStart;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("Player"))
        {
            SceneManager.LoadScene("Test");
        }
    }
}
