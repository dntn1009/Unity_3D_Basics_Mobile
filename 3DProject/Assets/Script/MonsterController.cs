using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{

    public enum BehaviourState
    {
        Idle,
        Chase,
        Patrol,
        Attack,
        Damaged,
        Die,
        Max
    }
    BehaviourState m_state;
    [Header("몬스터 능력치")]
    [SerializeField]
    Status m_status;
    [SerializeField]
    WayPointSystem m_waypointSystem;
    MonsterAnimController m_animCtr;
    PlayerController m_player;
    NavMeshAgent m_navAgent;
    TweenMove m_tweenMove;
    [SerializeField]
    HUDController m_hudCtr;
    [SerializeField]
    Collider m_collider;
    float m_idleDuration = 5f;
    float m_dieDuration = 4f;
    float m_dieTime;
    float m_idleTime;
    float m_attackDist = 1.6f; // 공격이 가능한 거리
    float m_detectDist = 4f;
    float m_sqrAttackDist;
    float m_sqrDetecDist;
    bool m_isMove; //  waypoint 
    int m_curWaypoint; // waypoint
    int m_delayFrame; // Hit Animation
    Coroutine m_coroutineDelayMotion; // Hit Animation;
    public Status MyStatus { get { return m_status; } }
    public bool IsDie {  get { return m_state == BehaviourState.Die; } }

    #region Animation Event Methods
    void AnimEvent_AttackFinished()
    {
        SetIdle(1f);
    }
    IEnumerator Coroutine_DelayMotion(int frame)
    {
        for (int i = 0; i < frame; i++)
            yield return null;
        SetIdle(0f);
        m_delayFrame = 0;
        m_navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
    }
    void AnimEvent_HitFinished()
    {
       m_coroutineDelayMotion = StartCoroutine(Coroutine_DelayMotion(m_delayFrame));
    }
    #endregion

    IEnumerator Coroutine_SetDestination(int frame, Transform target)
    {
        while (m_state == BehaviourState.Chase)
        {
            for(int i = 0; i< frame; i++)
            {
                yield return null;
            }
            m_navAgent.SetDestination(target.position);
        }

    }

    #region Public Methods and Operators

    public void SetDemage(AttackType attackType,float damage, SkillData skillData)
    {
        if (IsDie) return;

        m_hudCtr.ActiveUI();
        m_status.hp -= Mathf.CeilToInt(damage);
        m_hudCtr.DisplayDamage(attackType, damage, m_status.hp / (float)m_status.hpMax);
        
        if (attackType == AttackType.Dodge) return;
        
        m_navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        if (m_coroutineDelayMotion != null)
        {
            StopCoroutine(m_coroutineDelayMotion);
            m_coroutineDelayMotion = null;
        }
        m_animCtr.Play(MonsterAnimController.Motion.Hit, false);
        SetState(BehaviourState.Damaged);
        m_delayFrame = skillData.delayFrame;
        if(skillData.knockBack > 0f)
        {
            var duration = SkillData.MaxKnockBackDuration * (skillData.knockBack / SkillData.MaxKnockBackDist);
            m_tweenMove.Play(transform.position, transform.position + (transform.position - m_player.transform.position).normalized * skillData.knockBack,duration);
        }
        if(m_status.hp <= 0f)
        {
            SetState(BehaviourState.Die);
            m_animCtr.Play(MonsterAnimController.Motion.Die);
        }
    }

    public void SetMonster(Camera uiCamera, Transform hudPool)
    {
        m_hudCtr.SetHud(uiCamera, hudPool);
    }

    public void InitMonster(WayPointSystem path)
    {
        m_waypointSystem = path;
        m_curWaypoint = -1;
        transform.position = path.m_waypoints[0].transform.position;
        gameObject.SetActive(true);
        SetIdle(1f);
        m_status.hp = m_status.hpMax;
        m_collider.enabled = true;
        m_hudCtr.InitHud();
    }

    #endregion
    void SetState(BehaviourState state)
    {
        m_state = state;
    }

    void SetIdleDuration(float duration)
    {
        m_idleTime = m_idleDuration - duration;
        if (m_idleTime < 0f) m_idleTime = 0f;
    }

    bool IsInSetArea(float curDist, float targetdist)
    {
        if (Mathf.Approximately(curDist, targetdist) || curDist < targetdist)
        {
            return true;
        }
        return false;
    }
    bool FindTarget(Transform target, float distance)
    {
        //int collect = 0;
        var dir = target.position - transform.position;
        dir.y = 0f;
        RaycastHit hit;
        Debug.DrawRay(transform.position + Vector3.up * 1f, dir.normalized * distance, Color.red);
        if(Physics.Raycast(transform.position + Vector3.up * 1f, dir.normalized, out hit, distance, 1<< LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Player")))
        {
            if (hit.collider.CompareTag("Player"))
                return true;
            //collect++;
        }
        //var dot = Vector3.Dot(transform.forward, dir.normalized);
        //if(dot > 0)
            //collect++;
        return false;
    }
    
    void SetIdle(float duration)
    {
        m_isMove = false;
        m_navAgent.ResetPath();
        m_navAgent.isStopped = false;
        SetState(BehaviourState.Idle);
        m_animCtr.Play(MonsterAnimController.Motion.Idle);
        SetIdleDuration(duration);
    }
    void BehaviourProcess()
    {
        float dist = 0f;
        switch(m_state)
        {
            case BehaviourState.Idle:
                m_idleTime += Time.deltaTime;
                if(m_idleTime > m_idleDuration)
                {
                    m_idleTime = 0f;
                    m_navAgent.isStopped = false;

                    if (FindTarget(m_player.transform, m_attackDist))
                    {
                        SetState(BehaviourState.Attack);
                        m_animCtr.Play(MonsterAnimController.Motion.Attack1);
                        return;
                    }
                    if (FindTarget(m_player.transform, m_detectDist))
                    {
                        SetState(BehaviourState.Chase);
                        m_animCtr.Play(MonsterAnimController.Motion.Run);
                        StartCoroutine(Coroutine_SetDestination(15, m_player.transform));
                        m_navAgent.stoppingDistance = m_attackDist;
                        return;
                    }
                    SetState(BehaviourState.Patrol);
                    m_animCtr.Play(MonsterAnimController.Motion.Run);
                    m_navAgent.stoppingDistance = m_navAgent.radius;
                }
                break;
            case BehaviourState.Attack:
                break;
            case BehaviourState.Chase:
                // m_navAgent.SetDestination(m_player.transform.position);
                 dist = (m_player.transform.position - transform.position).sqrMagnitude;
                if(IsInSetArea(dist, m_sqrAttackDist))
                {
                    m_navAgent.ResetPath();
                    m_navAgent.isStopped = true;
                    SetIdle(0f);
                }
                break;
            case BehaviourState.Patrol:
                if (!FindTarget(m_player.transform, m_detectDist))
                {
                    if (!m_isMove)
                    {
                        m_curWaypoint++;
                        if (m_curWaypoint > m_waypointSystem.m_waypoints.Length - 1)
                            m_curWaypoint = 0;
                        m_navAgent.SetDestination(m_waypointSystem.m_waypoints[m_curWaypoint].transform.position);
                        m_isMove = true;
                    }
                    else
                    {
                        dist = (m_waypointSystem.m_waypoints[m_curWaypoint].transform.position - transform.position).sqrMagnitude;
                        if (IsInSetArea(dist, Mathf.Pow(m_navAgent.stoppingDistance, 2f)))
                        {
                            SetIdle(2f);
                        }
                    }
                }
                else
                {
                    m_navAgent.ResetPath();
                    m_navAgent.isStopped = true;
                    SetIdle(0f);
                }
                break;
            case BehaviourState.Die:
                m_dieTime += Time.deltaTime;
                if(m_dieTime >= m_dieDuration)
                {
                    MonsterManager.Instance.RemoveMonster(this);
                    m_dieTime = 0f;
                }
                break;
        }
    }

    void InitStatus()
    {
        m_status = new Status(500, 50f, 5f, 5f, 80f, 25f, 10f);
    }

    // Start is called before the first frame update
    void Awake()
    {
        m_curWaypoint = -1;
        m_sqrAttackDist = Mathf.Pow(m_attackDist, 2f);
        m_sqrDetecDist = Mathf.Pow(m_detectDist, 2f);
        m_animCtr = GetComponent<MonsterAnimController>();
        m_navAgent = GetComponent<NavMeshAgent>();
        m_tweenMove = GetComponent<TweenMove>();
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        InitStatus();
    }

    // Update is called once per frame
    void Update()
    {
        BehaviourProcess();
    }
}
