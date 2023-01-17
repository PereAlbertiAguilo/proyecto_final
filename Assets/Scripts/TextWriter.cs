using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextWriter : MonoBehaviour
{
    private UIManager UIManagerScript;

    [Header("Contents \n")]
    [SerializeField] private TextMeshProUGUI textUI;

    [SerializeField] private GameObject skipIndicator;
    private GameObject tutorial;

    [Header("Text Parameters \n")]
    [SerializeField] private float delay;
    [SerializeField] private float textExitTime;

    [SerializeField] private bool writeInStart;
    [SerializeField] private bool isIntro;
    private bool isLineFinished;
    private bool canFlicker;

    [HideInInspector] public int currentTextIndex = 0;
    private int currentLine = 1;

    [Header("Text to display \n")]
    [TextArea]
    public string[] textToDisplay;
    private string currentText = "";


    private void Start()
    {
        UIManagerScript = FindObjectOfType<UIManager>();

        if (isIntro)
        {
            tutorial = transform.parent.GetChild(0).gameObject;
        }

        if (writeInStart)
        {
            StartCoroutine(DisplayText());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (isLineFinished)
            {
                if (textToDisplay.Length > currentLine)
                {
                    currentLine++;
                    currentTextIndex++;
                    StartCoroutine(DisplayText());
                }
                else
                {
                    skipIndicator.SetActive(false);

                    if (isIntro)
                    {
                        GetComponentInChildren<Animator>().Play("from_black");

                        Invoke(nameof(DeactivateUI), textExitTime);
                        Invoke(nameof(EnterTutorial), textExitTime);
                    }
                    else
                    {
                        DeactivateUI();

                        Invoke(nameof(ExitText), textExitTime);
                    }
                }
            }
            else
            {
                isLineFinished = true;
            }
        }
    }

    public IEnumerator DisplayText()
    {
        ActivateUI();

        EnterText();

        bool isAddingRichTextTag = false;

        for (int i = 0; i < textToDisplay[currentTextIndex].Length; i++)
        {
            skipIndicator.SetActive(false);

            isLineFinished = false;

            if(textToDisplay[currentTextIndex].Substring(0, i).EndsWith('<') || isAddingRichTextTag)
            {
                isAddingRichTextTag = true;

                currentText = textToDisplay[currentTextIndex].Substring(0, i);
                textUI.text = currentText;

                if(textToDisplay[currentTextIndex].Substring(0, i).EndsWith('>'))
                {
                    isAddingRichTextTag = false;
                }
            }
            else
            {
                currentText = textToDisplay[currentTextIndex].Substring(0, i);
                textUI.text = currentText;
                yield return new WaitForSeconds(delay);
            }

            if (isLineFinished)
            {
                textUI.text = textToDisplay[currentTextIndex];
                i = textToDisplay[currentTextIndex].Length;
            }
        }

        skipIndicator.SetActive(true);
        skipIndicator.GetComponent<Animator>().Play("indicator_flickering");

        isLineFinished = true;
    }

    void DeactivateUI()
    {
        if (writeInStart)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            foreach(Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    void ActivateUI()
    {
        if (!writeInStart)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    void EnterTutorial()
    {
        tutorial.SetActive(true);
    }

    void EnterText()
    {
        UIManagerScript.CanMove(false);
        UIManagerScript.canPause = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void ExitText()
    {
        UIManagerScript.CanMove(true);
        UIManagerScript.canPause = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
