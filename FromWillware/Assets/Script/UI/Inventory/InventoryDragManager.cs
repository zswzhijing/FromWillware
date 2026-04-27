using UnityEngine;

public class InventoryDragManager : MonoBehaviour
{
    public static InventoryDragManager Instance;

    private InventorySlotUI currentlyDraggedSlot;

    private void Awake()
    {
        Instance = this;
    }

    // 这里把 ItemData 改成了 Item
    public void StartDrag(InventorySlotUI slot, Item item, int amount)
    {
        currentlyDraggedSlot = slot;
    }

    public InventorySlotUI GetDraggedSlot()
    {
        return currentlyDraggedSlot;
    }

    public void EndDrag()
    {
        currentlyDraggedSlot = null;
    }
}