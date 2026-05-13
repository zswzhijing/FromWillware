using UnityEngine;
using AshenThrone.DialogueSystem;

namespace AshenThrone
{
    public class NPCInteraction : MonoBehaviour
    {
        [Header("配置该NPC的数据")]
        public NPCData myData;

        private bool isPlayerNear = false;
        private bool isInteracting = false;

        private void Update()
        {
            if (isPlayerNear && !isInteracting && Input.GetKeyDown(KeyCode.E))
            {
                StartInteraction();
            }
        }

        private void StartInteraction()
        {
            if (myData == null)
            {
                Debug.LogError($"[NPCInteraction] {gameObject.name} 没有配置 NPCData!");
                return;
            }

            isInteracting = true;
            
            DialogueManager.Instance.OnDialogueClosed += EndInteraction;

            if (myData.hasTopicMenu)
            {
                Debug.Log($"与 {myData.npcName} 交互：先播放问候语");
                DialogueManager.Instance.PlayGreeting(myData);
            }
            else
            {
                Debug.Log($"与 {myData.npcName} 交互：直接开始说话");

                if (myData.topics.Length > 0)
                {
                    DialogueManager.Instance.StartDirectDialogue(myData);
                }
            }
        }

        public void EndInteraction()
        {
            isInteracting = false;
            
            if (DialogueManager.Instance != null)
                DialogueManager.Instance.OnDialogueClosed -= EndInteraction;
            
            Debug.Log($"与 {myData.npcName} 的交互已结束。");
        }

        private void OnDestroy()
        {
            if (DialogueManager.Instance != null)
                DialogueManager.Instance.OnDialogueClosed -= EndInteraction;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerNear = true;
                Debug.Log("显示提示：按 E 交互");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerNear = false;
                Debug.Log("隐藏提示");
            }
        }
    }
}