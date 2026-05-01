using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using System.Collections.Generic; // Add this for List<string>

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
    public bool pauseGameOnSettingsOpen = true; // 是否在打开设置时暂停游戏

    // 预设的三个分辨率
    private Vector2[] resolutions = new Vector2[]
    {
        new Vector2(1920, 1080),
        new Vector2(2560, 1440),
        new Vector2(3840, 2160)
    };

    private float originalTimeScale; // 用于存储游戏暂停前的Time.timeScale
    private bool isGamePausedBySettings = false; // 标记是否是设置界面暂停了游戏

    void Awake() // 使用Awake确保在Start之前初始化
    {
        // 确保设置面板在游戏开始时是隐藏的
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        // 初始化UI元素，使其反映当前游戏状态或上次保存的设置
        InitializeUI();
        LoadSettings(); // 加载保存的设置
    }

    void Update()
    {
        // 监听Esc键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettingsPanel(); // 切换设置面板的显示/隐藏
        }
    }

    // 切换设置面板的显示/隐藏，并处理游戏暂停/恢复
    // 切换设置面板的显示/隐藏，并处理游戏暂停/恢复
    public void ToggleSettingsPanel()
    {
        if (settingsPanel == null) return;

        bool currentlyActive = settingsPanel.activeSelf;
        settingsPanel.SetActive(!currentlyActive); // 切换状态

        if (!currentlyActive) // 如果面板现在要被打开 (之前是inactive)
        {
            // ========== 新增：打开设置时显示鼠标 ==========
            Cursor.visible = true; 
            Cursor.lockState = CursorLockMode.None; // 解除鼠标锁定
            // ============================================

            if (pauseGameOnSettingsOpen)
            {
                originalTimeScale = Time.timeScale; // 记录当前时间尺度
                Time.timeScale = 0f; // 暂停游戏
                isGamePausedBySettings = true;
            }
        }
        else // 如果面板现在要被关闭 (之前是active)
        {
            if (pauseGameOnSettingsOpen)
            {
                if (isGamePausedBySettings) // 只有当是设置界面暂停了游戏时才恢复
                {
                    Time.timeScale = originalTimeScale; // 恢复之前的时间尺度
                    isGamePausedBySettings = false;
                }
            }
        }
    }

    // 1. 设置显示模式 (Dropdown Index: 0=全屏, 1=窗口化)
    public void SetDisplayMode(int index)
    {
        bool isFullscreen = (index == 0);
        Screen.fullScreenMode = isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        PlayerPrefs.SetInt("DisplayMode", index); // 保存设置
        PlayerPrefs.Save();
    }

    // 2. 设置分辨率 (Dropdown Index对应数组下标)
    public void SetResolution(int index)
    {
        if (index >= 0 && index < resolutions.Length)
        {
            Screen.SetResolution((int)resolutions[index].x, (int)resolutions[index].y, Screen.fullScreenMode);
            PlayerPrefs.SetInt("ResolutionIndex", index); // 保存设置
            PlayerPrefs.Save();
        }
    }

    // 3. 设置主音量 (滑动条的值0.0001到1，转换为对数分贝值)
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MasterVolume", volume); // 保存设置
        PlayerPrefs.Save();
    }

    // 4. 设置音乐音量
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("MusicVolume", volume); // 保存设置
        PlayerPrefs.Save();
    }

    // 5. 退出游戏
    // 5. 退出游戏
    public void QuitGame()
    {
        Debug.Log("准备退出游戏！");

        // 如果当前是在 Unity 编辑器里面运行
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 让编辑器停止运行
#else
        // 如果是打包发布后的真正游戏
        Application.Quit(); // 关闭应用程序
#endif
    }

    // 6. 关闭设置界面 (现在直接调用ToggleSettingsPanel)
    public void CloseSettings()
    {
        if (settingsPanel.activeSelf)
        {
            ToggleSettingsPanel();
        }
    }

    // 7. 打开设置界面的方法 (现在直接调用ToggleSettingsPanel)
    public void OpenSettings()
    {
        if (!settingsPanel.activeSelf)
        {
            ToggleSettingsPanel();
        }
    }

    // 初始化UI元素以反映当前状态
    private void InitializeUI()
    {
        // 填充显示模式Dropdown
        displayModeDropdown.ClearOptions();
        List<string> displayOptions = new List<string> { "全屏", "窗口化" };
        displayModeDropdown.AddOptions(displayOptions);
        displayModeDropdown.value = (Screen.fullScreenMode == FullScreenMode.FullScreenWindow || Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen) ? 0 : 1;
        displayModeDropdown.RefreshShownValue();
        displayModeDropdown.onValueChanged.AddListener(SetDisplayMode); // 绑定事件

        // 填充分辨率Dropdown
        resolutionDropdown.ClearOptions();
        List<string> resolutionOptions = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            resolutionOptions.Add(resolutions[i].x + "x" + resolutions[i].y);
            // 找到当前分辨率在预设列表中的位置
            if (Screen.currentResolution.width == resolutions[i].x && Screen.currentResolution.height == resolutions[i].y)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(SetResolution); // 绑定事件

        // 初始化音量滑块并绑定事件
        masterVolumeSlider.minValue = 0.0001f; // 音量不能为0，否则Log10会出错
        masterVolumeSlider.maxValue = 1f;
        musicVolumeSlider.minValue = 0.0001f;
        musicVolumeSlider.maxValue = 1f;

        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
    }


    // 加载保存的设置
    private void LoadSettings()
    {
        // 加载显示模式
        if (PlayerPrefs.HasKey("DisplayMode"))
        {
            int displayMode = PlayerPrefs.GetInt("DisplayMode");
            displayModeDropdown.value = displayMode;
            SetDisplayMode(displayMode); // 应用设置
        }
        else
        {
            // 如果没有保存，则使用当前游戏状态
            SetDisplayMode(displayModeDropdown.value);
        }

        // 加载分辨率
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            int resIndex = PlayerPrefs.GetInt("ResolutionIndex");
            resolutionDropdown.value = resIndex;
            SetResolution(resIndex); // 应用设置
        }
        else
        {
            // 如果没有保存，则使用当前游戏状态
            SetResolution(resolutionDropdown.value);
        }

        // 加载主音量
        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            float volume = PlayerPrefs.GetFloat("MasterVolume");
            masterVolumeSlider.value = volume;
            SetMasterVolume(volume); // 应用设置
        }
        else
        {
            // 如果没有保存，从AudioMixer获取默认值
            float currentVolDB;
            if (audioMixer.GetFloat("MasterVolume", out currentVolDB))
            {
                 masterVolumeSlider.value = Mathf.Pow(10, currentVolDB / 20f);
            }
            else
            {
                masterVolumeSlider.value = 0.75f; // 默认值
            }
            SetMasterVolume(masterVolumeSlider.value);
        }


        // 加载音乐音量
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            float volume = PlayerPrefs.GetFloat("MusicVolume");
            musicVolumeSlider.value = volume;
            SetMusicVolume(volume); // 应用设置
        }
        else
        {
            // 如果没有保存，从AudioMixer获取默认值
            float currentVolDB;
            if (audioMixer.GetFloat("MusicVolume", out currentVolDB))
            {
                 musicVolumeSlider.value = Mathf.Pow(10, currentVolDB / 20f);
            }
            else
            {
                musicVolumeSlider.value = 0.75f; // 默认值
            }
            SetMusicVolume(musicVolumeSlider.value);
        }

        // 刷新Dropdown的显示值，确保UI与加载的数据同步
        displayModeDropdown.RefreshShownValue();
        resolutionDropdown.RefreshShownValue();
    }
}