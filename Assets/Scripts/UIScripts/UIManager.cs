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

public class UIManager : MonoBehaviour
{
    private int XboxOneController = 0;
    private int PS4Controller = 0;

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

    private void Awake()
    {
        lggValue = CheckFloatKey("gamma", lggSlider.value);
        fovValue = CheckFloatKey("fov", fovSlider.value);
        aaValue = CheckFloatKey("aaValue", aaSlider.value);
        sensX = CheckFloatKey("sensX", sensX);
        sensY = CheckFloatKey("sensY", sensY);
        mVolume = CheckFloatKey("mVolume", SetAudioMixerValue(musicAudioMixer, "MusicVolume", mVolume));
        sfxVolume = CheckFloatKey("sfxVolume", SetAudioMixerValue(SFXAudioMixer, "SFXVolume", sfxVolume));

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

        PostPorcessingParameters();

        sfxAudioSource.mute = true;
        UpdateUI();
        UpdateScene();
        sfxAudioSource.mute = false;

        Resume();

        canPause = true;
    }

    public void CurrentButton(GameObject g)
    {
        if(PS4Controller == 0 && XboxOneController == 0)
        {
            Cursor.lockState = CursorLockMode.Confined;
            EventSystem.current.SetSelectedGameObject(null);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
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

    float SetAudioMixerValue(AudioMixer am, string s, float f)
    {
        am.GetFloat(s, out f);

        return Mathf.Log10(f) * 20;

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
            if (names[i].Length == 19)
            {
                PS4Controller = 1;
                XboxOneController = 0;
            }
            if (names[i].Length == 33)
            {
                PS4Controller = 0;
                XboxOneController = 1;
            }
        }

        if (EventSystem.current.currentSelectedGameObject != null)
        {
            if (EventSystem.current.currentSelectedGameObject.TryGetComponent(out Button b))
            {
                ColorBlock cb = b.colors;
                cb.highlightedColor = highLightedColor;
                cb.selectedColor = highLightedColor;
                b.colors = cb;
            }
            else if (EventSystem.current.currentSelectedGameObject.TryGetComponent(out Slider s))
            {
                ColorBlock cb = s.colors;
                cb.highlightedColor = highLightedColor;
                cb.selectedColor = highLightedColor;
                s.colors = cb;

                if (Input.GetKeyDown(KeyCode.JoystickButton1))
                {
                    EventSystem.current.SetSelectedGameObject(s.transform.parent.gameObject);
                }
            }
            else if (EventSystem.current.currentSelectedGameObject.TryGetComponent(out Toggle t))
            {
                ColorBlock cb = t.colors;
                cb.highlightedColor = highLightedColor;
                cb.selectedColor = highLightedColor;
                t.colors = cb;
            }
            else if (EventSystem.current.currentSelectedGameObject.TryGetComponent(out Dropdown dd))
            {
                ColorBlock cb = dd.colors;
                cb.highlightedColor = highLightedColor;
                cb.selectedColor = highLightedColor;
                dd.colors = cb;
            }
        }
        else if(!isPaused)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7) && canPause)
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
            else if(isPaused && !pausePanel.activeInHierarchy)
            {
                GoBack();
            }
            else
            {
                Pause();
            }
        }
    }

    #region PauseMenuButtons
    public void Pause()
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        CanMove(false);
        
        isPaused = true;
        Cursor.lockState = CursorLockMode.Confined;
        backgroundPanel.SetActive(true);
        pausePanel.SetActive(true);
        CurrentButton(pauseFirstButton);
    }

    public void Resume()
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        CanMove(true);

        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        backgroundPanel.SetActive(false);
        pausePanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Options()
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        pausePanel.SetActive(false);
        optionsPanel.SetActive(true);
        CurrentButton(optionsFirstButton);
    }

    public void FinishLevel()
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        canPause = false;
        CanMove(false);
        
        Cursor.lockState = CursorLockMode.Confined;
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
        PlayerSFX(sfxs[0], 1, 1.5f);

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
        PlayerSFX(sfxs[0], 1, 1.5f);

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
        PlayerSFX(sfxs[0], 1, 1.5f);

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

    public void Quality(int i)
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        qLevel = i;

        QualitySettings.SetQualityLevel(qLevel);

        PlayerPrefs.SetInt("qLevel", qLevel);
    }

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
        PlayerSFX(sfxs[0], 1, 1.5f);

        lggValue = f;

        liftGammaGain.gamma.Override(new Vector4(1f, 1f, 1f, lggValue));

        PlayerPrefs.SetFloat("gamma", lggValue);
    }

    public void FieldOfView(float f)
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        fovValue = f;

        playerControllerScript.cvCam.m_Lens.FieldOfView = fovValue;
        playerControllerScript.fieldOfView = fovValue;

        PlayerPrefs.SetFloat("fov", fovValue);
    }

    public void SetMusicVolume(float f)
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        mVolume = f;

        musicAudioMixer.SetFloat("MusicVolume", Mathf.Log10(mVolume) * 20);

        PlayerPrefs.SetFloat("mVolume", mVolume);
    }

    public void SetSFXVolume(float f)
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        sfxVolume = f;

        SFXAudioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);

        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
    }
    public void SensX(float f)
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        sensX = f;

        playerControllerScript.cPOV.m_HorizontalAxis.m_MaxSpeed = sensX;

        PlayerPrefs.SetFloat("sensX", sensX);
    }

    public void SensY(float f)
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        sensY = f;

        playerControllerScript.cPOV.m_VerticalAxis.m_MaxSpeed = sensY;

        PlayerPrefs.SetFloat("sensY", sensY);
    }

    public void IgualateSens()
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        SensY(sensX);

        sensY = sensX;

        sXSlider.value = sensX;
        sYSlider.value = sensY;

        PlayerPrefs.SetFloat("sensX", sensX);
        PlayerPrefs.SetFloat("sensY", sensY);
    }

    void SetAntialiasing(int i)
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        renderTexture.Release();

        renderTexture.antiAliasing = i;

        renderTexture.Create();
    }

    public void GoBack()
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        PlayerSFX(sfxs[0], 1, 1.5f);

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
