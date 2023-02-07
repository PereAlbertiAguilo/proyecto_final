using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class MainMenu : MonoBehaviour
{
    private int XboxOneController = 0;

    [SerializeField] private string tutorial;
    public string actualLevel;

    [Header("UI\n")]
    [SerializeField] private Slider fovSlider;
    [SerializeField] private Slider aaSlider;
    [SerializeField] private Slider lggSlider;
    [SerializeField] private Slider sXSlider;
    [SerializeField] private Slider sYSlider;
    [SerializeField] private Slider mSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("\n")]
    [SerializeField] private Toggle fsToggle;
    [SerializeField] private Toggle ppToggle;

    [Header("\n")]
    [SerializeField] private TMP_Dropdown qDropdown;

    [Header("\n")]
    [SerializeField] private TextMeshProUGUI confirmMesage;

    [Header("\n")]
    [SerializeField] private GameObject yesExit;
    [SerializeField] private GameObject yesNewGame;

    [Header("Active Panels\n")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject confirmPanel;

    [Header("Buttons Seleted\n")]
    [SerializeField] private GameObject mainMenuFirstButton;
    [SerializeField] private GameObject optionsFirstButton;
    [SerializeField] private GameObject confirmFirstButton;

    [Header("Components\n")]
    [SerializeField] private Volume postProcessingVolume;
    [SerializeField] private AudioMixer musicAudioMixer;
    [SerializeField] private AudioMixer SFXAudioMixer;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioClip[] sfxs;
    [SerializeField] private Animator blackPanelAnimator;

    LiftGammaGain liftGammaGain;
    Bloom bloom;
    Vignette vignette;
    
    [SerializeField] private RenderTexture renderTexture;

    [SerializeField] private GameObject alpha02;
    [SerializeField] private Transform target;
    private Vector3 startPos;
    private Vector3 targetStartPos;
    private bool move;

    private float lggValue;
    private float fovValue;
    private float aaValue;
    private float sensX;
    private float sensY;
    private float mVolume;
    private float sfxVolume;

    private int qLevel;
    
    private bool fsIsOn;
    private bool ppIsOn;

    private void Awake()
    {
        //Sets all the ui values to the saved playerpref key or its default value
        lggValue = CheckFloatKey("gamma", lggSlider.value);
        fovValue = CheckFloatKey("fov", fovSlider.value);
        aaValue = CheckFloatKey("aaValue", aaSlider.value);
        sensX = CheckFloatKey("sensX", sXSlider.value);
        sensY = CheckFloatKey("sensY", sYSlider.value);
        mVolume = CheckFloatKey("mVolume", GetAudioMixerValue(musicAudioMixer, "MusicVolume", mVolume));
        sfxVolume = CheckFloatKey("sfxVolume", GetAudioMixerValue(SFXAudioMixer, "SFXVolume", sfxVolume));

        qLevel = CheckIntKey("qLevel", 0);

        ppIsOn = CheckBoolKey("ppBool");
        fsIsOn = CheckBoolKey("fsBool");

        actualLevel = CheckStringKey("currentScene", tutorial);
    }

    private void Start()
    {
        postProcessingVolume = GameObject.Find("PostProcesing").GetComponent<Volume>();
        sfxAudioSource = GameObject.Find("SFX").GetComponent<AudioSource>();

        startPos = alpha02.transform.position;
        targetStartPos = target.position;

        //If a playerpref key exists sets toggles a gameobject bettween true or false
        if (!PlayerPrefs.HasKey("currentScene"))
        {
            mainMenuPanel.transform.GetChild(0).Find("Continue").gameObject.SetActive(false);
        }
        else
        {
            mainMenuPanel.transform.GetChild(0).Find("Continue").gameObject.SetActive(true);
        }

        Screen.fullScreen = fsIsOn;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Confined;

        //Calls all the updated parameters functions (start)
        PostPorcessingParameters();

        sfxAudioSource.mute = true;
        UpdateUI();
        UpdateScene();
        sfxAudioSource.mute = false;
        //(end)
    }

    private void Update()
    {
        //Determinates if there is an xbox controller conected
        string[] names = Input.GetJoystickNames();
        
        for (int i = 0; i < names.Length; i++)
        {
            if (names[i].Length == 33)
            {
                print(names[i].Length);

                XboxOneController = 1;
            }
            else
            {
                print(names[i].Length);

                XboxOneController = 0;
            }
        }

        //When there is a slider selected to exit this selection with a controller useing a specific input
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            if (EventSystem.current.currentSelectedGameObject.TryGetComponent(out Slider s))
            {
                if (Input.GetKeyDown(KeyCode.JoystickButton0))
                {
                    EventSystem.current.SetSelectedGameObject(s.transform.parent.gameObject);
                }
            }
        }

        //If input a specific key returns to the main menu panel if its not active at the time
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            if (!mainMenuPanel.activeInHierarchy)
            {
                GoBack();
            }
        }

        //When you connect an xbox controller and you input any key sets the current selected gameobject depending on whitch panel is active and changes the visibility of the cursor
        if (Input.anyKeyDown)
        {
            if(XboxOneController == 1 && EventSystem.current.currentSelectedGameObject == null)
            {
                Cursor.visible = false;

                if (mainMenuPanel.activeInHierarchy)
                {
                    CurrentButton(mainMenuFirstButton);
                }
                else if (optionsPanel.activeInHierarchy)
                {
                    CurrentButton(optionsFirstButton);
                }
                else if (confirmPanel.activeInHierarchy)
                {
                    CurrentButton(confirmFirstButton);
                }
            }
            else if (XboxOneController == 0 && EventSystem.current.currentSelectedGameObject == null)
            {
                Cursor.visible = true;
            }
        }

        //Moves an object of the scene bettween to positions (Same logic as the script translate platform)
        if (!move)
        {
            target.position = startPos;
        }
        else
        {
            target.position = targetStartPos;
        }

        alpha02.transform.position = Vector3.Lerp(alpha02.transform.position, target.position, 2 * Time.deltaTime);
    }

    //Gets the current volume of an AudioMixer
    float GetAudioMixerValue(AudioMixer am, string s, float f)
    {
        am.GetFloat(s, out f);

        return Mathf.Log10(f) * 20;
    }

    //Plays an audioclip with a random picth bettween 2 values
    public void PlayerSFX(AudioClip ac, float f1, float f2)
    {
        float randomIndex = Random.Range(f1, f2);

        sfxAudioSource.pitch = randomIndex;

        sfxAudioSource.PlayOneShot(ac);
    }

    //Gets the paramters of a postprocesingVolume
    void PostPorcessingParameters()
    {
        if (!postProcessingVolume.profile.TryGet(out liftGammaGain))
        {
            throw new System.NullReferenceException(nameof(liftGammaGain));
        }
        if (!postProcessingVolume.profile.TryGet(out bloom))
        {
            throw new System.NullReferenceException(nameof(bloom));
        }
        if (!postProcessingVolume.profile.TryGet(out vignette))
        {
            throw new System.NullReferenceException(nameof(vignette));
        }
    }

    //Checks if float, int, string and bool Playerprefs have keys
    #region CheckPlayerPrefs
    float CheckFloatKey(string s, float f)
    {
        if (PlayerPrefs.HasKey(s))
        {
            return PlayerPrefs.GetFloat(s);
        }
        else
        {
            return f;
        }
    }

    int CheckIntKey(string s, int i)
    {
        if (PlayerPrefs.HasKey(s))
        {
            return PlayerPrefs.GetInt(s);
        }
        else
        {
            return i;
        }
    }

    string CheckStringKey(string s, string altS)
    {
        if (PlayerPrefs.HasKey(s))
        {
            return PlayerPrefs.GetString(s);
        }
        else
        {
            return altS;
        }
    }

    bool CheckBoolKey(string s)
    {
        if (PlayerPrefs.HasKey(s))
        {
            if (PlayerPrefs.GetInt(s) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }
    #endregion

    //Updates al the ui elements with their corresponding values saved with Playerprefs
    void UpdateUI()
    {
        lggSlider.value = lggValue;
        aaSlider.value = aaValue;
        fovSlider.value = fovValue;
        sXSlider.value = sensX;
        sYSlider.value = sensY;
        mSlider.value = mVolume;
        sfxSlider.value = sfxVolume;

        qDropdown.value = qLevel;

        fsToggle.isOn = fsIsOn;
        ppToggle.isOn = ppIsOn;
    }

    //Activates all the ui functions with their corresponding values tu update them at the start of the scene
    void UpdateScene()
    {
        PostProcesingToggle(ppIsOn);
        FullScreenToggle(fsIsOn);

        Quality(qLevel);

        AntiAliasing(aaValue);
        GammaGain(lggValue);
        FieldOfView(fovValue);
        SensX(sensX);
        SensY(sensY);
    }

    //If there is an xbox controller conected chnages the current selected gameobject by a given gameobject
    public void CurrentButton(GameObject g)
    {
        if (XboxOneController == 1)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(g);
        }
    }

    //Sets a active a panel and changes a text by a given string
    public void ConfirmMessage(string s)
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        confirmMesage.text = s;

        confirmPanel.SetActive(true);
        CurrentButton(confirmFirstButton);
    }

    //Depending on a give bool sets a gameobject true or false
    public void ConfirmButtons(bool b)
    {
        if (b)
        {
            yesNewGame.SetActive(false);
            yesExit.SetActive(true);
        }
        else
        {
            yesExit.SetActive(false);
            yesNewGame.SetActive(true);
        }
    }

    //Main menu ui functions
    #region MainMenuButtons

    //Loads a given scene
    public void YesNewGame()
    {
        StartCoroutine(LoadScene(tutorial));
    }

    //Loads a given scene
    public void Continue()
    {
        StartCoroutine(LoadScene(actualLevel));
    }

    //Coroutine that starts a aniation of a panel that fades to black and then loads a scene
    IEnumerator LoadScene(string s)
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        blackPanelAnimator.SetBool("ToBlack", true);

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(s);
    }

    //Opens the option panel
    public void Options()
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
        CurrentButton(optionsFirstButton);

        move = true;
    }

    //Exits the game
    public void Quit()
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }

        Application.Quit();
    }
    #endregion

    //Option ui functions
    #region OptionMenuButtons

    //Function of a button that sets the current selected gameobject to a given gameobject (slider)
    public void ActivateSlider(GameObject g)
    {
        EventSystem.current.SetSelectedGameObject(g);
    }

    //Activates and deactivates the postprocesing and saves a boolean (int) playerpref 
    public void PostProcesingToggle(bool b)
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        vignette.active = b;
        bloom.active = b;

        if (b)
        {
            PlayerPrefs.SetInt("ppBool", 1);
        }
        else
        {
            PlayerPrefs.SetInt("ppBool", 0);
        }
    }

    //Activates and deactivates the fullscreen and saves a boolean (int) playerpref 
    public void FullScreenToggle(bool b)
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        if (b)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

            PlayerPrefs.SetInt("fsBool", 1);
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;

            PlayerPrefs.SetInt("fsBool", 0);
        }
    }

    //Changes the quality settings of the project settings and saves the value with Playerprefs
    public void Quality(int i)
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        qLevel = i;

        QualitySettings.SetQualityLevel(qLevel);

        PlayerPrefs.SetInt("qLevel", qLevel);
    }

    //Changes the antialising of a render texture (camera output canvas) and saves the value with Playerprefs
    public void AntiAliasing(float f)
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        aaValue = f;

        if (aaValue == 0)
        {
            SetAntialiasing(1);
        }
        else if (aaValue == 1)
        {
            SetAntialiasing(2);
        }
        else if(aaValue == 2)
        {
            SetAntialiasing(4);
        }
        else if (aaValue == 3)
        {
            SetAntialiasing(8);
        }

        PlayerPrefs.SetFloat("aaValue", aaValue);
    }

    //Changes the gamma (brightness) parameter of a post procesing volume and saves the value with Playerprefs
    public void GammaGain(float f)
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        lggValue = f;

        liftGammaGain.gamma.Override(new Vector4(1f, 1f, 1f, lggValue));

        PlayerPrefs.SetFloat("gamma", lggValue);
    }

    //Changes the field of view value of the player and saves the value with Playerprefs
    public void FieldOfView(float f)
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        fovValue = f;

        PlayerPrefs.SetFloat("fov", fovValue);
    }

    //Changes the volume of an audio mixer and saves the value with Playerprefs
    public void SetMusicVolume(float f)
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        mVolume = f;

        musicAudioMixer.SetFloat("MusicVolume", Mathf.Log10(mVolume) * 20);

        PlayerPrefs.SetFloat("mVolume", mVolume);
    }

    //Changes the volume of an audio mixer and saves the value with Playerprefs
    public void SetSFXVolume(float f)
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        sfxVolume = f;

        SFXAudioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);

        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
    }

    //Changes the X sensibilitie of the camera rotation and saves the value with Playerprefs
    public void SensX(float f)
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        sensX = f;

        PlayerPrefs.SetFloat("sensX", sensX);
    }

    //Changes teh Y sensibilitie of the camera rotation and saves the value with Playerprefs
    public void SensY(float f)
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        sensY = f;

        PlayerPrefs.SetFloat("sensY", sensY);
    }

    //sets the Y sensibilitie to be the same as the X and saves the values with Playerprefs
    public void IgualateSens()
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        sensY = sensX;

        sXSlider.value = sensX;
        sYSlider.value = sensY;

        PlayerPrefs.SetFloat("sensX", sensX);
        PlayerPrefs.SetFloat("sensY", sensY);
    }

    //Sets the antialiasing of a render texture by a given int
    void SetAntialiasing(int i)
    {
        renderTexture.Release();

        renderTexture.antiAliasing = i;

        renderTexture.Create();
    }

    //Activates the mainMenu panel and deactivates the rest
    public void GoBack()
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        optionsPanel.SetActive(false);
        confirmPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        CurrentButton(mainMenuFirstButton);

        move = false;
    }

    #endregion
}
