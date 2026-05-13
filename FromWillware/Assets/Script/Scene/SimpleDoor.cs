using UnityEngine;

public class SimpleDoor : MonoBehaviour
{
    public float openAngle = 90f;       // 打开角度
    public float openSpeed = 2f;        // 打开速度
    public float interactDistance = 3f; // 交互距离
    public Transform doorTransform;     // 门的实际模型（用于判断玩家在哪一侧）
    
    private bool isOpen = false;
    private bool hasOpened = false;     // 标记门是否已经被打开过
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = transform.rotation * Quaternion.Euler(0, openAngle, 0);
        
        // 如果没有指定doorTransform，默认使用自身
        if (doorTransform == null)
        {
            doorTransform = transform;
        }
    }

    void Update()
    {
        // 如果门已经打开过，不再响应交互
        if (hasOpened)
        {
            isOpen = true;
        }
        else
        {
            PlayerInputHandler playerInput = FindObjectOfType<PlayerInputHandler>();
            if (playerInput != null && playerInput.interactPressed)
            {
                float distance = Vector3.Distance(transform.position, playerInput.transform.position);
                if (distance <= interactDistance)
                {
                    // 检查玩家是否在门的正确一侧
                    if (IsPlayerOnCorrectSide(playerInput.transform))
                    {
                        isOpen = true;
                        hasOpened = true; // 标记门已经打开过
                        Debug.Log("门已打开，从此不再关闭");
                    }
                    else
                    {
                        Debug.Log("玩家不在门的可交互侧");
                    }
                }
            }
        }

        // 平滑旋转门
        if (isOpen)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, openRotation, Time.deltaTime * openSpeed);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, closedRotation, Time.deltaTime * openSpeed);
        }
    }

    // 判断玩家是否在门的正确一侧（用门的forward方向判断）
    private bool IsPlayerOnCorrectSide(Transform player)
    {
        Vector3 doorForward = doorTransform.forward;
        Vector3 playerDirection = player.position - doorTransform.position;
        
        // 忽略Y轴高度差异
        doorForward.y = 0;
        playerDirection.y = 0;
        
        // 计算点积：如果点积为正，玩家在门的前方（可交互侧）
        float dot = Vector3.Dot(doorForward.normalized, playerDirection.normalized);
        return dot > 0;
    }

    void OnDrawGizmosSelected()
    {
        // 绘制交互范围
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactDistance);
        
        // 绘制门的可交互侧（绿色箭头表示可交互方向）
        if (doorTransform != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(doorTransform.position, doorTransform.forward * interactDistance);
        }
    }
}