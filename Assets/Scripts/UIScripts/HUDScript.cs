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

        slider1.maxValue = forceFieldShooterScript.maxInstances;

        slider2.maxValue = forceFieldShooterScript.cooldown;

        slider3.maxValue = forceFieldShooterScript.lifeTime;
    }

    private void Update()
    {
        slider1.value = slider1.maxValue - forceFieldShooterScript.currentInstance;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.JoystickButton5))
        {
            if (forceFieldShooterScript.currentInstance != forceFieldShooterScript.maxInstances)
            {
                timer = true;
            }
        }

        toggle.isOn = playerControllerScript.canSecondJump;

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

        if (!forceFieldShooterScript.isTimerOn)
        {
            slider3.gameObject.SetActive(false);
        }
        else
        {
            slider3.gameObject.SetActive(true);
            slider3.value = forceFieldShooterScript.currentLife;
        }
        
        if(doorOpenerScript != null)
        {
            interactablePopUp.SetActive(doorOpenerScript.canInteract);
        }
        else
        {
            interactablePopUp.SetActive(false);
        }
    }
}
