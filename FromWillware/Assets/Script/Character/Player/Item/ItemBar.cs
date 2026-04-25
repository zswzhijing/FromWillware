using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBar : MonoBehaviour
{
    public List<ItemStack> Items = new List<ItemStack>();
    public int MaxItems = 4;

    private Player player;
    private PlayerMove playerMove;
    private PlayerAttack playerAttack;
    private PlayerParry playerParry;

    void Start()
    {
        player = GetComponent<Player>();
        playerMove = GetComponent<PlayerMove>();
        playerAttack = GetComponent<PlayerAttack>();
        playerParry = GetComponent<PlayerParry>();

        Items.Clear();
        // 初始化固定槽位
        for (int i = 0; i < MaxItems; i++)
        {
            Items.Add(null);
        }
    }

    void Update()
    {
        bool canUse = (!playerMove.IsRolling && 
                       !playerAttack.IsAttacking && 
                       !playerParry.IsDefensing);

        if (!canUse) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) UseItem(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseItem(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) UseItem(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) UseItem(3);
    }

    // ⭐ 使用物品
    public void UseItem(int index)
    {
        if (index < 0 || index >= Items.Count) return;

        ItemStack stack = Items[index];

        if (stack == null || stack.item == null) return;

        if (stack.CurrentCount <= 0)
        {
            Debug.Log("The item is empty");
            return;
        }

        // 使用
        stack.item.Fun(player);

        stack.CurrentCount--;
        
    }

    // ⭐ 从背包加入（关键：引用）
    public void SetItem(int index, ItemStack stack)
    {
        if (index < 0 || index >= MaxItems) return;

        Items[index] = stack; // ✅ 直接引用
    }

    // ⭐ 移除快捷栏物品
    public void RemoveAt(int index)
    {
        if (index < 0 || index >= MaxItems) return;

        Items[index] = null;
    }

    // ⭐ 从所有地方移除（同步）
    void RemoveStack(ItemStack stack)
    {
        // 清快捷栏
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i] == stack)
            {
                Items[i] = null;
            }
        }

        // 👉 如果你有 Backpack，这里也应该调用：
        // backpack.RemoveStack(stack);
    }
}