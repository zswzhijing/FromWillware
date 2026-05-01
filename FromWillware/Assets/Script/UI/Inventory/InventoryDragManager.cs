using UnityEngine;
using UnityEngine.UI;

public class InventoryDragManager : MonoBehaviour
{
    public static InventoryDragManager Instance;

    [Header("拖拽时跟着鼠标跑的图标")]
    public Image dragIconImage; 

    private InventorySlotUI draggedSlot; // 记录当前正在拖拽的是哪个格子

    void Awake()
    {
        Instance = this;
        
        if (dragIconImage != null)
        {
            dragIconImage.enabled = false;
            // 【极其重要】拖拽图标绝对不能挡住鼠标射线，否则放不下！
            dragIconImage.raycastTarget = false; 
        }
    }

    void Update()
    {
        // 如果正在拖拽，让这个图片始终等于鼠标所在的屏幕位置
        if (draggedSlot != null && dragIconImage != null && dragIconImage.enabled)
        {
            dragIconImage.transform.position = Input.mousePosition;
        }
    }

    // 当鼠标在格子上按下并拖拽时触发
    public void StartDrag(InventorySlotUI slot, Item item, int amount)
    {
        draggedSlot = slot;

        if (dragIconImage != null && item != null)
        {
            dragIconImage.sprite = item.Icon;
            dragIconImage.enabled = true;
            
            // 强行把这个跟随图标放到 Canvas 的最后面（也就是屏幕最上层），防止被别的UI挡住
            dragIconImage.transform.SetAsLastSibling();
        }
    }

    // 鼠标松开时触发
    public void EndDrag()
    {
        draggedSlot = null;
        if (dragIconImage != null)
        {
            dragIconImage.enabled = false;
            dragIconImage.sprite = null;
        }
    }

    // 给其他格子获取拖拽来源的方法
    public InventorySlotUI GetDraggedSlot()
    {
        return draggedSlot;
    }
}