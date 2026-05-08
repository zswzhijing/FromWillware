using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBackPack : BackPack,ISaveable
{
    // Start is called before the first frame update
    public List<WeaponData> Weapons;
    public Transform WeaponPoint;

    private WeaponSystem weaponSystem;
    private WeaponPickup nearbyWeapon;
    private PlayerInputHandler inputHandler;
    void Start()
    {
        CurrentIndex = 0;
        CurrentSize = 0;
        weaponSystem = GetComponent<WeaponSystem>();
        inputHandler = GetComponent<PlayerInputHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        WeaponPickUp();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            nearbyWeapon = other.GetComponent<WeaponPickup>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            nearbyWeapon = null;
        }
    }

    public void WeaponPickUp()
    {
        if (nearbyWeapon != null &&
            (Input.GetKeyDown(KeyCode.E)
             || inputHandler.interactPressed))
        {
            // 先尝试加入背包
            bool success =
                WeaponAdd(nearbyWeapon.weaponData);

            if (success)
            {
                // 再生成武器
                weaponSystem.AddWeapon(
                    nearbyWeapon.weaponData);

                // 最后再标记已拾取
                nearbyWeapon.isPickedUp = true;

                nearbyWeapon.gameObject.SetActive(false);

                nearbyWeapon = null;
            }
        }
    }

    public bool WeaponAdd(WeaponData data)
    {
        if (Weapons.Count >= MaxSize)
        {
            Debug.Log("BackPack is full");
            return false;
        }

        Weapons.Add(data);

        return true;
    }

    public string GetUniqueID()
    {
        return "WeaponBackPack";
    }

    // ================= SAVE =================
    public string CaptureState()
    {
        WeaponBackPackSaveData saveData =
            new WeaponBackPackSaveData();

        foreach (var weapon in Weapons)
        {
            saveData.weaponIDs.Add(weapon.Name);
        }

        return JsonUtility.ToJson(saveData);
    }

    // ================= LOAD =================
    public void RestoreState(string json)
    {
        WeaponBackPackSaveData saveData =
            JsonUtility.FromJson<WeaponBackPackSaveData>(json);

        Weapons.Clear();

        foreach (string id in saveData.weaponIDs)
        {
            if (WeaponDatabase.dict.ContainsKey(id))
            {
                WeaponData data = WeaponDatabase.dict[id];

                Weapons.Add(data);

                // 恢复 WeaponSystem
                weaponSystem.AddWeapon(data);
            }
            else
            {
                Debug.LogWarning("Weapon not found: " + id);
            }
        }
    }

    
}
