using UnityEngine;

[CreateAssetMenu(menuName = "Item/Plot/Blood Order", fileName = "Blood Order")]
public class BloodOrder : PlotItem
{
    private void Awake()
    {
        Name = "带血的军令";
        Description = "一张被鲜血浸透的羊皮纸，上面是凯恩下达的最高禁卫军令。";
    }
}