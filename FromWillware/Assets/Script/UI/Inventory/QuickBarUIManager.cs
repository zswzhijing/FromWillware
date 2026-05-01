using UnityEngine;
using UnityEngine.UI;

public class QuickBarUIManager : MonoBehaviour
{
    [Header("玩家身上的快捷栏数据")]
    public ItemBar playerItemBar;[Header("四个快捷栏UI的图标Image组件")]
    public Image[] quickBarIcons = new Image[4];

    [Header("对应按键提示(可选)")]
    public Text[] keyTexts = new Text[4];

    void Update()
    {
        // 如果没拖拽，自动寻找玩家身上的 ItemBar
        if (playerItemBar == null)
        {
            playerItemBar = FindObjectOfType<ItemBar>();
            if (playerItemBar == null) return;
        }

        RefreshQuickBarUI();
    }

    void RefreshQuickBarUI()
    {
        for (int i = 0; i < quickBarIcons.Length; i++)
        {
            // 如果超出了数据范围，或者该位置没有物品
            if (i >= playerItemBar.Items.Count || playerItemBar.Items[i] == null || playerItemBar.Items[i].item == null)
            {
                quickBarIcons[i].sprite = null;
                quickBarIcons[i].enabled = false; // 隐藏图标
            }
            else
            {
                // 显示物品图标
                quickBarIcons[i].sprite = playerItemBar.Items[i].item.Icon;
                quickBarIcons[i].enabled = true; // 显示图标
            }
        }
    }
}