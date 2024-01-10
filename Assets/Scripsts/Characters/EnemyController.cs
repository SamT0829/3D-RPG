using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates
{
    GUARD,
    PARTOL,
    CHASE,
    DEAD
}

[RequireComponent(typeof(NavMeshAgent))]            //自動添加Component (開始運行時)
[RequireComponent(typeof(CharacterStats))]  
public class EnemyController : MonoBehaviour, IEndGameObserver
{
    [Header("Basic Setting")]
    public float sightRadius;
    public bool isGuard;
    private float speed;
    public float lookAtTime;
    private float remainLookAtTime;
    private float lastAttackTime;
    private Quaternion guardRotation;


    [Header("Patrol State")]
    public float partolRange;
    private Vector3 wayPoint;
    private Vector3 guardPos;

    //bool 配合動畫
    private bool isWalk;
    private bool isChase;
    private bool isFollow;
    private bool isDeath;
    private bool playerDead;


    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    private Animator anim;
    private Collider coll;
    public GameObject attackTarget;
    protected CharacterStats characterStats;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider>();
        characterStats = GetComponent<CharacterStats>();

        speed = agent.speed;
        guardPos = transform.position;
        guardRotation = transform.rotation;
        remainLookAtTime = lookAtTime;
    }

    private void Start()
    {
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PARTOL;
            GetNewWayPoint();
        }

        //FIXME: 切換場景時改掉
        GameManager.Instance.AddObserver(this);
    }

    // private void OnEnable()
    // {
    //     GameManager.Instance.AddObserver(this);
    // }

    private void OnDisable()
    {
        if(!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);
    }

    private void Update()
    {
        if (characterStats.CurrentHealth == 0)        
            isDeath = true;

        if(!playerDead){
            SwitchStates();
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime;
        }             
    }

    private void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDeath);
    }

    private void SwitchStates()
    {
        if (isDeath)
        {
            enemyStates = EnemyStates.DEAD;
        }
        else if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }


        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                isChase = false;

                if (transform.position != guardPos)
                {
                    isWalk = false;
                    agent.isStopped = false;
                    agent.destination = guardPos;

                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
                    }
                }
                break;
            case EnemyStates.PARTOL:
                isChase = false;
                agent.speed = speed * 0.5f;

                //判斷是否到了隨機巡邏點
                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                        remainLookAtTime -= Time.deltaTime;
                    else
                        GetNewWayPoint();
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }
                break;
            case EnemyStates.CHASE:
                //追Player
                //配合動畫
                isWalk = false;
                isChase = true;
                agent.speed = speed;
                if (!FoundPlayer())
                {
                    //拉脫回到上一個狀態
                    isFollow = false;
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }

                    else if (isGuard)
                    {
                        enemyStates = EnemyStates.GUARD;
                    }

                    else
                    {
                        isFollow = true;
                        enemyStates = EnemyStates.PARTOL;
                    }
                }
                else
                {
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                }

                //在攻擊範圍內則攻擊
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;

                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = characterStats.attackData.coolDown;

                        //爆擊判斷
                        characterStats.isCritical = Random.value < characterStats.attackData.critickalChance;
                        //執行攻擊
                        Attack();
                    }
                }
                break;
            case EnemyStates.DEAD:
                coll.enabled = false;
                // agent.enabled = false;
                agent.radius = 0;
                Destroy(gameObject, 2);
                break;
        }
    }

    private void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange() && !TargetInSkillRange())
        {
            //近身攻擊動畫
            anim.SetTrigger("Attack");
        }
        if (TargetInSkillRange())
        {
            //技能攻擊動畫
            anim.SetTrigger("Skill");
        }
    }

    private bool FoundPlayer()
    {
        //OverlapSphere 返回一個數組,其中包含羽球體內部的所有碰撞體
        var collider = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in collider)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        return false;
    }

    public bool TargetInAttackRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        else return false;

    }
    public bool TargetInSkillRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        else return false;
    }

    public void EndNotify()
    {
        //獲勝動畫
        //停止所有動畫
        anim.SetBool("Win", true);
        playerDead = true;
        isChase = false;
        isWalk = false;
        attackTarget = null;
    }

    private void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;

        float randomX = Random.Range(-partolRange, partolRange);
        float randomZ = Random.Range(-partolRange, partolRange);

        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);

        NavMeshHit hit;
        //NavMesh.SamplePosition 在指定範圍內找到導航網格上最近的點
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, partolRange, 1) ? hit.position : transform.position;
    }

    /// <summary>
    /// 在Unity畫出實線範圍
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }

    //Animation Event
    private void Hit()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }
}
