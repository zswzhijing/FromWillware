using UnityEngine;

namespace AshenThrone.DialogueSystem
{
    [CreateAssetMenu(fileName = "New_NPC_Data", menuName = "AshenThrone/NPC Data")]
    public class NPCData : ScriptableObject
    {
        [Header("NPC基础信息")]
        public string npcName;

        [Tooltip("勾选代表有话题菜单，不勾选代表靠近直接说话（边缘NPC）")]
        public bool hasTopicMenu = true;

        [Header("问候语")]
        [TextArea(2, 4)]
        public string greetingText;

        [Header("话题列表")]
        public TopicData[] topics;
    }
}