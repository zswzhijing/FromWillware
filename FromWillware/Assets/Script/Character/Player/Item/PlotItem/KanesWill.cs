using UnityEngine;

[CreateAssetMenu(menuName = "Item/Plot/Kane's Will", fileName = "Kane's Will")]
public class KanesWill : PlotItem
{
    private void Awake()
    {
        Name = "凯恩的遗志";
        Description = "鲁格重铸的神圣遗物，承载着凯恩未尽的意志。";
    }
}