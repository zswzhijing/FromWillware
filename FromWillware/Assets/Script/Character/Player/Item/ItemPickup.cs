using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour, ISaveable
{
    public Item ItemData;

    // 是否已经被拾取
    public bool isPickedUp = false;

    private Collider itemCollider;
    private Renderer[] renderers;

    private void Awake()
    {
        itemCollider = GetComponent<Collider>();

        // 获取自身和子物体所有 Renderer
        renderers = GetComponentsInChildren<Renderer>();
    }

    // 玩家拾取时调用
    public void PickUp()
    {
        isPickedUp = true;

        HideItem();
    }

    // 隐藏物品
    private void HideItem()
    {
        // 关闭碰撞
        if (itemCollider != null)
        {
            itemCollider.enabled = false;
        }

        // 关闭模型
        foreach (Renderer r in renderers)
        {
            r.enabled = false;
        }
        
        gameObject.SetActive(false);
    }

    // 显示物品
    private void ShowItem()
    {
        if (itemCollider != null)
        {
            itemCollider.enabled = true;
        }

        foreach (Renderer r in renderers)
        {
            r.enabled = true;
        }
    }

    // ================= 唯一ID =================

    public string GetUniqueID()
    {
        return GetComponent<SaveableEntity>().UniqueID;
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