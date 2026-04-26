using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public class Boss : Character
{
    private Animator anim;
    private NavMeshAgent agent;

    [Header("目标")]
    public Transform playerTarget;

    [Header("移动参数")]
    public float activationRange = 20f;
    public float optimalDistance = 8f;
    public float chaseSpeed = 5f;
    public float repositionSpeed = 3.5f;
    public float actionCooldown = 3f;

    [Header("破防系统")]
    public float staggerThreshold = 100f; // 破防上限
    public float currentStagger = 0f;    // 当前累计的架势值（面板可见）

    [HideInInspector] public bool isExecutingSkill { get; private set; }

    private BossSkill currentActiveSkill;
    private BossSkill pendingSkill;

    private bool isMovingToAttackDistance = false;
    private float attackDistanceTolerance = 0.5f;

    private float lastActionTime = -999f;
    private List<BossSkill> skills;
    private BossMoveController moveController;
    private BossSkillSelector skillSelector;

    private bool isInStagger = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // 初始化血量（使用父类 Character 的属性）
        CurrentHP = MaxHP;

        agent.updateRotation = false;
        agent.stoppingDistance = 0f;
        agent.acceleration = 25f;
        agent.autoBraking = false;
        lastActionTime = Time.time;
        anim.applyRootMotion = false;

        skills = GetComponents<BossSkill>().ToList();
        foreach (var s in skills)
            s.Initialize(this);

        moveController = new BossMoveController(this, agent);
        skillSelector = new BossSkillSelector(this, skills);

        if (playerTarget == null)
            playerTarget = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    // ================= [受击逻辑：参考小兵逻辑修改] =================

    // 1. 触发器检测玩家的武器
    public void OnTriggerEnter(Collider other)
    {
        // 这里的标签 "PlayerAttack" 必须与你小兵脚本里的完全一致
        if (other.CompareTag("PlayerAttack"))
        {
            // 获取玩家武器上的 Damage 脚本
            Damage playerDamage = other.GetComponent<Damage>();
            if (playerDamage != null)
            {
                TakeDamage(playerDamage.damage);
            }
        }
    }

    // 2. 处理扣血、架势条和受伤逻辑
    public void TakeDamage(int damageAmount)
    {
        if (IsDead || CurrentHP <= 0) return;

        // 扣血
        CurrentHP -= damageAmount;

        // 累计架势条 (1:1 转换)
        currentStagger += damageAmount;

        Debug.Log($"Boss受击: {damageAmount}, 剩余血量: {CurrentHP}, 当前架势: {currentStagger}");

        // 死亡检测
        if (CurrentHP <= 0)
        {
            Die();
            return;
        }

        // 检查是否霸体（霸体状态下不触发破防动画）
        bool isHyper = isExecutingSkill &&
                       currentActiveSkill != null &&
                       currentActiveSkill.isHyperArmor;

        // 破防逻辑：如果不在霸体且架势条满了
        if (!isHyper && currentStagger >= staggerThreshold)
        {
            currentStagger = 0f; // 重置架势条
            TriggerBreak();      // 播放 DoHit 并停止动作
        }
    }
    // ======================================================

    void Update()
    {
        if (IsDead || playerTarget == null) return;

        // 修复受击瞬移Bug：硬直时不强行同步位置
        if (!isInStagger)
        {
            transform.position = agent.nextPosition;
        }
        else
        {
            agent.nextPosition = transform.position;
        }

        float currentDistance = Vector3.Distance(transform.position, playerTarget.position);

        if (isInStagger)
        {
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
            if (!anim.IsInTransition(0) && state.normalizedTime >= 1f)
                EndStaggerInternal();

            UpdateAnimator();
            return;
        }

        if (isMovingToAttackDistance && pendingSkill != null)
        {
            float moveDistance = Vector3.Distance(transform.position, playerTarget.position);

            if (Mathf.Abs(moveDistance - pendingSkill.attackDistance) <= attackDistanceTolerance)
            {
                isMovingToAttackDistance = false;
                ExecuteSkill(pendingSkill);
                pendingSkill = null;
                return;
            }

            Vector3 dir = (transform.position - playerTarget.position).normalized;
            Vector3 targetPos = playerTarget.position + dir * pendingSkill.attackDistance;

            agent.speed = repositionSpeed;
            agent.SetDestination(targetPos);

            FaceTarget(8f);
            UpdateAnimator();
            return;
        }

        if (isExecutingSkill)
        {
            FaceTarget(10f);

            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
            if (!anim.IsInTransition(0) && state.normalizedTime >= 0.98f)
                OnSkillEnd();

            UpdateAnimator();
            return;
        }

        if (currentDistance > activationRange)
        {
            agent.isStopped = true;
            UpdateAnimator();
            return;
        }

        if (Time.time >= lastActionTime + actionCooldown)
        {
            var skill = skillSelector.ChooseSkill(currentDistance);
            if (skill != null)
            {
                pendingSkill = skill;
                isMovingToAttackDistance = true;
                return;
            }
        }

        agent.isStopped = false;
        FaceTarget(8f);
        moveController.Tick(currentDistance, playerTarget, optimalDistance, chaseSpeed, repositionSpeed);

        UpdateAnimator();
    }

    void OnAnimatorMove()
    {
        if (!isInStagger) return;

        agent.nextPosition += anim.deltaPosition;
        transform.rotation *= anim.deltaRotation;
    }

    public void GetParried()
    {
        if (!isExecutingSkill || currentActiveSkill == null)
            return;

        if (currentActiveSkill.isHyperArmor)
            return;

        StartStaggerLogic();
        anim.SetTrigger("DoStagger_Minor");
    }

    private void TriggerBreak()
    {
        StartStaggerLogic();
        anim.SetTrigger("DoHit");
    }

    private void StartStaggerLogic()
    {
        isExecutingSkill = false;
        currentActiveSkill = null;
        isInStagger = true;

        agent.isStopped = true;
        agent.ResetPath();
        agent.velocity = Vector3.zero;
        agent.nextPosition = transform.position;
    }

    private void EndStaggerInternal()
    {
        isInStagger = false;
        agent.nextPosition = transform.position;
        agent.isStopped = false;
    }

    private void ExecuteSkill(BossSkill skill)
    {
        isExecutingSkill = true;
        currentActiveSkill = skill;

        agent.isStopped = true;
        agent.ResetPath();
        agent.velocity = Vector3.zero;
        agent.nextPosition = transform.position;

        skill.Use();
        lastActionTime = Time.time;
    }

    public void OnSkillStart(BossSkill skill)
    {
        isExecutingSkill = true;
        currentActiveSkill = skill;
        agent.isStopped = true;
    }

    public void OnSkillEnd()
    {
        isExecutingSkill = false;
        currentActiveSkill = null;
        agent.isStopped = false;
    }

    public override void Die()
    {
        agent.isStopped = true;
        IsDead = true;
        anim.SetTrigger("DoDeath");
    }

    void FaceTarget(float speed)
    {
        Vector3 dir = (playerTarget.position - transform.position);
        dir.y = 0;
        if (dir.sqrMagnitude < 0.01f) return;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(dir),
            Time.deltaTime * speed);
    }

    void UpdateAnimator()
    {
        Vector3 local = transform.InverseTransformDirection(agent.velocity);

        anim.SetFloat("VelocityX", local.x / chaseSpeed, 0.15f, Time.deltaTime);
        anim.SetFloat("VelocityZ", local.z / chaseSpeed, 0.15f, Time.deltaTime);
    }
}