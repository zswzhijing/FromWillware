using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement; // 必须引用场景管理

public class MainMenu : MonoBehaviour
{
    [Header("UI面板")]
    public GameObject Settings;[Header("音效设置")]
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
        
        // 如果是新游戏，我们不需要读取旧存档
        SaveSystem.shouldLoadSaveGame = false;

        // 可选：删除旧的存档文件，防止新游戏被旧数据污染
        string savePath = SaveSystem.GetSavePath("save.json");
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
        PlayerPrefs.DeleteKey("SavedSceneIndex");

        // 按场景名加载
        SceneManager.LoadScene("MainScene");
    }

    // 2. 继续游戏 (修改后)
    public void ContinueGame()
    {
        PlayClickSFX();

        // 检查 json 存档文件是否存在
        string savePath = SaveSystem.GetSavePath("save.json");
        bool hasJsonSave = File.Exists(savePath);

        // 获取记录的场景索引
        int savedSceneIndex = PlayerPrefs.GetInt("SavedSceneIndex", 0);

        if (hasJsonSave && savedSceneIndex != 0)
        {
            Debug.Log("正在加载存档，关卡索引：" + savedSceneIndex);
            
            // 【关键步骤】：告诉 SaveSystem，场景加载完毕后请执行 Load() 函数
            SaveSystem.shouldLoadSaveGame = true; 
            
            // 加载游戏场景
            SceneManager.LoadScene(savedSceneIndex);
        }
        else
        {
            Debug.Log("没有发现存档，请开始新游戏");
            // 可以在这里弹出一个UI提示框告诉玩家没有存档
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