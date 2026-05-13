using UnityEngine;
using System;
using AshenThrone;

namespace AshenThrone.DialogueSystem
{
    [Serializable]
    public class TopicData
    {
        [Header("话题基础设置")]
        public string topicName;

        [Tooltip("解锁该话题需要的全局变量名，为空则默认解锁")]
        public string requiredFlag = "";

        [Tooltip("该话题在哪些主线进度阶段显示")]
        public MainQuestPhase visibleInPhases = MainQuestPhase.PhaseA&MainQuestPhase.PhaseB&MainQuestPhase.PhaseC;

        [Tooltip("是否为一次性话题（播完后按钮消失）")]
        public bool isOneTime = false;

        [Header("台词内容")]
        [TextArea(2, 5)]
        public string[] dialogueLines;

        [TextArea(2, 5)]
        [Tooltip("如果为空，则复读dialogueLines的最后一句")]
        public string[] repeatLines;

        [Header("特殊事件")]
        [Tooltip("对话结束时触发的特殊事件名")]
        public string triggerEvent = "";
    }
}