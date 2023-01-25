using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), (typeof(AudioSource)))]
public class DisableForceFields : MonoBehaviour
{
    public bool enable;
    public bool disable;

    public GameObject HUD;

    private ForceFieldShooter forceFieldShooterScript;

    private AudioSource _audioSource;

    private void Start()
    {
        forceFieldShooterScript = FindObjectOfType<ForceFieldShooter>();

        _audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            _audioSource.Play();

            if (enable)
            {
                HUD.SetActive(true);
                forceFieldShooterScript.enabled = true;
            }
            else if (disable)
            {
                HUD.SetActive(false);
                forceFieldShooterScript.enabled = false;
                forceFieldShooterScript.Reload();
            }
        }
    }
}
