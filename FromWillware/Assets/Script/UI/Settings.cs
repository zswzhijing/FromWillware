using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

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

    // 预设的三个分辨率
    private Vector2[] resolutions = new Vector2[]
    {
        new Vector2(1920, 1080),
        new Vector2(2560, 1440),
        new Vector2(3840, 2160)
    };

    void Start()
    {
        // 游戏启动时加载你的保存设置（此处略写，根据需要可以加入PlayerPrefs读取）
        // 默认隐藏设置界面
        // settingsPanel.SetActive(false); 
    }

    // 1. 设置显示模式 (Dropdown Index: 0=全屏, 1=窗口化)
    public void SetDisplayMode(int index)
    {
        bool isFullscreen = (index == 0);
        Screen.fullScreen = isFullscreen;
        Screen.fullScreenMode = isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    // 2. 设置分辨率 (Dropdown Index对应数组下标)
    public void SetResolution(int index)
    {
        if (index >= 0 && index < resolutions.Length)
        {
            Screen.SetResolution((int)resolutions[index].x, (int)resolutions[index].y, Screen.fullScreen);
        }
    }

    // 3. 设置主音量 (滑动条的值0.0001到1，转换为对数分贝值)
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    // 3. 设置音乐音量
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }


    // 5. 退出游戏
    public void QuitGame()
    {
        Debug.Log("退出游戏！");
        Application.Quit(); // 在编辑器中无效，打包后有效
    }

    // 6. 关闭设置界面
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    // （附赠）打开设置界面的方法，可以在其他UI按钮上调用
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }
}