using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DisableForceFields : MonoBehaviour
{
    public bool enable;
    public bool disable;

    public GameObject HUD;

    private ForceFieldShooter forceFieldShooterScript;

    private void Start()
    {
        forceFieldShooterScript = FindObjectOfType<ForceFieldShooter>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            if (enable)
            {
                HUD.SetActive(true);
                forceFieldShooterScript.enabled = true;
            }
            else if (disable)
            {
                HUD.SetActive(false);
                forceFieldShooterScript.enabled = false;
            }
        }
    }
}
