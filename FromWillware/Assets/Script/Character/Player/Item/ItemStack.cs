using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ItemStack 
{
   public Item item;
   public int CurrentCount;
   public int BarIndex;
   
   public ItemStack(Item item, int count)
   {
      this.item = item;
      this.CurrentCount = count;
      BarIndex = -1;
   }
}
