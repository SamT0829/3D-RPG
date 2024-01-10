using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private GameObject attackTarget;
    private CharacterStats characterStats;
    private float lastAttackTime;

    private bool isDeath;

    private float stopDistance;  

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();

        stopDistance = agent.stoppingDistance;
    }

    private void OnEnable()
    {
        MouseManager.Instance.OnMouseCliceked += MoveToTarget;
        MouseManager.Instance.OnEnemyCliceked += EventAttack;
        GameManager.Instance.RigisterPlayer(characterStats);

    }

    private void Start()
    {
        SaveManager.Instance.LoadPlayerData();       
    }

    private void OnDisable()
    {
        MouseManager.Instance.OnMouseCliceked -= MoveToTarget;
        MouseManager.Instance.OnEnemyCliceked -= EventAttack;
    }

    private void Update()
    {
        isDeath = characterStats.CurrentHealth == 0;
        if (isDeath)
        {
            GameManager.Instance.NotifyObservers();
        }

        SwitchAnimation();

        lastAttackTime -= Time.deltaTime;
    }

    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
        anim.SetBool("Death", isDeath);
    }

    private void EventAttack(GameObject target)
    {
        if (isDeath) return;

        if (target != null)
        {
            attackTarget = target;
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.critickalChance;
            StartCoroutine(MoveToAttackTarget());
        }
    }

    private void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();
        if (isDeath) return;

        agent.isStopped = false;
        agent.destination = target;
        agent.stoppingDistance = stopDistance;
    }

    private IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
        agent.stoppingDistance = characterStats.attackData.attackRange;
        transform.LookAt(attackTarget.transform);

        //TODO: 修改攻擊範圍參數
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }
        agent.isStopped = true;

        //Atack
        if (lastAttackTime < 0)
        {
            anim.SetBool("Critical", characterStats.isCritical);
            anim.SetTrigger("Attack");
            //重置冷卻時間
            lastAttackTime = characterStats.attackData.coolDown;
        }
    }

    //Animation Event
    private void Hit()
    {
        if (attackTarget.CompareTag("Attackable"))
        {
            if (attackTarget.GetComponent<Rock>())
            {
                attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
            }
        }
        else
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();

            targetStats.TakeDamage(characterStats, targetStats);
        }
    }

   
}
