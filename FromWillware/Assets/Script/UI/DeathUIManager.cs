using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathUIManager : MonoBehaviour
{[Header("引用设置")]
    public Player player;             // 引用你的主角
    public GameObject deathPanel;     // 死亡UI面板
    public float showDelay = 2.5f;    // 死亡后延迟几秒显示UI

    private bool hasTriggeredDeath = false; // 防止重复触发的开关

    void Start()
    {
        // 如果你在面板里忘了拖拽主角，这里会自动去场景里找一次
        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }
    }

    void Update()
    {
        // 核心逻辑：如果在 Update 中发现主角死了，且还没触发过 UI 逻辑
        if (player != null && player.IsDead && !hasTriggeredDeath)
        {
            hasTriggeredDeath = true; // 标记为已触发，防止每帧都执行
            StartCoroutine(ShowDeathMenuDelay()); // 开启延迟显示协程
        }
    }

    // 延迟显示UI的协程
    private IEnumerator ShowDeathMenuDelay()
    {
        yield return new WaitForSeconds(showDelay);

        if (deathPanel != null)
        {
            deathPanel.SetActive(true); // 显示死亡面板
            
            // 【重要】如果你游戏里隐藏了鼠标，死亡后需要把鼠标解锁显示出来才能点按钮
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // --- 下面是给按钮绑定的功能 ---

    public void ContinueGame()
    {
        Debug.Log("死亡后点击了继续游戏");

        string savePath = SaveSystem.GetSavePath("save.json");
        int savedSceneIndex = PlayerPrefs.GetInt("SavedSceneIndex", 0);

        // 如果有存档，就重新加载存档所在的场景，并发出读档信号
        if (File.Exists(savePath) && savedSceneIndex != 0)
        {
            SaveSystem.shouldLoadSaveGame = true; 
            SceneManager.LoadScene(savedSceneIndex); 
        }
        else
        {
            // 如果没存档，退回主菜单 (索引0)
            Debug.Log("没有存档，返回主菜单");
            SceneManager.LoadScene(0);
        }
    }

    public void ExitGame()
    {
        Debug.Log("退出游戏");
        Application.Quit();
    }
}