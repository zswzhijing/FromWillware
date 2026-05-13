using UnityEngine;

[CreateAssetMenu(menuName = "Item/Plot Item", fileName = "New Plot Item")]
public abstract class PlotItem : Item
{
    private void Awake()
    {
        Kind = ItemKind.Plot;
        MaxCount = 1;
    }

    public override void Fun(Player player)
    {
        Debug.Log($"【剧情道具】{Name}：{Description}");
    }
}