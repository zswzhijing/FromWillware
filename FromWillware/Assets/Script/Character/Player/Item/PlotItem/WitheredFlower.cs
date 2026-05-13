using UnityEngine;

[CreateAssetMenu(menuName = "Item/Plot/Withered Flower", fileName = "Withered Flower")]
public class WitheredFlower : PlotItem
{
    private void Awake()
    {
        Name = "枯萎的花";
        Description = "一朵枯萎的紫罗兰，却是莉恩先祖死时紧紧握住的信物。";
    }
}