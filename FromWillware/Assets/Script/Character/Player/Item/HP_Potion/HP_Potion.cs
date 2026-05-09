using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Item/HP Potion")]
public class HP_Potion : Item
{
    public int RecoveryHP = 30;
    public GameObject HealEffect;
    public Transform HealPos;
    
    public override void Fun(Player player)
    {
       HealPos = player.transform;
        player.PlayDrinkAnim();
        Instantiate(HealEffect, HealPos.position, Quaternion.identity);
        if (player.CurrentHP + RecoveryHP < player.MaxHP)
        {
            player.CurrentHP += RecoveryHP;
        }
        else
        {
            player.CurrentHP = player.MaxHP;
        }
    }
}