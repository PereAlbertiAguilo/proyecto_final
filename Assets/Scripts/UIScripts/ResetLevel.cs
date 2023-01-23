using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

[RequireComponent(typeof(BoxCollider))]
public class ResetLevel : MonoBehaviour
{
    private ForceFieldShooter forceFieldShooterScript;
    private DisableForceFields disableForceFieldsScript;
    private UIManager UIManagerScript;

    private Transform levelStart;
    private GameObject player;
    private GameObject cameraHolder;

    [HideInInspector] public Transform checkPoint;


    private void Start()
    {
        UIManagerScript = FindObjectOfType<UIManager>();
        forceFieldShooterScript = FindObjectOfType<ForceFieldShooter>();
        disableForceFieldsScript = FindObjectOfType<DisableForceFields>();

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
        PlayerController playerControllerScript = player.GetComponent<PlayerController>();
        CinemachineVirtualCamera vCam = playerControllerScript.virtualCam.GetComponent<CinemachineVirtualCamera>();
        CinemachinePOV POV = vCam.GetCinemachineComponent<CinemachinePOV>();

        forceFieldShooterScript.Reload();


        
        disableForceFieldsScript.HUD.SetActive(forceFieldShooterScript.enabled);

        forceFieldShooterScript.enabled = forceFieldShooterScript.enabled;  

        player.GetComponent<Rigidbody>().velocity = Vector3.zero;

        if (checkPoint == null)
        {
            POV.m_HorizontalAxis.Value = levelStart.transform.eulerAngles.y;
            POV.m_VerticalAxis.Value = 0;

            player.transform.position = levelStart.position;
        }
        else
        {
            POV.m_HorizontalAxis.Value = checkPoint.transform.eulerAngles.y;
            POV.m_VerticalAxis.Value = 0;

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
