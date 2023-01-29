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
        lggValue = CheckFloatKey("gamma", lggSlider.value);
        fovValue = CheckFloatKey("fov", fovSlider.value);
        aaValue = CheckFloatKey("aaValue", aaSlider.value);
        sensX = CheckFloatKey("sensX", sXSlider.value);
        sensY = CheckFloatKey("sensY", sYSlider.value);
        mVolume = CheckFloatKey("mVolume", SetAudioMixerValue(musicAudioMixer, "MusicVolume", mVolume));
        sfxVolume = CheckFloatKey("sfxVolume", SetAudioMixerValue(SFXAudioMixer, "SFXVolume", sfxVolume));

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

        if (!PlayerPrefs.HasKey("currentScene"))
        {
            mainMenuFirstButton.SetActive(false);
        }
        else
        {
            mainMenuFirstButton.SetActive(true);
        }

        Screen.fullScreen = fsIsOn;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Confined;

        PostPorcessingParameters();

        sfxAudioSource.mute = true;
        UpdateUI();
        UpdateScene();
        sfxAudioSource.mute = false;
    }

    private void Update()
    {
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

    float SetAudioMixerValue(AudioMixer am, string s, float f)
    {
        am.GetFloat(s, out f);

        return Mathf.Log10(f) * 20;
    }

    public void PlayerSFX(AudioClip ac, float f1, float f2)
    {
        float randomIndex = Random.Range(f1, f2);

        sfxAudioSource.pitch = randomIndex;

        sfxAudioSource.PlayOneShot(ac);
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

    public void CurrentButton(GameObject g)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(g);
    }

    public void ConfirmMessage(string s)
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        if (PlayerPrefs.HasKey("currentScene"))
        {
            confirmMesage.text = s;

            confirmPanel.SetActive(true);
            CurrentButton(confirmFirstButton);
        }
    }

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

    #region MainMenuButtons
    public void NewGame()
    {
        if (!PlayerPrefs.HasKey("currentScene"))
        {
            StartCoroutine(LoadScene(tutorial));
        }
    }

    public void YesNewGame()
    {
        StartCoroutine(LoadScene(tutorial));
    }

    public void Continue()
    {
        StartCoroutine(LoadScene(actualLevel));
    }

    IEnumerator LoadScene(string s)
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        blackPanelAnimator.SetBool("ToBlack", true);

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(s);
    }

    public void Options()
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
        CurrentButton(optionsFirstButton);

        move = true;
    }

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

    #region OptionMenuButtons

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

        PlayerPrefs.SetFloat("sensX", sensX);
    }

    public void SensY(float f)
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

        sensY = f;

        PlayerPrefs.SetFloat("sensY", sensY);
    }

    public void IgualateSens()
    {
        PlayerSFX(sfxs[0], 1, 1.5f);

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
        PlayerSFX(sfxs[0], 1, 1.5f);

        optionsPanel.SetActive(false);
        confirmPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        CurrentButton(mainMenuFirstButton);

        move = false;
    }

    #endregion
}
