using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    private PlayerController playerControllerScript;

    [Header("Jumper Parameters\n")]
    public float force = 10f;
    public float delay = 0.2f;

    private bool canJump = true;

    private void Start()
    {
        playerControllerScript = FindObjectOfType<PlayerController>();
    }

    //when the player collides with the collider of this gameobject gets the players rigidbody and adds a force with the gameobject upwards
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name.Equals("Player") && canJump)
        {
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();

            playerControllerScript.activateSpeedControl = false;

            playerControllerScript.PlayerSFX(playerControllerScript.sfxs[0], 1, 1.5f);
            rb.AddForce(transform.up * force, ForceMode.Impulse);
            StartCoroutine(Delay());
        }
    }

    //when called lets the player to only jump one time in a certain time
    IEnumerator Delay()
    {
        canJump = false;
        yield return new WaitForSeconds(delay);
        playerControllerScript.activateSpeedControl = true;
        canJump = true;
    }
}
