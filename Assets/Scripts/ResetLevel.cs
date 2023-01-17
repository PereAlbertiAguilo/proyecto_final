using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(MeshCollider))]
public class ResetLevel : MonoBehaviour
{
    private ForceFieldShooter forceFieldShooterScript;
    private UIManager UIManagerScript;

    [SerializeField] private Transform levelStart;
    private GameObject player;
    private GameObject cameraHolder;

    [HideInInspector] public Transform checkPoint;


    private void Start()
    {
        UIManagerScript = FindObjectOfType<UIManager>();
        forceFieldShooterScript = FindObjectOfType<ForceFieldShooter>();

        player = GameObject.Find("Player");
        cameraHolder = GameObject.Find("CameraHolder");

        player.transform.position = levelStart.transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //Invoke(nameof(CheckPointRestart), 0.2f);
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
            player.transform.rotation = Quaternion.Euler(0, levelStart.rotation.y, 0);
            cameraHolder.transform.rotation = Quaternion.Euler(0, levelStart.rotation.y, 0);
        }
        else
        {
            player.transform.position = checkPoint.position;
            player.transform.rotation = Quaternion.Euler(0, checkPoint.rotation.y, 0);
            cameraHolder.transform.rotation = Quaternion.Euler(0, checkPoint.rotation.y, 0);
        }
        yield return new WaitForSeconds(0.2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("Player"))
        {
            //Invoke(nameof(CheckPointRestart), 0.2f);
            StartCoroutine(CheckPointRestart());
        }
    }
}
