using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider))]
public class ResetLevel : MonoBehaviour
{
    public Transform levelStart;

    private void Start()
    {
        GameObject.Find("Player").transform.position = levelStart.transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Test");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("Player"))
        {
            SceneManager.LoadScene("Test");
        }
    }
}
