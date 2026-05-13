using UnityEngine;

[CreateAssetMenu(menuName = "Item/Plot/Broken Sword", fileName = "Broken Sword")]
public class BrokenSword : PlotItem
{
    private void Awake()
    {
        Name = "凯恩的断剑";
        Description = "凯恩曾经使用的武器，现在只剩下残缺的碎片。";
    }
}