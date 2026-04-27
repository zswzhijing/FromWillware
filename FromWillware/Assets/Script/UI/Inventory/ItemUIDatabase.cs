using UnityEngine;
using System.Collections.Generic;

// 这个脚本用来在外部手动指定物品的图标和描述
[CreateAssetMenu(fileName = "ItemUIDatabase", menuName = "Inventory/UI Database")]
public class ItemUIDatabase : ScriptableObject
{
    [System.Serializable]
    public struct ItemUIInfo
    {
        public string itemName; // 对应 Item 里的 Name
        public Sprite icon;
        [TextArea] public string description;
    }

    public List<ItemUIInfo> itemInfos;

    // 辅助方法：通过名字查找图标
    public Sprite GetIcon(string name)
    {
        var info = itemInfos.Find(x => x.itemName == name);
        return info.icon;
    }

    // 辅助方法：通过名字查找描述
    public string GetDescription(string name)
    {
        var info = itemInfos.Find(x => x.itemName == name);
        return info.description;
    }
}