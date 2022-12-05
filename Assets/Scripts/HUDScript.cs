using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDScript : MonoBehaviour
{
    private ForceFieldShooter forceFieldShooter;

    [SerializeField] private Slider slider1;

    [SerializeField] private Slider slider2;

    [SerializeField] private Slider slider3;

    private float value;

    private void Start()
    {
        forceFieldShooter = FindObjectOfType<ForceFieldShooter>();

        slider1.maxValue = forceFieldShooter.maxInstances;

        slider2.maxValue = forceFieldShooter.cooldown;

        slider3.maxValue = forceFieldShooter.lifeTime;
    }

    private void LateUpdate()
    {
        slider1.value = slider1.maxValue - forceFieldShooter.currentInstance;

        if (forceFieldShooter.forceFields.Count <= forceFieldShooter.maxInstances && Input.GetMouseButtonDown(1))
        {
            value = forceFieldShooter.cooldown;
        }

        if(value > 0)
        {
            value -= Time.deltaTime;

            slider2.value = value;
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
