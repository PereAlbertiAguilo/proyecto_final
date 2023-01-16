using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private bool isPaused;
    [HideInInspector] public bool canPause;

    private PlayerController playerControllerScript;
    private ResetLevel resetLevelScript;
    private NextLevel nextLevelScript;

    [Header("Panels\n")]
    [SerializeField] private GameObject backgroundPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject nextLevelPanel;

    [Header("Buttons\n")]
    [SerializeField] private GameObject pauseFirstButton;
    [SerializeField] private GameObject optionsFirstButton;
    [SerializeField] private GameObject nextLevelFirstButton;

    private void Start()
    {
        playerControllerScript = FindObjectOfType<PlayerController>();
        resetLevelScript = FindObjectOfType<ResetLevel>();
        nextLevelScript = FindObjectOfType<NextLevel>();

        Resume();

        canPause = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && canPause)
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

    public void CanMove(bool b)
    {
        playerControllerScript.canMove = b;
        playerControllerScript.virtualCam.gameObject.SetActive(b);
    }

    public void Pause()
    {
        CanMove(false);
        isPaused = true;
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0;
        backgroundPanel.SetActive(true);
        pausePanel.SetActive(true);
        CurrentButton(pauseFirstButton);
    }

    public void Resume()
    {
        CanMove(true);
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        backgroundPanel.SetActive(false);
        pausePanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void FinishLevel()
    {
        canPause = false;
        CanMove(false);
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0;
        backgroundPanel.SetActive(true);
        nextLevelPanel.SetActive(true);
        CurrentButton(nextLevelFirstButton);
    }

    public void NextLevel()
    {
        Resume();
        nextLevelScript.LoadLevel();
    }

    public void CheckPointRestart()
    {
        Resume();
        resetLevelScript.CheckPointRestart();
    }

    public void ResetLevel()
    {
        Resume();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void CurrentButton(GameObject g)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(g);
    }
}
