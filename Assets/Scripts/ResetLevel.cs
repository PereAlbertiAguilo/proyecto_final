using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(MeshCollider))]
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
            Invoke(nameof(Restart), 0.2f);
        }
    }

    public void Restart()
    {
        forceFieldShooterScript.Reload();

        player.GetComponent<Rigidbody>().velocity = Vector3.zero;

        if (checkPoint == null)
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
            Invoke(nameof(Restart), 0.2f);
        }
    }
}
