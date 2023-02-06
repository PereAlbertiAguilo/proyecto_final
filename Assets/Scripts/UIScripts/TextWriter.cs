using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class TextWriter : MonoBehaviour
{
    private UIManager UIManagerScript;
    private PlayerController playerControllerScript;

    private AudioSource _audioSource;
    private GameObject musicManager;

    private Image image;

    private Color imageColor;

    [Header("Contents \n")]
    [SerializeField] private TextMeshProUGUI textUI;

    [SerializeField] private GameObject skipIndicator;
    [SerializeField] private GameObject textTrigger;

    [Header("Text Parameters \n")]
    [SerializeField] private float delay;
    [SerializeField] private float textExitTime;
    [SerializeField] private float speed;

    [Header("\n")]
    [SerializeField] private bool writeInStart;
    [SerializeField] private bool isIntro;
    [SerializeField] private bool isTriggered;
    [SerializeField] private bool mobileText;
    [SerializeField] private bool smoothIn;
    [SerializeField] private bool isEnd;
    private bool isLineFinished;
    private bool canFlicker;
    private bool canTrigger = true;
    [HideInInspector] public bool canWrite = true;

    [HideInInspector] public int currentLine = 1;
    [HideInInspector] public int currentTextIndex = 0;

    [Header("Text to display \n")]
    [TextArea]
    public string[] textToDisplay;
    private string currentText = "";


    private void Start()
    {
        playerControllerScript = FindObjectOfType<PlayerController>();
        UIManagerScript = FindObjectOfType<UIManager>();

        _audioSource = GetComponent<AudioSource>();
        _audioSource.mute = true;

        musicManager = GameObject.Find("Music");

        image = transform.Find("TextBackground").GetComponent<Image>();

        if (smoothIn)
        {
            imageColor = image.color;
            imageColor.a = 0;
            image.color = imageColor;
        }

        if (isIntro)
        {
            musicManager.SetActive(false);
        }

        if (isTriggered)
        {
            DeactivateUI();
        }

        if (writeInStart)
        {
            canWrite = true;
            StartCoroutine(DisplayText());
        }
        else
        {
            canWrite = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            if (isLineFinished && !playerControllerScript.canMove && canWrite)
            {
                if (textToDisplay.Length > currentLine)
                {
                    _audioSource.mute = true;

                    currentLine++;
                    currentTextIndex++;
                    StartCoroutine(DisplayText());
                }
                else
                {
                    skipIndicator.SetActive(false);

                    if (isIntro)
                    {
                        if (isEnd)
                        {
                            Invoke(nameof(ExitText), textExitTime /= 2);

                            Invoke(nameof(NextLevel), textExitTime);
                        }

                        GetComponentInChildren<Animator>().Play("from_black");

                        musicManager.SetActive(true);

                        UIManagerScript.CanMove(true);
                        Cursor.lockState = CursorLockMode.Locked;

                        Invoke(nameof(ExitText), textExitTime);
                    }
                    else if (isEnd)
                    {
                        Invoke(nameof(ExitText), textExitTime /= 2);

                        Invoke(nameof(NextLevel), textExitTime);
                    }
                    else
                    {
                        UIManagerScript.CanMove(true);
                        Cursor.lockState = CursorLockMode.Locked;

                        Invoke(nameof(ExitText), textExitTime);
                    }
                }
            }
            else
            {
                isLineFinished = true;
            }
        }

        if (smoothIn)
        {
            if (image.gameObject.activeInHierarchy)
            {
                image.color = imageColor;
                Mathf.Lerp(imageColor.a, 255, Time.deltaTime * speed);
                imageColor = image.color;
            }
        }

        if (!isLineFinished && !mobileText && canWrite)
        {
            UIManagerScript.CanMove(false);
        }


        if(textTrigger != null)
        {
            if (!textTrigger.activeInHierarchy && isTriggered && canTrigger)
            {
                canTrigger = false;
                canWrite = true;
                StartCoroutine(DisplayText());
            }

            if (textToDisplay.Length <= currentLine && mobileText && isLineFinished && !canTrigger)
            {
                skipIndicator.SetActive(false);

                Invoke(nameof(ExitText), textExitTime);
            }
        }
    }

    void NextLevel()
    {
        UIManagerScript.NextLevel();
    }

    public IEnumerator DisplayText()
    {
        _audioSource.mute = false;

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

        if (!mobileText)
        {
            skipIndicator.SetActive(true);
            skipIndicator.GetComponent<Animator>().Play("indicator_flickering");
        }

        isLineFinished = true;

        _audioSource.mute = true;
    }

    void DeactivateUI()
    {
        if (!writeInStart)
        {
            foreach (Transform child in transform)
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
                child.gameObject.SetActive(true);
            }
        }
    }

    void EnterText()
    {
        if (!mobileText)
        {
            UIManagerScript.CanMove(false);
            Cursor.lockState = CursorLockMode.Confined;
        }

        UIManagerScript.canPause = false;
    }

    void ExitText()
    {
        this.gameObject.SetActive(false);

        UIManagerScript.canPause = true;
    }
}
