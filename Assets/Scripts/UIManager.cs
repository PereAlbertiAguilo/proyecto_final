using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    private bool isPaused;

    private PlayerController playerControllerScript;
    private ResetLevel resetLevelScript;

    [Header("Panels\n")]
    [SerializeField] private GameObject backgroundPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Buttons\n")]
    [SerializeField] private GameObject pauseFirstButton;
    [SerializeField] private GameObject optionsFirstButton;

    private void Start()
    {
        playerControllerScript = FindObjectOfType<PlayerController>();
        resetLevelScript = FindObjectOfType<ResetLevel>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        playerControllerScript.canMove = false;
        playerControllerScript.virtualCam.gameObject.SetActive(false);
        isPaused = true;
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0;
        backgroundPanel.SetActive(true);
        CurrentButton(pauseFirstButton);
    }

    public void Resume()
    {
        playerControllerScript.canMove = true;
        playerControllerScript.virtualCam.gameObject.SetActive(true);
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        backgroundPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Restart()
    {
        Resume();
        resetLevelScript.Restart();
    }

    public void CurrentButton(GameObject g)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(g);
    }
}
