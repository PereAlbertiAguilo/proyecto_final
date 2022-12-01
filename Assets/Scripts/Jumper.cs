using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumper : MonoBehaviour
{
    [Header("Jumper Parameters\n")]
    public float force = 10f;
    public float delay = 0.2f;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name.Equals("Player"))
        {
            StartCoroutine(Delay(other));
        }
    }

    IEnumerator Delay(Collision other)
    {
        yield return new WaitForSeconds(delay);
        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();

        rb.AddForce(Vector3.up * force, ForceMode.Impulse);
    }
}
