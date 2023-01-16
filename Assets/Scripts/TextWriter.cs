using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextWriter : MonoBehaviour
{
    private UIManager UIManagerScript;

    [SerializeField] private TextMeshProUGUI textUI;

    [SerializeField] private GameObject skipIndicator;

    [SerializeField] private float delay;

    [SerializeField] private bool writeInStart;

    private int currentTextIndex = 0;
    private int currentLine = 1;

    [TextArea]
    [SerializeField] private string[] textToDisplay;
    private string currentText = "";

    private bool isLineFinished;
    private bool canFlicker;

    private void Start()
    {
        UIManagerScript = FindObjectOfType<UIManager>();

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
                    GetComponentInChildren<Animator>().Play("from_black");
                    Invoke(nameof(ExitText), 1);
                }
            }
            else
            {
                isLineFinished = true;
            }
        }
    }

    IEnumerator DisplayText()
    {
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
