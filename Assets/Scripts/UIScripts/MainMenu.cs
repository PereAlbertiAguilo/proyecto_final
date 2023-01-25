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

    [Header("Panels\n")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Buttons\n")]
    [SerializeField] private GameObject mainMenuFirstButton;
    [SerializeField] private GameObject optionsFirstButton;

    [Header("Components\n")]
    [SerializeField] private Volume postProcessingVolume;
    [SerializeField] private AudioMixer musicAudioMixer;
    [SerializeField] private AudioMixer SFXAudioMixer;

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
        Screen.fullScreen = true;
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;

        postProcessingVolume = GameObject.Find("PostProcesing").GetComponent<Volume>();

        PostPorcessingParameters();

        UpdateUI();
        UpdateScene();
    }

    float SetAudioMixerValue(AudioMixer am, string s, float f)
    {
        am.GetFloat(s, out f);

        return Mathf.Log10(f) * 20;

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

    #region MainMenuButtons
    public void NewGame()
    {
        SceneManager.LoadScene(tutorial);
    }

    public void Continue()
    {
        SceneManager.LoadScene(actualLevel);
    }

    public void Options()
    {
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
        CurrentButton(optionsFirstButton);
    }

    public void Quit()
    {
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
        lggValue = f;

        liftGammaGain.gamma.Override(new Vector4(1f, 1f, 1f, lggValue));

        PlayerPrefs.SetFloat("gamma", lggValue);
    }

    public void FieldOfView(float f)
    {
        fovValue = f;

        PlayerPrefs.SetFloat("fov", fovValue);
    }

    public void SetMusicVolume(float f)
    {
        mVolume = f;

        musicAudioMixer.SetFloat("MusicVolume", Mathf.Log10(mVolume) * 20);

        PlayerPrefs.SetFloat("mVolume", mVolume);
    }

    public void SetSFXVolume(float f)
    {
        sfxVolume = f;

        SFXAudioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);

        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
    }

    public void SensX(float f)
    {
        sensX = f;

        PlayerPrefs.SetFloat("sensX", sensX);
    }

    public void SensY(float f)
    {
        sensY = f;

        PlayerPrefs.SetFloat("sensY", sensY);
    }

    public void IgualateSens()
    {
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
        mainMenuPanel.SetActive(true);
        CurrentButton(mainMenuFirstButton);
    }

    #endregion
}
