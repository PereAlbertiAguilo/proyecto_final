using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider))]
public class ResetLevel : MonoBehaviour
{
    private ForceFieldShooter forceFieldShooterScript;

    [SerializeField] private Transform levelStart;
    [SerializeField] private GameObject player;

    [HideInInspector] public Transform checkPoint;

    private void Start()
    {
        forceFieldShooterScript = FindObjectOfType<ForceFieldShooter>();

        player.transform.position = levelStart.transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }

    public void Restart()
    {
        forceFieldShooterScript.Reload();

        if(checkPoint == null)
        {
            player.transform.position = levelStart.position;
        }
        else
        {
            player.transform.position = checkPoint.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("Player"))
        {
            Restart();
        }
    }
}
