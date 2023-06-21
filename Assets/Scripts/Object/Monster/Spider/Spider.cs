using SpiderState;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spider : Monster, IHitable
{
    public enum State { Idle, Trace, Return, BiteAttack, ProjecTileAttack, TakeDamage, Die, CastAttack, Size }

    [NonSerialized] public Vector3 spawnPos;
    [NonSerialized] public Animator animator;
    [NonSerialized] public SpiderSpawn spawnInfo;

    [NonSerialized] public float ProjecttileTime;

    List<MonsterBaseState<Spider>> states;
    public Coroutine projectTileAttackRoutine;
    public Coroutine projectTileMoveRoutine;
    public Coroutine biteRoutine;
    public Coroutine takedamageRoutine;
    State currState;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        states = new List<MonsterBaseState<Spider>>((int)State.Size)
        {
            new SpiderState.IdleState(this),
            new SpiderState.TraceState(this),
            new SpiderState.ReturnState(this),
            new SpiderState.BiteAttackState(this),
            new SpiderState.ProjectTileAttackState(this),
            new SpiderState.TakeDamageState(this),
            new SpiderState.DieState(this),
        };
    }

    private void OnEnable()
    {
        currHp = data.MaxHp;
        spawnPos = transform.position;
    }

    private void Start()
    {
        currState = State.Idle;
        states[(int)currState]?.Enter();
    }

    private void Update()
    {
        states[(int)currState]?.Update();
    }

    public void ChangeState(State state)
    {
        if (currState == State.Die)
            return;

        states[(int)currState]?.Exit();
        currState = state;
        states[(int)currState]?.Enter();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "SpiderArea")
        {
            StopCoroutine(projectTileAttackRoutine);
            StopCoroutine(biteRoutine);
            ChangeState(State.Return);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.AgressiveMonsterData[0].DetectRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.MeleeMonsterData[0].DetectRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, data.RangeMonsterData[0].DetectRange);
    }

    public void Hit(int damage)
    {
        if (currState == State.Die)
            return;

        if (biteRoutine != null && projectTileAttackRoutine != null)
        {
            StopCoroutine(projectTileAttackRoutine);
            StopCoroutine(biteRoutine);
        }    

        animator.SetBool("Move", false);
        animator.SetBool("Attack", false);

        if (currState == State.TakeDamage)
            StartCoroutine(coolTimer());
        else
            ChangeState(State.TakeDamage);

        currHp -= damage;

        if (currHp <= 0)
        {
            if (biteRoutine != null && projectTileAttackRoutine != null && takedamageRoutine != null && projectTileMoveRoutine != null)
            {
                StopCoroutine(projectTileAttackRoutine);
                StopCoroutine(biteRoutine);
                StopCoroutine(takedamageRoutine);
                StopCoroutine(projectTileMoveRoutine);
            }

            animator.SetBool("Move", false);
            animator.SetBool("Attack", false);
            animator.SetBool("TakeDamage", false);
            ChangeState(State.Die);
        }
    }

    IEnumerator coolTimer()
    {
        states[(int)currState]?.Exit();
        yield return new WaitForSeconds(0.1f);
        states[(int)currState]?.Enter();
    }
}