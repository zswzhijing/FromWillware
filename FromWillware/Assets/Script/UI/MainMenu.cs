using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 必须引用场景管理

public class MainMenu : MonoBehaviour
{
    [Header("UI面板")]
    public GameObject Settings;

    [Header("音效设置")]
    public AudioSource sfxSource;
    public AudioClip clickSound;
    
    private void PlayClickSFX()
    {
        if (sfxSource != null && clickSound != null)
        {
            sfxSource.PlayOneShot(clickSound);
        }
    }

    // 1. 新的游戏
    public void NewGame()
    {
        PlayClickSFX();
        // 如果开始新游戏，通常需要清除旧的存档记录
        // PlayerPrefs.DeleteKey("SavedSceneIndex"); 

        // 按场景名加载，避免 Build Index 变化导致跳错场景
        SceneManager.LoadScene("MainScene");
    }

    // 2. 继续游戏 (新增)
    public void ContinueGame()
    {
        PlayClickSFX();

        // 从本地读取保存的场景索引。如果从没玩过，默认返回 0 (通常是主菜单)
        int savedSceneIndex = PlayerPrefs.GetInt("SavedSceneIndex", 0);

        if (savedSceneIndex != 0)
        {
            Debug.Log("正在加载存档，关卡索引：" + savedSceneIndex);
            SceneManager.LoadScene(savedSceneIndex);
        }
        else
        {
            // 如果没有存档，可以让他开始新游戏，或者在 UI 上提示“无存档”
            Debug.Log("没有发现存档，请开始新游戏");
            // NewGame(); // 或者取消注释这一行直接开始新游戏
        }
    }

    // 3. 设置按钮
    public void OpenSettings()
    {
        PlayClickSFX();
        Settings.SetActive(true);
    }

    public void CloseSettings()
    {
        PlayClickSFX();
        Settings.SetActive(false);
    }

    // 4. 退出游戏
    public void ExitGame()
    {
        PlayClickSFX();
        Debug.Log("退出游戏");
        Application.Quit();
    }
}