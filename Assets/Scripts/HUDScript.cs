using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDScript : MonoBehaviour
{
    private ForceFieldShooter forceFieldShooter;

    [SerializeField] private Slider slider;

    private void Start()
    {
        forceFieldShooter = FindObjectOfType<ForceFieldShooter>();

        slider.maxValue = forceFieldShooter.maxInstances;
    }

    private void Update()
    {
        GameObject[] g = GameObject.FindGameObjectsWithTag("SecondJump");

        slider.value = slider.maxValue - forceFieldShooter.currentInstance;
    }
}
