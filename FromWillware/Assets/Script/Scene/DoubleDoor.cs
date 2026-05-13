using UnityEngine;

public class DoubleDoor : MonoBehaviour
{
    public Transform leftDoor;            // 左门
    public Transform rightDoor;           // 右门
    public float leftOpenAngle = -90f;    // 左门打开角度（通常是负数，向左开）
    public float rightOpenAngle = 90f;    // 右门打开角度（通常是正数，向右开）
    public float openSpeed = 2f;          // 打开速度
    public float interactDistance = 3f;   // 交互距离
    public Transform doorCenter;          // 门中心位置（用于判断玩家在哪一侧）
    
    private bool isOpen = false;
    private bool hasOpened = false;       // 标记门是否已经被打开过
    private Quaternion leftClosedRotation;
    private Quaternion leftOpenRotation;
    private Quaternion rightClosedRotation;
    private Quaternion rightOpenRotation;

    void Start()
    {
        // 保存初始旋转
        if (leftDoor != null)
        {
            leftClosedRotation = leftDoor.rotation;
            leftOpenRotation = leftDoor.rotation * Quaternion.Euler(0, leftOpenAngle, 0);
        }
        
        if (rightDoor != null)
        {
            rightClosedRotation = rightDoor.rotation;
            rightOpenRotation = rightDoor.rotation * Quaternion.Euler(0, rightOpenAngle, 0);
        }
        
        // 如果没有指定doorCenter，默认使用自身
        if (doorCenter == null)
        {
            doorCenter = transform;
        }
    }

    void Update()
    {
        if (hasOpened)
        {
            isOpen = true;
        }
        else
        {
            PlayerInputHandler playerInput = FindObjectOfType<PlayerInputHandler>();
            if (playerInput != null && playerInput.interactPressed)
            {
                float distance = Vector3.Distance(doorCenter.position, playerInput.transform.position);
                if (distance <= interactDistance)
                {
                    if (IsPlayerOnCorrectSide(playerInput.transform))
                    {
                        isOpen = true;
                        hasOpened = true;
                        Debug.Log("双开门已打开，从此不再关闭");
                    }
                    else
                    {
                        Debug.Log("玩家不在门的可交互侧");
                    }
                }
            }
        }

        // 平滑旋转左门
        if (leftDoor != null)
        {
            if (isOpen)
            {
                leftDoor.rotation = Quaternion.Lerp(leftDoor.rotation, leftOpenRotation, Time.deltaTime * openSpeed);
            }
            else
            {
                leftDoor.rotation = Quaternion.Lerp(leftDoor.rotation, leftClosedRotation, Time.deltaTime * openSpeed);
            }
        }

        // 平滑旋转右门
        if (rightDoor != null)
        {
            if (isOpen)
            {
                rightDoor.rotation = Quaternion.Lerp(rightDoor.rotation, rightOpenRotation, Time.deltaTime * openSpeed);
            }
            else
            {
                rightDoor.rotation = Quaternion.Lerp(rightDoor.rotation, rightClosedRotation, Time.deltaTime * openSpeed);
            }
        }
    }

    private bool IsPlayerOnCorrectSide(Transform player)
    {
        Vector3 doorForward = doorCenter.forward;
        Vector3 playerDirection = player.position - doorCenter.position;
        
        doorForward.y = 0;
        playerDirection.y = 0;
        
        float dot = Vector3.Dot(doorForward.normalized, playerDirection.normalized);
        return dot < 0;  // 改为从门的后方开门
    }

    void OnDrawGizmosSelected()
    {
        // 如果doorCenter没有赋值，使用transform作为默认值
        Transform center = doorCenter != null ? doorCenter : transform;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center.position, interactDistance);
        
        Gizmos.color = Color.green;
        Gizmos.DrawRay(center.position, center.forward * interactDistance);
    }
}