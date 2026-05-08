using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using System.Text;

public class UI_WeaponBackpackManager : MonoBehaviour
{
    // === 关键修改：添加静态实例 ===
    public static UI_WeaponBackpackManager Instance;

    [Header("必须绑定的引用")]
    public WeaponSystem weaponSystem;
    public WeaponBackPack weaponBackPack;
    public GameObject inventoryPanel;
    public TextMeshProUGUI listText;
    public TextMeshProUGUI detailsText;

    private Dictionary<WeaponData, Transform> equippedWeapons = new Dictionary<WeaponData, Transform>();

    public bool isMenuOpen = false;
    private int selectedIndex = 0;
    private int lastWeaponCount = 0;

    void Awake()
    {
        Instance = this; // 初始化实例
    }

    void Start()
    {
        inventoryPanel.SetActive(false);
    }

    void Update()
    {
        // === 1. Q键：开关背包 ===
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isMenuOpen = !isMenuOpen;
            inventoryPanel.SetActive(isMenuOpen);

            if (isMenuOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                if (weaponBackPack.Weapons.Count > 0 && selectedIndex >= weaponBackPack.Weapons.Count)
                    selectedIndex = 0;
                RefreshUI();
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        // === 2. 菜单打开时的输入逻辑 ===
        if (isMenuOpen)
        {
            if (lastWeaponCount != weaponBackPack.Weapons.Count) RefreshUI();

            if (weaponBackPack.Weapons.Count > 0)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    selectedIndex = (selectedIndex - 1 + weaponBackPack.Weapons.Count) % weaponBackPack.Weapons.Count;
                    RefreshUI();
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    selectedIndex = (selectedIndex + 1) % weaponBackPack.Weapons.Count;
                    RefreshUI();
                }
                else if (Input.GetKeyDown(KeyCode.R))
                {
                    ToggleEquip(weaponBackPack.Weapons[selectedIndex]);
                }
            }
        }
    }

     private void ToggleEquip(WeaponData data)
    {
        Transform foundTransform = null;
        // 查找逻辑
        foreach (Transform t in weaponSystem.Weapons)
        {
            if (t != null && t.GetComponent<Weapon>().Name == data.Name)
            {
                foundTransform = t;
                break;
            }
        }

        if (foundTransform != null)
        {
            // 卸下逻辑
            weaponSystem.Weapons.Remove(foundTransform);
            Destroy(foundTransform.gameObject);
            if (weaponSystem.CurrentWeapon == foundTransform) weaponSystem.CurrentWeapon = null;
            if (weaponSystem.Weapons.Count > 0) weaponSystem.EquipWeapon(0);
        }
        else
        {
            // 装备逻辑
            if (weaponSystem.Weapons.Count < weaponSystem.MaxSzie)
            {
                weaponSystem.AddWeapon(data);
            }
            else
            {
                Debug.LogWarning("装备栏已满！");
            }
        }
        RefreshUI();
    }
     private void RefreshUI()
    {
        // 1. 安全检查
        if (weaponBackPack == null || weaponSystem == null) return;

        lastWeaponCount = weaponBackPack.Weapons.Count;
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("<color=#FFD700>=== 武器背包 ===</color>");
        sb.AppendLine("<size=24><color=#888888>[Q 关闭 | W/S 滚动 | R 装备/卸下]</color></size>\n");

        for (int i = 0; i < weaponBackPack.Weapons.Count; i++)
        {
            WeaponData wData = weaponBackPack.Weapons[i];

            // --- 核心修改：实时查找 ---
            // 直接遍历快捷栏列表，对比名字来判断是否已装备
            bool isEquipped = false;
            foreach (Transform t in weaponSystem.Weapons)
            {
                if (t != null)
                {
                    Weapon wComp = t.GetComponent<Weapon>();
                    // 如果名字匹配，说明该武器已在快捷栏中
                    if (wComp != null && wComp.Name == wData.Name)
                    {
                        isEquipped = true;
                        break;
                    }
                }
            }

            bool isSelected = (i == selectedIndex);

            // 构造显示文本
            string cursor = isSelected ? "<color=#FFD700>▶ </color>" : "  ";
            string status = isEquipped ? "<color=#00FF00>[E]</color> " : "<color=#888888>[ ] </color>";
            string name = isSelected ? $"<color=#FFFFFF><u>{wData.Name}</u></color>" : $"<color=#AAAAAA>{wData.Name}</color>";

            sb.AppendLine($"{cursor}{status}{name}");
        }

        // 4. 给 UI 赋值
        listText.text = sb.ToString();

        // 5. 刷新右侧详情文字
        if (weaponBackPack.Weapons.Count > 0)
        {
            WeaponData currentData = weaponBackPack.Weapons[selectedIndex];
            detailsText.text = $"<size=50><color=#FFD700>【 {currentData.Name} 】</color></size>\n\n" +
                               $"<size=36><b>伤害:</b> <color=#FF5555>{currentData.Damage}</color></size>\n" +
                               $"<size=36><b>耗力:</b> <color=#55AAFF>{currentData.ConsumingStamina}</color></size>\n\n" +
                               $"<color=#DDDDDD><i>{currentData.Introduction}</i></color>";
        }
        else
        {
            detailsText.text = "背包里空空如也...";
        }
    }
}