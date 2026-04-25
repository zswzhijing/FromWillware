using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableBackPack : BackPack
{
    // Start is called before the first frame update
    public List<ItemStack> Items = new List<ItemStack>();

    private ItemPickup nearbyItem;
    private ItemBar itemBar;
    private ItemPickup itemPickup;
    
    void Start()
    {
        itemBar = GetComponent<ItemBar>();
        //Items = new List<ItemStack>(new ItemStack[MaxSize]);
    }

    // Update is called once per frame
    

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            nearbyItem = other.GetComponent<ItemPickup>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            nearbyItem = null;
        }
    }

    void Update()
    {
        if (nearbyItem != null && Input.GetKeyDown(KeyCode.E))
        {
            AddItem(nearbyItem.ItemData);
            Destroy(nearbyItem.gameObject); // 拾取后消失
            nearbyItem = null;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            AddToItemBar(0,0);
        }
        
        if (Input.GetKeyDown(KeyCode.G))
        {
            RemoveFromItemBar(0);
        }
    }
    
    public void AddItem(Item item)
    {
        // 1. 先堆叠
        foreach (var stack in Items)
        {
            if (stack != null && stack.item == item && stack.CurrentCount < item.MaxCount)
            {
                stack.CurrentCount++;
                return;
            }
        }

        // 2. 新建
        if (Items.Count < MaxSize)
        {
            if(item.Name == "HP_Potion") Items.Add(new ItemStack(item, item.MaxCount));
            Items.Add(new ItemStack(item, 1));
        }
        else
        {
            Debug.Log("背包满");
            return;
        }
    }

    //整理背包
    void TidyBackPack()
    {
        List<ItemStack> newList = new List<ItemStack>();

        // 1️⃣ 收集所有非空物品
        foreach (var stack in Items)
        {
            if (stack != null)
            {
                newList.Add(stack);
            }
        }

        // 2️⃣ 补空位
        while (newList.Count < MaxSize)
        {
            newList.Add(null);
        }

        // 3️⃣ 替换
        Items = newList;
    }
    
    public void RemoveItem(int index, int amount = 1)
    {
        if (Items[index] == null) return;

        Items[index].CurrentCount -= amount;

        if (Items[index].CurrentCount <= 0)
        {
            Items[index] = null;
            TidyBackPack();
        }
    }

    public void AddToItemBar(int BackIndex,int BarIndex)
    {
        if (Items[BackIndex].BarIndex != -1)
        {
            Debug.Log("Is already in bar");
            return;
        }

        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i] != null && Items[i].BarIndex == BarIndex)
            {
                RemoveFromItemBar(i);
                break;
            }
        }
        Items[BackIndex].BarIndex = BarIndex;
        itemBar.Items[BarIndex] =  Items[BackIndex];
    }

    public void RemoveFromItemBar(int BackIndex)
    {
        if (Items[BackIndex].BarIndex == -1)
        {
            Debug.Log("Is not in bar");
            return;
        }
        itemBar.Items[Items[BackIndex].BarIndex] =  null;
        Items[BackIndex].BarIndex = -1;
       
    }

    
}
