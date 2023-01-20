using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private ResetLevel[] resetLevelScript;

    private void Start()
    {
        resetLevelScript = FindObjectsOfType<ResetLevel>();    
    }
    private void OnTriggerEnter(Collider other)
    {
        foreach(ResetLevel rs in resetLevelScript)
        {
            rs.checkPoint = transform.GetChild(0);
        }
    }
}
