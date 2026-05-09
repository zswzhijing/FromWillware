using System.Collections.Generic;
using UnityEngine;

public class WeaponDatabase : MonoBehaviour
{
    public static Dictionary<string, WeaponData> dict =
        new Dictionary<string, WeaponData>();

    public List<WeaponData> allWeapons;

    private void Awake()
    {
        dict.Clear();

        foreach (var weapon in allWeapons)
        {
            if (!dict.ContainsKey(weapon.Name))
            {
                dict.Add(weapon.Name, weapon);
            }
        }
    }
}