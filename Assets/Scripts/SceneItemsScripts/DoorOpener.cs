using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject nextDoorOpener;

    public float interactDist;

    public LayerMask whatIsInteractable;

    [HideInInspector] public bool canInteract;
    private bool isDoorOpened;

    private AudioSource _audioSource;
    [SerializeField] private AudioClip[] sfxs;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        isDoorOpened = false;
    }

    private void Update()
    {
        //When you input the right key sets 2 gameobjects to false if you can interact with this gameobejct
        if (Input.GetKeyDown(KeyCode.E) || (Input.GetKeyDown(KeyCode.JoystickButton2)))
        {
            if (canInteract && !isDoorOpened)
            {
                if(_audioSource != null)
                {
                    _audioSource.PlayOneShot(sfxs[0]);
                }

                transform.parent.Find("Trigger").gameObject.SetActive(false);

                StartCoroutine(OpenDoor());
            }
        }
    }

    //Start a coroutine that deactivate a gameobject with a small animation
    IEnumerator OpenDoor()
    {
        isDoorOpened = true;
        yield return new WaitForSeconds(0.8f);
        door.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        if (_audioSource != null)
        {
            _audioSource.PlayOneShot(sfxs[1]);
        }
        door.SetActive(true);
        yield return new WaitForSeconds(0.02f);
        door.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        door.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        door.SetActive(false);
        yield return new WaitForSeconds(0.4f);
        door.SetActive(true);
        if (_audioSource != null)
        {
            _audioSource.PlayOneShot(sfxs[1]);
        }
        yield return new WaitForSeconds(0.2f);
        door.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        door.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        door.SetActive(false);

        if(nextDoorOpener != null)
        {
            nextDoorOpener.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //If the player triggers this gameobject collider starts a coroutine
        if (other.tag.Equals("Player"))
        {
            StartCoroutine(OpenDoor());
        }
    }
}
