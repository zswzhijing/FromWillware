using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AshenThrone;
using AshenThrone.DialogueSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    
    public event Action OnDialogueClosed;

    [Header("==== UI 面板引用 ====")]
    public GameObject menuPanel;          // 左侧话题菜单面板
    public GameObject subtitlePanel;      // 底部字幕面板
    
    [Header("==== 菜单组件 ====")]
    public TextMeshProUGUI menuNpcNameText; // 菜单顶部的NPC名字
    public Transform buttonContainer;       // 存放选项按钮的父节点
    public GameObject buttonPrefab;         // 选项按钮的预制体

    [Header("==== 字幕组件 ====")]
    public TextMeshProUGUI subtitleText;    // 底部显示的台词文字
    public float typeSpeed = 0.05f;         // 打字机速度

    [Header("==== 鲁格最终演出 ====")]
    public GameObject blackScreen;           // 黑屏面板（需要有CanvasGroup组件）
    public TextMeshProUGUI blackScreenText;  // 黑屏上显示文字的TextMeshPro
    public Transform anvilTransform;        // 铁砧位置（生成道具用）
    public GameObject kanesWillPrefab;       // 凯恩的遗志预制体
    public GameObject rugerGameObject;       // 鲁格游戏对象
    public AudioClip hammerSound;            // 打铁音效
    public AudioSource audioSource;          // 音频播放器
    public float fadeDuration = 1.5f;        // 淡入淡出时长

    // ---- 状态与缓存 ----
    private NPCData currentNPC;
    private TopicData currentTopic;
    private string[] currentLinesToPlay;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool isDialogueActive = false;
    private MainQuestPhase dialogueStartPhase; // 记录对话开始时的阶段

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 初始化时隐藏UI（先检查是否为null）
        if (menuPanel != null)
            menuPanel.SetActive(false);
        if (subtitlePanel != null)
            subtitlePanel.SetActive(false);
        
        // 初始状态隐藏鼠标
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                if (subtitleText != null && currentLinesToPlay != null && currentLineIndex < currentLinesToPlay.Length)
                    subtitleText.text = currentLinesToPlay[currentLineIndex];
                isTyping = false;
            }
            else
            {
                DisplayNextLine();
            }
        }
    }

    // ================== 1. 话题菜单逻辑 ==================

    public void OpenTopicMenu(NPCData npcData)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        GameManager.Instance.SetDialogueActive(true);
        
        currentNPC = npcData;
        Debug.Log($"[DialogueManager] 打开话题菜单 - NPC: {npcData.npcName}, 当前阶段: {GameManager.Instance.MainQuestPhase}");
        
        if (menuNpcNameText != null)
            menuNpcNameText.text = npcData.npcName;
        
        if (buttonContainer != null)
            {
                // 清理旧的按钮
                foreach (Transform child in buttonContainer)
                {
                    Destroy(child.gameObject);
                }

                // 遍历该NPC的所有话题，动态生成按钮
                if (npcData.topics != null)
                {
                    foreach (TopicData topic in npcData.topics)
                    {
                        // 检查主线进度条件（当前阶段是否在可见阶段中）
                        if ((GameManager.Instance.MainQuestPhase & topic.visibleInPhases) == 0)
                            continue;

                        // 检查解锁条件
                        if (!string.IsNullOrEmpty(topic.requiredFlag) && !GameManager.Instance.GetFlag(topic.requiredFlag)) 
                            continue;

                        // 检查是否是一次性话题且已经读过
                        string readFlag = currentNPC.npcName + "_" + topic.topicName + "_" + GameManager.Instance.MainQuestPhase + "_Read";
                        if (topic.isOneTime && GameManager.Instance.GetFlag(readFlag))
                            continue; // 一次性且读过了，按钮消失

                        // 生成按钮
                        if (buttonPrefab != null)
                        {
                            GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
                            
                            // 先获取TextMeshPro
                            TextMeshProUGUI btnText = btnObj.GetComponent<TextMeshProUGUI>(); // 先找本身
                            if (btnText == null)
                            {
                                btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>(); // 找不到再找子对象
                            }
                            
                            if (btnText != null)
                            {
                                btnText.text = topic.topicName;
                                Debug.Log($"设置话题: " + topic.topicName);
                            }
                            else
                            {
                                Debug.LogError("找不到TextMeshProUGUI！");
                            }
                            
                            // 绑定按钮点击事件
                            Button btn = btnObj.GetComponent<Button>();
                            if (btn != null)
                            {
                                btn.onClick.AddListener(() => StartTopic(topic));
                                Debug.Log($"添加点击事件: " + topic.topicName);
                            }
                            else
                            {
                                Debug.LogError("找不到Button组件！");
                            }
                        }
                    }
                }

                // 永远在最下方添加一个"离开"按钮
                if (buttonPrefab != null)
                {
                    GameObject leaveBtnObj = Instantiate(buttonPrefab, buttonContainer);
                    TextMeshProUGUI leaveBtnText = leaveBtnObj.GetComponentInChildren<TextMeshProUGUI>();
                    if (leaveBtnText != null)
                        leaveBtnText.text = "离开";
                    Button leaveBtn = leaveBtnObj.GetComponent<Button>();
                    if (leaveBtn != null)
                        leaveBtn.onClick.AddListener(CloseAll);
                }
            }

        // 切换UI显示
        if (subtitlePanel != null)
            subtitlePanel.SetActive(false);
        if (menuPanel != null)
            menuPanel.SetActive(true);
    }

    public void PlayGreeting(NPCData npcData)
    {
        if (string.IsNullOrEmpty(npcData.greetingText))
        {
            OpenTopicMenu(npcData);
            return;
        }

        currentNPC = npcData;
        if (menuPanel != null)
            menuPanel.SetActive(false);
        if (subtitlePanel != null)
            subtitlePanel.SetActive(true);
        isDialogueActive = true;
        GameManager.Instance.SetDialogueActive(true);

        StartCoroutine(PlayGreetingCoroutine(npcData.greetingText));
    }

    private System.Collections.IEnumerator PlayGreetingCoroutine(string greetingText)
    {
        isTyping = true;
        if (subtitleText != null)
            subtitleText.text = "";

        foreach (char letter in greetingText.ToCharArray())
        {
            if (subtitleText != null)
                subtitleText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;

        bool waitingForInput = true;
        while (waitingForInput)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isTyping)
                {
                    StopAllCoroutines();
                    if (subtitleText != null)
                        subtitleText.text = greetingText;
                    isTyping = false;
                }
                else
                {
                    waitingForInput = false;
                }
            }
            yield return null;
        }

        if (subtitlePanel != null)
            subtitlePanel.SetActive(false);
        isDialogueActive = false;
        OpenTopicMenu(currentNPC);
    }

    // ================== 2. 对话播放逻辑 ==================

    public void StartTopic(TopicData topic)
    {
        currentTopic = topic;
        if (menuPanel != null)
            menuPanel.SetActive(false);
        if (subtitlePanel != null)
            subtitlePanel.SetActive(true);
        isDialogueActive = true;
        GameManager.Instance.SetDialogueActive(true);
        
        dialogueStartPhase = GameManager.Instance.MainQuestPhase;

        string readFlag = currentNPC.npcName + "_" + topic.topicName + "_" + dialogueStartPhase + "_Read";
        bool hasReadBefore = GameManager.Instance.GetFlag(readFlag);
        
        Debug.Log($"[StartTopic] 话题: {topic.topicName}, 阶段: {dialogueStartPhase}, 读取标记: {readFlag}, 是否已读: {hasReadBefore}");

        // 【魂系复读机逻辑判定】
        if (hasReadBefore)
        {
            // 如果读过，优先播放 repeatLines
            if (topic.repeatLines != null && topic.repeatLines.Length > 0)
            {
                currentLinesToPlay = topic.repeatLines;
                Debug.Log("[StartTopic] 播放复读内容");
            }
            else
            {
                // 如果没有配置 repeatLines，就只重复原对话的最后一句
                if (topic.dialogueLines != null && topic.dialogueLines.Length > 0)
                {
                    currentLinesToPlay = new string[] { topic.dialogueLines[topic.dialogueLines.Length - 1] };
                    Debug.Log("[StartTopic] 播放最后一句");
                }
            }
        }
        else
        {
            // 第一次阅读
            currentLinesToPlay = topic.dialogueLines;
            Debug.Log("[StartTopic] 首次阅读，播放完整对话");
            if (topic.dialogueLines != null)
            {
                Debug.Log("原对话句数: " + topic.dialogueLines.Length);
                for (int i = 0; i < topic.dialogueLines.Length; i++)
                {
                    Debug.Log($"第{i}句: " + topic.dialogueLines[i]);
                }
            }
            else
            {
                Debug.LogError("topic.dialogueLines 为 null！");
            }
        }

        currentLineIndex = 0;
        if (currentLinesToPlay != null && currentLinesToPlay.Length > 0)
        {
            Debug.Log($"当前句子索引: 0, 总句数: " + currentLinesToPlay.Length);
            StartCoroutine(TypeText(currentLinesToPlay[0]));
        }
        else
        {
            Debug.LogError("没有可以播放的句子！");
        }
    }

    // 边缘NPC直接说话（针对老瞎眼等，没有菜单，直接播放第一个话题）
    public void StartDirectDialogue(NPCData npcData)
    {
        currentNPC = npcData;
        if (npcData.topics.Length > 0)
        {
            StartTopic(npcData.topics[0]);
        }
    }

    private void DisplayNextLine()
    {
        currentLineIndex++;
        Debug.Log($"DisplayNextLine - 索引: " + currentLineIndex);

        if (currentLinesToPlay != null && currentLineIndex < currentLinesToPlay.Length)
        {
            // 还有下一句，继续播放
            Debug.Log($"播放下一句");
            StartCoroutine(TypeText(currentLinesToPlay[currentLineIndex]));
        }
        else
        {
            // 该话题播放完毕
            Debug.Log("对话结束");
            EndTopic();
        }
    }

    // 打字机特效协程（对话字幕用）
    private IEnumerator TypeText(string line)
    {
        isTyping = true;
        if (subtitleText != null)
            subtitleText.text = "";

        if (subtitleText != null)
        {
            foreach (char letter in line.ToCharArray())
            {
                subtitleText.text += letter;
                // 可在这里加入极轻微的打字音效：AudioManager.Play("TypeSound");
                yield return new WaitForSeconds(typeSpeed);
            }
        }

        isTyping = false;
    }

    // 黑屏打字机特效协程
    private IEnumerator TypeBlackScreenText(string line)
    {
        if (blackScreenText != null)
        {
            blackScreenText.text = "";
            foreach (char letter in line.ToCharArray())
            {
                blackScreenText.text += letter;
                yield return new WaitForSeconds(typeSpeed);
            }
        }
    }

    // ================== 3. 结束与事件分发 ==================

    private void EndTopic()
    {
        isDialogueActive = false;
        
        if (currentTopic != null)
        {
            string readFlag = currentNPC.npcName + "_" + currentTopic.topicName + "_" + dialogueStartPhase + "_Read";
            Debug.Log($"[EndTopic] 设置读取标记: {readFlag} = true");
            GameManager.Instance.SetFlag(readFlag, true);

            if (!string.IsNullOrEmpty(currentTopic.triggerEvent))
            {
                HandleSpecialEvent(currentTopic.triggerEvent);
                
                if (currentTopic.triggerEvent == "Ruger_FinalRitual")
                {
                    return;
                }
            }
        }

        if (currentNPC.hasTopicMenu)
        {
            OpenTopicMenu(currentNPC);
        }
        else
        {
            CloseAll();
        }
    }

    // 集中处理文档中提到的特殊事件（给道具、黑屏等）
    private void HandleSpecialEvent(string eventName)
    {
        Debug.Log($"触发对话特殊事件：{eventName}");
        
        switch (eventName)
        {
            case "Give_Withered_Flower":
                GameManager.Instance.SetFlag("Has_Withered_Flower", true);
                GameManager.Instance.GiveLienQuestItem();
                break;
            
            case "Ruger_Repair_Sword":
                GameManager.Instance.SetFlag("Ruger_State", true);
                break;
            
            case "Open_Shop":
                CloseAll();
                break;
            
            case "Open_Upgrade":
                CloseAll();
                break;
            
            case "Ruger_FinalRitual":
                StartCoroutine(RugerFinalRitual());
                break;
        }
    }

    private IEnumerator RugerFinalRitual()
    {
        CloseAll();
        yield return new WaitForSeconds(0.5f);
        
        // 移除背包中的断锤
        GameManager.Instance.RemoveBrokenSword();
        
        CanvasGroup blackScreenCanvas = null;
        CanvasGroup blackScreenTextCanvas = null;
        
        if (blackScreen != null)
        {
            blackScreenCanvas = blackScreen.GetComponent<CanvasGroup>();
            if (blackScreenCanvas == null)
            {
                blackScreenCanvas = blackScreen.AddComponent<CanvasGroup>();
            }
            blackScreenCanvas.alpha = 0f;
            blackScreen.SetActive(true);
            Debug.Log("[RugerFinalRitual] 开始淡入");
        }
        
        if (blackScreenText != null)
        {
            blackScreenTextCanvas = blackScreenText.GetComponent<CanvasGroup>();
            if (blackScreenTextCanvas == null)
            {
                blackScreenTextCanvas = blackScreenText.gameObject.AddComponent<CanvasGroup>();
            }
            blackScreenTextCanvas.alpha = 0f;
            blackScreenText.text = "铛……铛……铛……";
            blackScreenText.gameObject.SetActive(true);
        }
        
        if (audioSource != null && hammerSound != null)
        {
            audioSource.PlayOneShot(hammerSound);
            Debug.Log("[RugerFinalRitual] 播放打铁音效");
        }
        
        // 1. 黑屏淡入
        if (blackScreenCanvas != null)
        {
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                blackScreenCanvas.alpha = t / fadeDuration;
                yield return null;
            }
            blackScreenCanvas.alpha = 1f;
            Debug.Log("[RugerFinalRitual] 黑屏淡入完成");
        }
        
        // 让鲁格消失
        if (rugerGameObject != null)
        {
            rugerGameObject.SetActive(false);
            Debug.Log("[RugerFinalRitual] 鲁格已消失");
        }
        
        yield return new WaitForSeconds(0.5f);
        
        // 2. 文字淡入
        if (blackScreenTextCanvas != null)
        {
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                blackScreenTextCanvas.alpha = t / fadeDuration;
                yield return null;
            }
            blackScreenTextCanvas.alpha = 1f;
            Debug.Log("[RugerFinalRitual] 文字淡入完成");
        }
        
        // 3. 文字停留显示
        yield return new WaitForSeconds(1.5f);
        
        // 4. 文字淡出
        if (blackScreenTextCanvas != null)
        {
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                blackScreenTextCanvas.alpha = 1f - (t / fadeDuration);
                yield return null;
            }
            blackScreenTextCanvas.alpha = 0f;
            blackScreenText.gameObject.SetActive(false);
            Debug.Log("[RugerFinalRitual] 文字淡出完成");
        }
        
        if (kanesWillPrefab != null && anvilTransform != null)
        {
            Instantiate(kanesWillPrefab, anvilTransform.position, Quaternion.identity);
            Debug.Log("[RugerFinalRitual] 生成道具：凯恩的遗志");
        }
        
        if (blackScreenText != null)
        {
            blackScreenText.gameObject.SetActive(false);
        }
        
        yield return new WaitForSeconds(0.5f);
        
        if (blackScreenCanvas != null)
        {
            Debug.Log("[RugerFinalRitual] 开始淡出");
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                blackScreenCanvas.alpha = 1f - (t / fadeDuration);
                yield return null;
            }
            blackScreenCanvas.alpha = 0f;
            blackScreen.SetActive(false);
            Debug.Log("[RugerFinalRitual] 淡出完成");
        }
        
        GameManager.Instance.SetFlag("Has_Kanes_Will", true);
        Debug.Log("【系统】获得道具：凯恩的遗志");
    }

    public void CloseAll()
    {
        if (menuPanel != null)
            menuPanel.SetActive(false);
        if (subtitlePanel != null)
            subtitlePanel.SetActive(false);
        isDialogueActive = false;
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        GameManager.Instance.SetDialogueActive(false);
        OnDialogueClosed?.Invoke();
    }
}
