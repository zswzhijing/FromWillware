using UnityEngine;

public class WallTrigger : MonoBehaviour
{
    public WallTrigger otherWall;
    public Collider wallCollider;
    public Collider triggerCollider;
    public Renderer wallRenderer;
    public Boss boss;
    
    [HideInInspector] public bool hasTriggered = false;
    [HideInInspector] public bool bossDead = false;
    private bool playerEntered = false;
    private float enterSide = 0f;

    void Start()
    {
        SetWallActive(false);
        
        if (triggerCollider != null)
        {
            triggerCollider.enabled = true;
        }
    }

    void Update()
    {
        if (!bossDead && boss != null && boss.IsDead)
        {
            bossDead = true;
            DisableWallPermanently();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (bossDead) return;
        
        if (other.CompareTag("Player") && !hasTriggered)
        {
            Vector3 wallNormal = transform.forward;
            Vector3 toPlayer = other.transform.position - transform.position;
            toPlayer.y = 0;
            
            enterSide = Vector3.Dot(wallNormal, toPlayer.normalized);
            playerEntered = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (bossDead) return;
        
        if (other.CompareTag("Player") && !hasTriggered && playerEntered)
        {
            Vector3 wallNormal = transform.forward;
            Vector3 toPlayer = other.transform.position - transform.position;
            toPlayer.y = 0;
            
            float exitSide = Vector3.Dot(wallNormal, toPlayer.normalized);
            
            if (enterSide > 0 && exitSide < 0)
            {
                TriggerWalls();
            }
            
            playerEntered = false;
        }
    }
    
    void DisableWallPermanently()
    {
        SetWallActive(false);
        
        if (triggerCollider != null)
        {
            triggerCollider.enabled = false;
        }
        
        if (otherWall != null)
        {
            otherWall.SetWallActive(false);
            if (otherWall.triggerCollider != null)
            {
                otherWall.triggerCollider.enabled = false;
            }
            otherWall.bossDead = true;
        }
        
        Debug.Log("Boss死亡，空气墙已永久解除");
    }

    void TriggerWalls()
    {
        hasTriggered = true;
        SetWallActive(true);
        
        if (otherWall != null)
        {
            otherWall.SetWallActive(true);
            otherWall.hasTriggered = true;
        }
        
        Debug.Log("空气墙已激活");
    }

    public void SetWallActive(bool active)
    {
        if (wallCollider != null)
        {
            wallCollider.enabled = active;
        }
        
        if (wallRenderer != null)
        {
            wallRenderer.enabled = active;
            if (active)
            {
                wallRenderer.material.color = Color.black;
            }
            else
            {
                Color c = wallRenderer.material.color;
                c.a = 0;
                wallRenderer.material.color = c;
            }
        }
    }

    public void DestroyWall()
    {
        Destroy(gameObject);
    }
}