using UnityEngine;

public abstract class Item : ScriptableObject
{
    public string Name;
    public ItemKind Kind;
    public int MaxCount;

    // --- 新增这两个字段 ---
    public Sprite Icon;
    [TextArea(3, 5)] public string Description;
    // ---------------------

    public abstract void Fun(Player player);
}