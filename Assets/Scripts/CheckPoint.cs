using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private ResetLevel resetLevelScript;

    private void Start()
    {
        resetLevelScript = FindObjectOfType<ResetLevel>();    
    }
    private void OnTriggerEnter(Collider other)
    {
        resetLevelScript.checkPoint = transform.GetChild(0);
    }
}
