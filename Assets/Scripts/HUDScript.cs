using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDScript : MonoBehaviour
{
    private ForceFieldShooter forceFieldShooter;
    private PlayerController playerController;

    [SerializeField] private Slider slider1;
    [SerializeField] private Slider slider2;
    [SerializeField] private Slider slider3;

    [SerializeField] private Toggle toggle;

    private float value;
    private bool timer;

    private void Start()
    {
        forceFieldShooter = FindObjectOfType<ForceFieldShooter>();
        playerController = FindObjectOfType<PlayerController>();

        slider1.maxValue = forceFieldShooter.maxInstances;

        slider2.maxValue = forceFieldShooter.cooldown;

        slider3.maxValue = forceFieldShooter.lifeTime;
    }

    private void Update()
    {
        slider1.value = slider1.maxValue - forceFieldShooter.currentInstance;

        if (Input.GetMouseButtonDown(1))
        {
            if (forceFieldShooter.forceFields.Count > 0)
            {
                timer = true;
            }
        }

        if (playerController.canSecondJump)
        {
            toggle.isOn = true;
        }
        else
        {
            toggle.isOn = false;
        }

        if (value > 0 && timer)
        {
            value -= Time.deltaTime;

            slider2.value = value;
        }
        else if (forceFieldShooter.canShoot)
        {
            value = forceFieldShooter.cooldown;
            timer = false;
        }

        if (!forceFieldShooter.isTimerOn)
        {
            slider3.gameObject.SetActive(false);
        }
        else
        {
            slider3.gameObject.SetActive(true);
            slider3.value = forceFieldShooter.currentLife;
        }   
    }
}
