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

    private float dobleUnit = 2f;

    private void Start()
    {
        playerControllerScript = FindObjectOfType<PlayerController>();
        UIManagerScript = FindObjectOfType<UIManager>();

        _audioSource = GetComponent<AudioSource>();
        _audioSource.mute = true;

        musicManager = GameObject.Find("Music");

        //If the bool is true the music at the start of the scene is off
        if (isIntro)
        {
            musicManager.SetActive(false);
        }

        //If the bool is true deactivates the ui visible components 
        if (isTriggered)
        {
            DeactivateUI();
        }

        //If the bool is true the dysplaytextFunction starts at the begining of the scene
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
        //If you input the right key and the text is being displayed 
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            if (isLineFinished && !playerControllerScript.canMove && canWrite)
            {
                //if there are more lines to display goes to the next line
                if (textToDisplay.Length > currentLine)
                {
                    _audioSource.mute = true;

                    currentLine++;
                    currentTextIndex++;
                    StartCoroutine(DisplayText());
                }
                //Exits the logic
                else
                {
                    skipIndicator.SetActive(false);

                    //Depending on whitch bool is active the exit logic changes

                    if (isIntro)
                    {
                        if (isEnd)
                        {
                            Invoke(nameof(ExitText), textExitTime /= dobleUnit);

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
                        Invoke(nameof(ExitText), textExitTime /= dobleUnit);

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
            //If the line isn't finished it finishes
            else
            {
                isLineFinished = true;
            }
        }

        //While line isn't finished and you are able to move and there is a text writting the player is able to move
        if (!isLineFinished && !mobileText && canWrite)
        {
            UIManagerScript.CanMove(false);
        }


        //When a gemobject becomes inactive starts to Dysplay the text
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

    //Loads a new scene
    void NextLevel()
    {
        UIManagerScript.NextLevel();
    }

    public IEnumerator DisplayText()
    {
        //activates all the display components (start)
        _audioSource.mute = false;

        ActivateUI();

        EnterText();
        //(end)

        bool isAddingRichTextTag = false;

        //Dysplays a string array to a ui text displaying letter by letter
        for (int i = 0; i < textToDisplay[currentTextIndex].Length; i++)
        {
            skipIndicator.SetActive(false);

            isLineFinished = false;

            //if the next letter to display is the scpecified character ('<') it means that there is a color comand executing in the string
            if(textToDisplay[currentTextIndex].Substring(0, i).EndsWith('<') || isAddingRichTextTag)
            {
                isAddingRichTextTag = true;

                currentText = textToDisplay[currentTextIndex].Substring(0, i);
                textUI.text = currentText;
                //if the next letter to display is the scpecified character ('<') it means that the color comand executing in the string is over
                if (textToDisplay[currentTextIndex].Substring(0, i).EndsWith('>'))
                {
                    isAddingRichTextTag = false;
                }
            }
            else
            {
                //Delay bettween characters
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

        //Unless is a mobileText the skipIndicateor is inactive
        if (!mobileText)
        {
            skipIndicator.SetActive(true);
            skipIndicator.GetComponent<Animator>().Play("indicator_flickering");
        }

        //At the end sets the line to be over and mutes the audiosource
        isLineFinished = true;

        _audioSource.mute = true;
    }

    //Deactivates all the displayable ui elements
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

    //Activates all the displayable ui elements
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

    //Sets how should all the parameters of the player be when entering the DisplayText function
    void EnterText()
    {
        if (!mobileText)
        {
            UIManagerScript.CanMove(false);
            Cursor.lockState = CursorLockMode.Confined;
        }

        UIManagerScript.canPause = false;
    }

    //Sets how should all the parameters of the player be when exiting the DisplayText function
    void ExitText()
    {
        this.gameObject.SetActive(false);

        UIManagerScript.canPause = true;
    }
}
