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
    }

    private void Update()
    {
        GameObject[] g = GameObject.FindGameObjectsWithTag("SecondJump");

        slider.value = 5 - forceFieldShooter.currentInstance;
    }
}
