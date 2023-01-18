using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [SerializeField] private GameObject door;

    public float interactDist;

    public LayerMask whatIsInteractable;

    [HideInInspector] public bool canInteract;
    private bool isDoorOpened;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canInteract && !isDoorOpened)
        {
            StartCoroutine(OpenDoor());
        }
    }

    IEnumerator OpenDoor()
    {
        isDoorOpened = true;
        yield return new WaitForSeconds(0.8f);
        door.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        door.SetActive(true);
        yield return new WaitForSeconds(0.02f);
        door.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        door.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        door.SetActive(false);
        yield return new WaitForSeconds(0.4f);
        door.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        door.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        door.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        door.SetActive(false);
    }
}
