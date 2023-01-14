using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider))]
public class NextLevel : MonoBehaviour
{
    private UIManager UIManagerScript;

    [SerializeField] private string nextLevel;

    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;    
    }

    private void Start()
    {
        UIManagerScript = FindObjectOfType<UIManager>();
    }

    public void LoadLevel()
    {
        SceneManager.LoadScene(nextLevel);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            UIManagerScript.FinishLevel();
        }
    }
}