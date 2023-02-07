using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDScript : MonoBehaviour
{
    private ForceFieldShooter forceFieldShooterScript;
    private PlayerController playerControllerScript;
    private DoorOpener doorOpenerScript;

    [SerializeField] private Slider slider1;
    [SerializeField] private Slider slider2;
    [SerializeField] private Slider slider3;

    [SerializeField] private Toggle toggle;

    [SerializeField] private GameObject interactablePopUp;

    private float value;
    private bool timer;

    private void Start()
    {
        doorOpenerScript = FindObjectOfType<DoorOpener>();
        forceFieldShooterScript = FindObjectOfType<ForceFieldShooter>();
        playerControllerScript = FindObjectOfType<PlayerController>();

        //Sets the max values to their corresponding values
        slider1.maxValue = forceFieldShooterScript.maxInstances;

        slider2.maxValue = forceFieldShooterScript.cooldown;

        slider3.maxValue = forceFieldShooterScript.lifeTime;
    }

    private void Update()
    {
        //Updates the value of a slider substracting a int that changes in the forcefieldshooter script
        slider1.value = slider1.maxValue - forceFieldShooterScript.currentInstance;

        //When you the current instance is lower than the max instances and inputs the right key starts a timer
        //that lowers a slider value in time from a given float "cooldown" to 0, when it reaches 0 the timer resets
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.JoystickButton5))
        {
            if (forceFieldShooterScript.currentInstance != forceFieldShooterScript.maxInstances)
            {
                timer = true;
            }
        }

        if (value > 0 && timer)
        {
            value -= Time.deltaTime;

            slider2.value = value;
        }
        else if (forceFieldShooterScript.canShoot)
        {
            value = forceFieldShooterScript.cooldown;
            timer = false;
        }

        //A toggle boolean is determined by a boolenan in the playercontroller script
        toggle.isOn = playerControllerScript.canSecondJump;

        //Enables a gameobject depending on if a boolean of the forcefieldshooter script
        if (!forceFieldShooterScript.isTimerOn)
        {
            slider3.gameObject.SetActive(false);
        }
        else
        {
            slider3.gameObject.SetActive(true);
            slider3.value = forceFieldShooterScript.currentLife;
        }

        //If there is a dooropener script in the scene a popup gameobjects toggles bettween true or false depending on a boolean in the dooropener script script
        if (doorOpenerScript != null)
        {
            interactablePopUp.SetActive(doorOpenerScript.canInteract);
        }
        else
        {
            interactablePopUp.SetActive(false);
        }
    }
}
