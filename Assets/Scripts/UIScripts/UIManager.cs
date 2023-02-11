using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using Cinemachine;

public class UIManager : MonoBehaviour
{
    [HideInInspector] public int XboxOneController = 0;

    [HideInInspector] public string actualLevel;

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
    [SerializeField] private GameObject yesRestart;

    [Header("Parameters\n")]
    [SerializeField] private Volume postProcessingVolume;
    [SerializeField] private AudioMixer musicAudioMixer;
    [SerializeField] private AudioMixer SFXAudioMixer;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioClip[] sfxs;
    [SerializeField] private Animator blackPanelAnimator;
    [SerializeField] private Color highLightedColor;
    [SerializeField] private LayerMask layerUI;


    LiftGammaGain liftGammaGain;
    Bloom bloom;
    Vignette vignette;

    [SerializeField] private RenderTexture renderTexture;

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

    private bool isPaused;
    [HideInInspector] public bool canPause;

    private PlayerController playerControllerScript;
    private ForceFieldShooter ForceFieldShooterScript;
    private ResetLevel resetLevelScript;
    private NextLevel nextLevelScript;

    [Header("Panels\n")]
    [SerializeField] private GameObject backgroundPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject nextLevelPanel;
    [SerializeField] private GameObject confirmPanel;

    [Header("Buttons\n")]
    [SerializeField] private GameObject pauseFirstButton;
    [SerializeField] private GameObject optionsFirstButton;
    [SerializeField] private GameObject nextLevelFirstButton;
    [SerializeField] private GameObject confirmFirstButton;

    private float oneUnit = 1f;
    private float halfUnit = .5f;
    private float audioMixerVolumeMulti = 20f;
    private float allXboxButtons = 33f;

    private float defaultGammaValue = 1f;
    private float defaultFovValue = 65f;
    private float defaultSens = 1.5f;
    private float defaultVolume = .2f;

    private bool changeScene;

    private void Awake()
    {
        lggValue = CheckFloatKey("gamma", defaultGammaValue);
        fovValue = CheckFloatKey("fov", defaultFovValue);
        aaValue = CheckFloatKey("aaValue", 0);
        sensX = CheckFloatKey("sensX", defaultSens);
        sensY = CheckFloatKey("sensY", defaultSens);
        mVolume = CheckFloatKey("mVolume", defaultVolume);
        sfxVolume = CheckFloatKey("sfxVolume", defaultVolume);

        qLevel = CheckIntKey("qLevel", 0);

        ppIsOn = CheckBoolKey("ppBool");
        fsIsOn = CheckBoolKey("fsBool");

        actualLevel = CheckStringKey("currentScene", SceneManager.GetActiveScene().name);
    }

    private void Start()
    {
        playerControllerScript = FindObjectOfType<PlayerController>();
        ForceFieldShooterScript = FindObjectOfType<ForceFieldShooter>();
        resetLevelScript = FindObjectOfType<ResetLevel>();
        nextLevelScript = FindObjectOfType<NextLevel>();

        postProcessingVolume = GameObject.Find("PostProcesing").GetComponent<Volume>();
        sfxAudioSource = GameObject.Find("SFX").GetComponent<AudioSource>();

        StartCoroutine(MuteAS());

        PostPorcessingParameters();

        UpdateUI();
        UpdateScene();
        
        canPause = true;

        Resume();
    }

    IEnumerator MuteAS()
    {
        sfxAudioSource.mute = true;
        yield return new WaitForSeconds(halfUnit);
        sfxAudioSource.mute = false;
    }

