using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string level;

    private void Start()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void Play()
    {
        SceneManager.LoadScene(level);
    }
}
