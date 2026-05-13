using UnityEngine;
using System;
using System.Collections.Generic;

namespace AshenThrone
{
    [Flags]
    public enum MainQuestPhase
    {
        None = 0,
        PhaseA = 1 << 0,  // 从墓窖出来第一次见面
        PhaseB = 1 << 1,   // 打通矿道进入王城，但未获得真相
        PhaseC = 1 << 2,   // 获得枯萎的花后，知晓真相
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public bool IsDialogueActive { get; private set; }
        public bool IsSettingsOpen { get; private set; }
        public bool IsBackpackOpen { get; private set; }

        public MainQuestPhase MainQuestPhase { get; private set; }

        private Dictionary<string, bool> gameFlags = new Dictionary<string, bool>();

        [Header("剧情道具")]
        public BloodOrder bloodOrder;
        public BrokenSword brokenSword;
        public WitheredFlower witheredFlower;
        public KanesWill kanesWill;

        private ConsumableBackPack backpack;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeDefaultFlags();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            backpack = FindObjectOfType<ConsumableBackPack>();
        }

        private void InitializeDefaultFlags()
        {
            SetFlag("Has_Blood_Order", false);
            SetFlag("Has_Withered_Flower", false);
            SetFlag("Has_Broken_Sword", false);
            SetFlag("Has_Kanes_Will", false);
            MainQuestPhase = MainQuestPhase.PhaseA;
        }

        public void SetFlag(string flagName, bool value)
        {
            if (gameFlags.ContainsKey(flagName))
            {
                gameFlags[flagName] = value;
            }
            else
            {
                gameFlags.Add(flagName, value);
            }
            Debug.Log($"[GameManager] 设置状态: {flagName} = {value}");

            if (flagName == "Has_Withered_Flower" && value)
            {
                SetMainQuestPhase(MainQuestPhase.PhaseC);
            }
        }

        public bool GetFlag(string flagName)
        {
            if (string.IsNullOrEmpty(flagName))
                return true;

            if (gameFlags.ContainsKey(flagName))
            {
                return gameFlags[flagName];
            }

            return false;
        }

        public void SetDialogueActive(bool active)
        {
            IsDialogueActive = active;
            UpdateCursorState();
        }

        public void SetSettingsOpen(bool open)
        {
            IsSettingsOpen = open;
            UpdateCursorState();
        }

        public void SetBackpackOpen(bool open)
        {
            IsBackpackOpen = open;
            UpdateCursorState();
        }

        private void UpdateCursorState()
        {
            bool needCursor = IsDialogueActive || IsSettingsOpen || IsBackpackOpen;

            if (needCursor)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        public void SetMainQuestPhase(MainQuestPhase phase)
        {
            MainQuestPhase = phase;
            Debug.Log($"[GameManager] 主线进度切换至: {phase}");
        }

        // ==================== 剧情道具相关 ====================

        private bool GiveItem(Item item)
        {
            if (item == null)
            {
                Debug.LogError("[GameManager] 剧情道具为空！");
                return false;
            }

            if (backpack == null)
            {
                backpack = FindObjectOfType<ConsumableBackPack>();
            }

            if (backpack == null)
            {
                Debug.LogError("[GameManager] 找不到 ConsumableBackPack！");
                return false;
            }

            bool success = backpack.AddItem(item);
            if (success)
            {
                Debug.Log($"[GameManager] 获得剧情道具：{item.Name}");
            }
            return success;
        }

        public bool HasPlotItem(string itemName)
        {
            if (backpack == null) return false;

            foreach (var stack in backpack.Items)
            {
                if (stack != null && stack.item != null && stack.item.Name == itemName)
                {
                    return true;
                }
            }
            return false;
        }

        public void GiveKaneDefeatItems()
        {
            GiveItem(bloodOrder);
            GiveItem(brokenSword);
            SetFlag("Has_Blood_Order", true);
            SetFlag("Has_Broken_Sword", true);
        }

        public void GiveLienQuestItem()
        {
            GiveItem(witheredFlower);
            SetFlag("Has_Withered_Flower", true);
        }

        public void GiveKanesWill()
        {
            GiveItem(kanesWill);
            SetFlag("Has_Kanes_Will", true);
        }

        // 移除断锤（鲁格最终仪式时调用）
        public void RemoveBrokenSword()
        {
            if (backpack == null)
            {
                backpack = FindObjectOfType<ConsumableBackPack>();
            }

            if (backpack != null && brokenSword != null)
            {
                bool removed = backpack.RemoveItemByReference(brokenSword);
                if (removed)
                {
                    SetFlag("Has_Broken_Sword", false);
                    Debug.Log("[GameManager] 已移除断锤");
                }
            }
        }

        // ========================================================

        private void Update()
        {
            #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F1))
            {
                SetMainQuestPhase(MainQuestPhase.PhaseA);
                Debug.Log("【调试】切换到 PhaseA - 初次见面");
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
                SetMainQuestPhase(MainQuestPhase.PhaseB);
                Debug.Log("【调试】切换到 PhaseB - 打通矿道");
            }
            if (Input.GetKeyDown(KeyCode.F3))
            {
                SetMainQuestPhase(MainQuestPhase.PhaseC);
                Debug.Log("【调试】切换到 PhaseC - 知晓真相");
            }
            if (Input.GetKeyDown(KeyCode.F4))
            {
                GiveKaneDefeatItems();
                Debug.Log("【调试】发放击败凯恩的道具");
            }
            if (Input.GetKeyDown(KeyCode.F5))
            {
                GiveLienQuestItem();
                Debug.Log("【调试】发放枯萎的花");
            }
            if (Input.GetKeyDown(KeyCode.F6))
            {
                GiveKanesWill();
                Debug.Log("【调试】发放凯恩的遗志");
            }
            if (Input.GetKeyDown(KeyCode.F7))
            {
                gameFlags.Clear();
                InitializeDefaultFlags();
                Debug.Log("【调试】重置所有状态");
            }
            if (Input.GetKeyDown(KeyCode.F8))
            {
                Player player = FindObjectOfType<Player>();
                if (player != null)
                {
                    player.Die();
                    Debug.Log("【调试】触发玩家死亡");
                }
            }
            #endif
        }
    }
}
