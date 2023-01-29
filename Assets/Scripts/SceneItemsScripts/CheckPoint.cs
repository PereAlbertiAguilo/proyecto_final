using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private ResetLevel[] resetLevelScript;
    private ForceFieldShooter forceFieldShooterScript;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        forceFieldShooterScript = FindObjectOfType<ForceFieldShooter>();
        resetLevelScript = FindObjectsOfType<ResetLevel>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            foreach (ResetLevel rs in resetLevelScript)
            {
                if (rs.checkPoint != transform.GetChild(0))
                {
                    _audioSource.Play();
                }

                rs.checkPoint = transform.GetChild(0);
            }
        }
    }
}
