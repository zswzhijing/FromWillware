using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using System.Collections.Generic;
using AshenThrone;

public class Settings : MonoBehaviour
{
    [Header("UI 引用")]
    public GameObject settingsPanel;
    public TMP_Dropdown displayModeDropdown;
    public TMP_Dropdown resolutionDropdown;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;

    [Header("音频设置")]
    public AudioMixer audioMixer;

    [Header("游戏控制")]
    public bool pauseGameOnSettingsOpen = true;

    private Vector2[] resolutions = new Vector2[]
    {
        new Vector2(1920, 1080),
        new Vector2(2560, 1440),
        new Vector2(3840, 2160)
    };

    private float originalTimeScale;
    private bool isGamePausedBySettings = false;

    void Awake()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        InitializeUI();
        LoadSettings();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettingsPanel();
        }
    }

    public void ToggleSettingsPanel()
    {
        if (settingsPanel == null) return;

        bool currentlyActive = settingsPanel.activeSelf;
        settingsPanel.SetActive(!currentlyActive);

        if (!currentlyActive)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.SetSettingsOpen(true);

            if (pauseGameOnSettingsOpen)
            {
                originalTimeScale = Time.timeScale;
                Time.timeScale = 0f;
                isGamePausedBySettings = true;
            }
        }
        else
        {
            if (GameManager.Instance != null)
                GameManager.Instance.SetSettingsOpen(false);

            if (pauseGameOnSettingsOpen)
            {
                if (isGamePausedBySettings)
                {
                    Time.timeScale = originalTimeScale;
                    isGamePausedBySettings = false;
                }
            }
        }
    }

    public void SetDisplayMode(int index)
    {
        bool isFullscreen = (index == 0);
        Screen.fullScreenMode = isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        PlayerPrefs.SetInt("DisplayMode", index);
        PlayerPrefs.Save();
    }

    public void SetResolution(int index)
    {
        if (index >= 0 && index < resolutions.Length)
        {
            Screen.SetResolution((int)resolutions[index].x, (int)resolutions[index].y, Screen.fullScreenMode);
            PlayerPrefs.SetInt("ResolutionIndex", index);
            PlayerPrefs.Save();
        }
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void QuitGame()
    {
        Debug.Log("准备退出游戏！");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void CloseSettings()
    {
        if (settingsPanel.activeSelf)
        {
            ToggleSettingsPanel();
        }
    }

    public void OpenSettings()
    {
        if (!settingsPanel.activeSelf)
        {
            ToggleSettingsPanel();
        }
    }

    private void InitializeUI()
    {
        displayModeDropdown.ClearOptions();
        List<string> displayOptions = new List<string> { "全屏", "窗口化" };
        displayModeDropdown.AddOptions(displayOptions);
        displayModeDropdown.value = (Screen.fullScreenMode == FullScreenMode.FullScreenWindow || Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen) ? 0 : 1;
        displayModeDropdown.RefreshShownValue();
        displayModeDropdown.onValueChanged.AddListener(SetDisplayMode);

        resolutionDropdown.ClearOptions();
        List<string> resolutionOptions = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            resolutionOptions.Add(resolutions[i].x + "x" + resolutions[i].y);
            if (Screen.currentResolution.width == resolutions[i].x && Screen.currentResolution.height == resolutions[i].y)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(SetResolution);

        masterVolumeSlider.minValue = 0.0001f;
        masterVolumeSlider.maxValue = 1f;
        musicVolumeSlider.minValue = 0.0001f;
        musicVolumeSlider.maxValue = 1f;

        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("DisplayMode"))
        {
            int displayMode = PlayerPrefs.GetInt("DisplayMode");
            displayModeDropdown.value = displayMode;
            SetDisplayMode(displayMode);
        }
        else
        {
            SetDisplayMode(displayModeDropdown.value);
        }

        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            int resIndex = PlayerPrefs.GetInt("ResolutionIndex");
            resolutionDropdown.value = resIndex;
            SetResolution(resIndex);
        }
        else
        {
            SetResolution(resolutionDropdown.value);
        }

        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            float volume = PlayerPrefs.GetFloat("MasterVolume");
            masterVolumeSlider.value = volume;
            SetMasterVolume(volume);
        }
        else
        {
            float currentVolDB;
            if (audioMixer.GetFloat("MasterVolume", out currentVolDB))
            {
                 masterVolumeSlider.value = Mathf.Pow(10, currentVolDB / 20f);
            }
            else
            {
                masterVolumeSlider.value = 0.75f;
            }
            SetMasterVolume(masterVolumeSlider.value);
        }

        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            float volume = PlayerPrefs.GetFloat("MusicVolume");
            musicVolumeSlider.value = volume;
            SetMusicVolume(volume);
        }
        else
        {
            float currentVolDB;
            if (audioMixer.GetFloat("MusicVolume", out currentVolDB))
            {
                 musicVolumeSlider.value = Mathf.Pow(10, currentVolDB / 20f);
            }
            else
            {
                musicVolumeSlider.value = 0.75f;
            }
            SetMusicVolume(musicVolumeSlider.value);
        }

        displayModeDropdown.RefreshShownValue();
        resolutionDropdown.RefreshShownValue();
    }
}
