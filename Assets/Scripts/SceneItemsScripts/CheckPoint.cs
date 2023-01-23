using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private ResetLevel[] resetLevelScript;
    private ForceFieldShooter forceFieldShooterScript;

    [HideInInspector] public bool forceFieldsActive;

    private void Start()
    {
        forceFieldShooterScript = FindObjectOfType<ForceFieldShooter>();
        resetLevelScript = FindObjectsOfType<ResetLevel>();    
    }

    private void Update()
    {
        if (forceFieldShooterScript.enabled == true)
        {
            forceFieldsActive = true;
        }
        else
        {
            forceFieldsActive = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            foreach (ResetLevel rs in resetLevelScript)
            {
                rs.checkPoint = transform.GetChild(0);
            }
        }
    }
}
