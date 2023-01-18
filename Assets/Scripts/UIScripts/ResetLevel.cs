using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider))]
public class ResetLevel : MonoBehaviour
{
    private ForceFieldShooter forceFieldShooterScript;
    private UIManager UIManagerScript;

    private Transform levelStart;
    private GameObject player;
    private GameObject cameraHolder;

    [HideInInspector] public Transform checkPoint;


    private void Start()
    {
        UIManagerScript = FindObjectOfType<UIManager>();
        forceFieldShooterScript = FindObjectOfType<ForceFieldShooter>();

        player = GameObject.Find("Player");
        cameraHolder = GameObject.Find("CameraHolder");
        levelStart = GameObject.Find("LevelStart").transform;

        player.transform.position = levelStart.transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(CheckPointRestart());
        }
    }

    public IEnumerator CheckPointRestart()
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
        yield return new WaitForSeconds(0.2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("Player"))
        {
            StartCoroutine(CheckPointRestart());
        }
    }
}
