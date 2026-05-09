using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour, ISaveable
{
    public WeaponData weaponData;
     
    public string uniqueId;
    public bool isPickedUp = false;
    
    void Start()
    {
       
        var child = FindChildWithTag(this.transform, "PlayerAttack");

        if (child != null)
        {
            Collider col = child.GetComponent<Collider>();

            if (col != null)
            {
                col.enabled = false;
            }
        }
    }

    Transform FindChildWithTag(Transform parent, string tag)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tag))
            {
                return child;
            }

            Transform result = FindChildWithTag(child, tag);

            if (result != null)
                return result;
        }

        return null;
    }

    // ================= 拾取 =================

    public void PickUp()
    {
        isPickedUp = true;

        HideItem();
    }

    // ================= 隐藏 =================

    private void HideItem()
    {
        gameObject.SetActive(false);
    }

    private void ShowItem()
    {
        gameObject.SetActive(true);
    }

    // ================= 唯一ID =================

    public string GetUniqueID()
    {
        return uniqueId;
    }

    // ================= 保存 =================

    [System.Serializable]
    class SaveData
    {
        public bool pickedUp;
    }

    public string CaptureState()
    {
        SaveData data = new SaveData();

        data.pickedUp = isPickedUp;

        return JsonUtility.ToJson(data);
    }

    // ================= 读取 =================

    public void RestoreState(string json)
    {
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        isPickedUp = data.pickedUp;

        if (isPickedUp)
        {
            HideItem();
        }
        else
        {
            ShowItem();
        }
    }
}