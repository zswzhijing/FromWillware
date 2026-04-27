using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance;

    [Header("绑定外部背包数据")]
    // 如果你不想在面板里手动填，保留 FindObjectOfType 逻辑即可
    public ConsumableBackPack backPack;

    [Header("UI 引用")]
    public GameObject inventoryPanel;
    public Transform slotsParent;
    public GameObject slotPrefab;
    public Button closeButton;
    public RectTransform selectionBox;

    public InventorySlotUI[] slots;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        slots = new InventorySlotUI[27];
        for (int i = 0; i < 27; i++)
        {
            GameObject slotGO = Instantiate(slotPrefab, slotsParent);
            slots[i] = slotGO.GetComponent<InventorySlotUI>();
            slots[i].slotIndex = i;
        }

        closeButton.onClick.AddListener(CloseInventory);
        inventoryPanel.SetActive(false);

        if (selectionBox != null)
            selectionBox.gameObject.SetActive(false);
    }

    void Update()
    {
        // 如果槽位没填，尝试自动找场景里的 ConsumableBackPack
        if (backPack == null)
        {
            backPack = FindObjectOfType<ConsumableBackPack>();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }

        if (inventoryPanel.activeSelf && backPack != null)
        {
            RefreshUI();
        }
    }

    void ToggleInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
        UpdateMouseCursor();
    }

    void CloseInventory()
    {
        inventoryPanel.SetActive(false);
        UpdateMouseCursor();
    }

    void UpdateMouseCursor()
    {
        Cursor.visible = inventoryPanel.activeSelf;
        Cursor.lockState = inventoryPanel.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < backPack.Items.Count && backPack.Items[i] != null && backPack.Items[i].item != null)
            {
                // 现在直接读取 Item 里的 Icon
                slots[i].UpdateSlot(backPack.Items[i].item, backPack.Items[i].CurrentCount);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }

    public void SwapItems(int indexA, int indexB)
    {
        if (backPack == null) return;
        var temp = backPack.Items[indexA];
        backPack.Items[indexA] = backPack.Items[indexB];
        backPack.Items[indexB] = temp;
        RefreshUI();
    }

    public void MoveSelectionBox(Transform targetTransform)
    {
        if (selectionBox == null) return;
        selectionBox.gameObject.SetActive(true);
        selectionBox.position = targetTransform.position;
    }
}