    public void CurrentButton(GameObject g)
    {
        if(XboxOneController == 1)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(g);
        }
    }

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

    float GetAudioMixerValue(AudioMixer am, string s, float f)
    {
        am.GetFloat(s, out f);

        return Mathf.Log10(f) * audioMixerVolumeMulti;
    }

    public void CanMove(bool b)
    {
        playerControllerScript.canMove = b;
        playerControllerScript.virtualCam.gameObject.SetActive(b);
        ForceFieldShooterScript.canShoot = b;

        if (ForceFieldShooterScript.enabled)
        {
            ForceFieldShooterScript.canShoot = b;
        }

        if (!b)
        {
            Physics.gravity = Vector3.zero;
            playerControllerScript._playerRigidbody.velocity = Vector3.zero;
        }
        else
        {
            Physics.gravity = new Vector3(0, playerControllerScript.normalGrav, 0);
        }
    }

    private void Update()
    {
        string[] names = Input.GetJoystickNames();

        for (int i = 0; i < names.Length; i++)
        {
            if (names[i].Length == allXboxButtons)
            {
                XboxOneController = 1;
            }
            else
            {
                XboxOneController = 0;
            }
        }

        if (EventSystem.current.currentSelectedGameObject != null)
        {
            if (EventSystem.current.currentSelectedGameObject.TryGetComponent(out Slider s))
            {
                if (Input.GetKeyDown(KeyCode.JoystickButton1))
                {
                    EventSystem.current.SetSelectedGameObject(s.transform.parent.gameObject);
                }
            }
        }

        if (Input.anyKeyDown)
        {
            if(pausePanel.activeInHierarchy || nextLevelPanel.activeInHierarchy || confirmPanel.activeInHierarchy)
            {
                if (XboxOneController == 1 && EventSystem.current.currentSelectedGameObject == null)
                {
                    Cursor.visible = false;

                    if (pausePanel.activeInHierarchy)
                    {
                        CurrentButton(pausePanel);
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
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            if (canPause)
            {
                if (isPaused && pausePanel.activeInHierarchy)
                {
                    if (confirmPanel.activeInHierarchy)
                    {
                        GoBack();
                    }
                    else
                    {
                        Resume();
                    }
                }
                else if (isPaused && !pausePanel.activeInHierarchy)
                {
                    GoBack();
                }
                else
                {
                    Pause();
                }
            }
        }

        if (changeScene)
        {
            float mf;
            float sf;

            musicAudioMixer.GetFloat("MusicVolume", out mf);
            SFXAudioMixer.GetFloat("SFXVolume", out sf);

            musicAudioMixer.SetFloat("MusicVolume", Mathf.Lerp(mf, -80f, Time.deltaTime * (halfUnit + defaultVolume)));
            SFXAudioMixer.SetFloat("SFXVolume", Mathf.Lerp(sf, -80f, Time.deltaTime * (halfUnit + defaultVolume)));
        }
    }

    //Plays a sound effect
    public void ButtonSFX()
    {
        PlayerSFX(sfxs[0], oneUnit, oneUnit + halfUnit);
    }

    #region PauseMenuButtons
    public void Pause()
    {
        CanMove(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
        backgroundPanel.SetActive(true);
        pausePanel.SetActive(true);
        CurrentButton(pauseFirstButton);
    }

    public void Resume()
    {
        CanMove(true);

        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        backgroundPanel.SetActive(false);
        pausePanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Options()
    {
        pausePanel.SetActive(false);
        optionsPanel.SetActive(true);
        CurrentButton(optionsFirstButton);
    }

    public void FinishLevel()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        canPause = false;
        CanMove(false);
        backgroundPanel.SetActive(true);
        nextLevelPanel.SetActive(true);
        CurrentButton(nextLevelFirstButton);
    }

    public void NextLevel()
    {
        CanMove(false);

        StartCoroutine(LoadScene(nextLevelScript.nextLevel, nextLevelScript.nextLevel));
    }

    public void CheckPointRestart()
    {
        Resume();
        resetLevelScript.CheckPointRestart();
    }

    public void ResetLevel()
    {
        CanMove(false);

        StartCoroutine(LoadScene(SceneManager.GetActiveScene().name, SceneManager.GetActiveScene().name));
    }

    public void PlayerSFX(AudioClip ac, float f1, float f2)
    {
        float randomIndex = Random.Range(f1, f2);

        sfxAudioSource.pitch = randomIndex;

        sfxAudioSource.PlayOneShot(ac);
    }

    public void ConfirmMessage(string s)
    {
        confirmMesage.text = s;

        pausePanel.SetActive(false);
        confirmPanel.SetActive(true);
        CurrentButton(confirmFirstButton);
    }

    public void ConfirmButtons(bool b)
    {
        if (b)
        {
            yesRestart.SetActive(false);
            yesExit.SetActive(true);
        }
        else
        {
            yesExit.SetActive(false);
            yesRestart.SetActive(true);
        }
    }

    public void Quit()
    {
        StartCoroutine(LoadScene("MainMenu", SceneManager.GetActiveScene().name));
    }

    IEnumerator LoadScene(string loadScene, string saveScene)
    {
        changeScene = true;

        blackPanelAnimator.SetBool("ToBlack", true);

        PlayerPrefs.SetString("currentScene", saveScene);

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(loadScene);
    }

#endregion

    #region OptionMenuButtons

    public void ActivateSlider(GameObject g)
    {
        EventSystem.current.SetSelectedGameObject(g);
    }

    public void PostProcesingToggle(bool b)
    {
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

    public void FullScreenToggle(bool b)
    {
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

    public void Quality(int i)
    {
        qLevel = i;

        QualitySettings.SetQualityLevel(qLevel);

        PlayerPrefs.SetInt("qLevel", qLevel);
    }

    public void AntiAliasing(float f)
    {
        aaValue = f;

        if (aaValue == 0)
        {
            SetAntialiasing(1);
        }
        else if (aaValue == 1)
        {
            SetAntialiasing(2);
        }
        else if (aaValue == 2)
        {
            SetAntialiasing(4);
        }
        else if (aaValue == 3)
        {
            SetAntialiasing(8);
        }

        PlayerPrefs.SetFloat("aaValue", aaValue);
    }

    public void GammaGain(float f)
    {
        lggValue = f;

        liftGammaGain.gamma.Override(new Vector4(1f, 1f, 1f, lggValue));

        PlayerPrefs.SetFloat("gamma", lggValue);
    }

    public void FieldOfView(float f)
    {
        fovValue = f;

        playerControllerScript.cvCam.m_Lens.FieldOfView = fovValue;
        playerControllerScript.fieldOfView = fovValue;

        PlayerPrefs.SetFloat("fov", fovValue);
    }

    public void SetMusicVolume(float f)
    {
        mVolume = f;

        musicAudioMixer.SetFloat("MusicVolume", Mathf.Log10(mVolume) * audioMixerVolumeMulti);

        PlayerPrefs.SetFloat("mVolume", mVolume);
    }

    public void SetSFXVolume(float f)
    {
        sfxVolume = f;

        SFXAudioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * audioMixerVolumeMulti);

        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
    }
    public void SensX(float f)
    {
        sensX = f;

        playerControllerScript.cPOV.m_HorizontalAxis.m_MaxSpeed = sensX;

        PlayerPrefs.SetFloat("sensX", sensX);
    }

    public void SensY(float f)
    {
        sensY = f;

        playerControllerScript.cPOV.m_VerticalAxis.m_MaxSpeed = sensY;

        PlayerPrefs.SetFloat("sensY", sensY);
    }

    public void IgualateSens()
    {
        SensY(sensX);

        sensY = sensX;

        sXSlider.value = sensX;
        sYSlider.value = sensY;

        PlayerPrefs.SetFloat("sensX", sensX);
        PlayerPrefs.SetFloat("sensY", sensY);
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
        confirmPanel.SetActive(false);
        pausePanel.SetActive(true);
        CurrentButton(pauseFirstButton);
    }

    #endregion

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("currentScene", SceneManager.GetActiveScene().name);
    }
}
