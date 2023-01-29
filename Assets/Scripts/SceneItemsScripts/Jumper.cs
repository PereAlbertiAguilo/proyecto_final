using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    [Header("Jumper Parameters\n")]
    public float force = 10f;
    public float delay = 0.2f;

    private bool canJump = true;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name.Equals("Player") && canJump)
        {
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            PlayerController playerControllerScript = other.gameObject.GetComponent<PlayerController>();

            playerControllerScript.PlayerSFX(playerControllerScript.sfxs[0], 1, 1.5f);
            rb.AddForce(Vector3.up * force, ForceMode.Impulse);
            //StartCoroutine(Delay(other));
        }
    }

    IEnumerator Delay(Collision other)
    {
        canJump = false;
        yield return new WaitForSeconds(delay);
        canJump = true;
    }
}
