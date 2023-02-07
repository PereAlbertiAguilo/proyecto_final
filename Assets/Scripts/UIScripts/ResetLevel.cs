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
    private GrapplingController grapplingControllerScript;

    private Transform levelStart;
    private GameObject player;
    private GameObject cameraHolder;

    [HideInInspector] public Transform checkPoint;


    private void Start()
    {
        forceFieldShooterScript = FindObjectOfType<ForceFieldShooter>();
        disableForceFieldsScript = FindObjectOfType<DisableForceFields>();
        grapplingControllerScript = FindObjectOfType<GrapplingController>();

        player = GameObject.Find("Player");
        cameraHolder = GameObject.Find("CameraHolder");
        levelStart = GameObject.Find("LevelStart").transform;

        //At the start of the scene sets the player pos to the start pos of the scene
        player.transform.position = levelStart.transform.position;
    }

    private void Update()
    {
        //When the player inputs a certain key Starts the CheckPointRestart function
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(CheckPointRestart());
        }
    }


    //Resets the player position, camera rotation, forcefields current instance, and grapple
    public IEnumerator CheckPointRestart()
    {
        //Creates and saves variables
        PlayerController playerControllerScript = player.GetComponent<PlayerController>();
        CinemachineVirtualCamera vCam = playerControllerScript.virtualCam.GetComponent<CinemachineVirtualCamera>();
        CinemachinePOV POV = vCam.GetCinemachineComponent<CinemachinePOV>();

        grapplingControllerScript.StopGrapple();

        forceFieldShooterScript.Reload();

        //Checks if the forcefields should be on or off depending on a boolena of the playercontroller script
        if(disableForceFieldsScript != null)
        {
            disableForceFieldsScript.HUD.SetActive(playerControllerScript.forceFieldsActive);
        }

        forceFieldShooterScript.enabled = playerControllerScript.forceFieldsActive;

        player.GetComponent<Rigidbody>().velocity = Vector3.zero;

        //When you start the level and use this function the checpoint is the level start pos if you trigger a checkpoint
        //the reset position uses the checkpoint pos as well as the rotation using the POV conponent of the virtual camera
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

    //When the player triggers this gameobjects collider Starts the CheckPointRestart function
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("Player"))
        {
            StartCoroutine(CheckPointRestart());
        }
    }
}
