using UnityEngine;
using TMPro;

public class LevelUI : MonoBehaviour
{
    [Header("数据来源")]
    public LevelSystem levelSystem; 

    [Header("UI 组件")]
    public TextMeshProUGUI levelText; // 只保留这一个

    private int lastLevel = -1;

    void Start()
    {
        if (levelSystem == null) levelSystem = FindFirstObjectByType<LevelSystem>();
        RefreshUI();
    }

    void Update()
    {
        if (levelSystem == null) return;

        // 只要等级变了就刷新
        if (levelSystem.level != lastLevel)
        {
            RefreshUI();
            lastLevel = levelSystem.level;
        }
    }

    private void RefreshUI()
    {
        if (levelText != null)
        {
            levelText.text = levelSystem.level.ToString();
        }
    }
}