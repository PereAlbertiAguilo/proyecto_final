using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string tutorial;
    public string actualLevel;

    [Header("Panels\n")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Buttons\n")]
    [SerializeField] private GameObject mainMenuFirstButton;
    [SerializeField] private GameObject optionsFirstButton;

    [Header("Parameters\n")]
    [SerializeField] private Volume postProcessingVolume;

    [SerializeField] private RenderTexture renderTexture;

    private float fov;

    private void Awake()
    {
        fov = PlayerPrefs.GetFloat("fov");
    }

    private void Start()
    {
        Screen.fullScreen = true;

        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Confined;

        if(actualLevel == "")
        {
            actualLevel = tutorial;
        }
    }
    public void CurrentButton(GameObject g)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(g);
    }

    #region MainMenuButtons
    public void NewGame()
    {
        SceneManager.LoadScene(tutorial);
    }

    public void Continue()
    {
        SceneManager.LoadScene(actualLevel);
    }

    public void Options()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
        CurrentButton(optionsFirstButton);
    }

    public void Quit()
    {
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }

        Application.Quit();
    }
    #endregion

    #region OptionMenuButtons

    public void PotProcesingToggle(bool b)
    {
        postProcessingVolume.enabled = b;
    }

    public void FullScreenToggle(bool b)
    {
        if (b)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }

    public void AntiAliasing(float f)
    {
        if (f == 0)
        {
            SetAntialiasing(1);
        }
        else if (f == 1)
        {
            SetAntialiasing(2);
        }
        else if(f == 2)
        {
            SetAntialiasing(4);
        }
        else if (f == 3)
        {
            SetAntialiasing(8);
        }
    }

    public void FieldOfView(float f)
    {
        fov = f;
        PlayerPrefs.SetFloat("fov", fov);
    }

    void SetAntialiasing(int i)
    {
        renderTexture.Release();

        renderTexture.antiAliasing = i;

        renderTexture.Create();
    }

    public void GoBack()
    {
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        CurrentButton(mainMenuFirstButton);
    }

    #endregion
}
