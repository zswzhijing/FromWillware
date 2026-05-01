using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Image iconImage;
    public TextMeshProUGUI amountText;

    public int slotIndex;

    // 修改：这里使用别人脚本里的 Item 类
    private Item currentItem;
    private int currentAmount;

    public void UpdateSlot(Item item, int amount)
    {
        currentItem = item;
        currentAmount = amount;

        if (item != null)
        {
            // 现在直接读 item.Icon，不需要外部传参了
            iconImage.sprite = item.Icon;
            iconImage.enabled = true;

            amountText.text = amount.ToString();
            amountText.enabled = true;
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        currentAmount = 0;
        iconImage.sprite = null;
        iconImage.enabled = false;
        amountText.text = "";
        amountText.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem != null && TooltipManager.Instance != null)
        {
            // 直接显示物品自带的描述
            TooltipManager.Instance.ShowTooltip(currentItem.Name, currentItem.Description);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TooltipManager.Instance != null)
        {
            TooltipManager.Instance.HideTooltip();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && InventoryUIManager.Instance != null)
        {
            InventoryUIManager.Instance.SelectSlot(this.slotIndex);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem != null && InventoryDragManager.Instance != null)
        {
            // 注意这里类型如果不匹配，需要去 InventoryDragManager 里把 ItemData 改成 Item
            InventoryDragManager.Instance.StartDrag(this, currentItem, currentAmount);
            iconImage.color = new Color(1, 1, 1, 0.5f);
        }
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        iconImage.color = new Color(1, 1, 1, 1f);
        if (InventoryDragManager.Instance != null) InventoryDragManager.Instance.EndDrag();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (InventoryDragManager.Instance == null || InventoryUIManager.Instance == null) return;

        InventorySlotUI droppedSlot = InventoryDragManager.Instance.GetDraggedSlot();

        if (droppedSlot != null && droppedSlot != this)
        {
            // 调用我们自己 UIManager 里的方法去强行交换数据
            InventoryUIManager.Instance.SwapItems(droppedSlot.slotIndex, this.slotIndex);
        }
    }
}