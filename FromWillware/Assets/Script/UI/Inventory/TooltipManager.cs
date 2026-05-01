using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    public GameObject tooltipPanel; // 提示框的背景面板
    public TextMeshProUGUI nameText; // 名字文本
    public TextMeshProUGUI descriptionText; // 描述文本

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        HideTooltip(); // 游戏开始时隐藏
    }

    private void Update()
    {
        // 如果提示框显示着，让它跟随鼠标移动
        if (tooltipPanel.activeSelf)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void ShowTooltip(string itemName, string description)
    {
        tooltipPanel.SetActive(true);
        nameText.text = itemName;
        descriptionText.text = description;
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}