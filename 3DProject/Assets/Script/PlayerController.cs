using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [Header("주인공 능력치")]
    [SerializeField]
    Status m_status;
    Vector3 m_dir;
    [SerializeField]
    float m_speed = 2f;
    PlayerAnimController m_animCtr;
    CharacterController m_charCtr;
    Queue<KeyCode> m_keyBuffer = new Queue<KeyCode>();
    AttackAreaUnitFind[] m_attackArea;
    [SerializeField]
    GameObject m_attackAreaObj;
    GameObject m_fxHitPrefab;
    LineRenderer m_lineRenderer;
    NavMeshAgent m_navAgent;
    NavMeshPath m_path;
    [SerializeField]
    HUDText m_hudText;
    float m_accel;
    Dictionary<PlayerAnimController.Motion, SkillData> m_skillTable = new Dictionary<PlayerAnimController.Motion, SkillData>();

    Vector3 m_touchDir;
    Vector3 m_targetPos;

    public Status MyStatus { get { return m_status; } }

    public bool IsAttack { 
        get { if (m_animCtr.GetAnimState() == PlayerAnimController.Motion.Attack1 ||
                m_animCtr.GetAnimState() == PlayerAnimController.Motion.Attack2 ||
                m_animCtr.GetAnimState() == PlayerAnimController.Motion.Attack3 ||
                m_animCtr.GetAnimState() == PlayerAnimController.Motion.Attack4)
                return true;
            return false;
        } }//공격할떄 움직이지 말라는 명령어를 내리기 위해서
    bool m_isPressAttack;
    List<PlayerAnimController.Motion> m_comboList = new List<PlayerAnimController.Motion>() { PlayerAnimController.Motion.Attack1, PlayerAnimController.Motion.Attack2, PlayerAnimController.Motion.Attack3, PlayerAnimController.Motion.Attack4 };

    int m_comboIndex; // 몇번쨰 ATTACk 인지 알려주려는 변수

    #region Animation Event Methods
    void AnimEvent_Attack()
    {
        SkillData skillData;
        float damage = 0f;
        if (m_skillTable.TryGetValue(m_animCtr.GetAnimState(), out skillData))
        {
            var unitList = m_attackArea[skillData.AttackArea].UnitList;
            for (int i = 0; i < unitList.Count; i++)
            {
                var mon = unitList[i].GetComponent<MonsterController>();
                var dummy = Util.FindChildObject(unitList[i], "Dummy_Hit");
                if (dummy != null && mon != null)
                {
                    if (mon.IsDie) continue;

                    AttackType type = AttackProcess(mon, skillData, out damage);
                    mon.SetDemage(type ,damage, skillData);
                    if (type == AttackType.Dodge) return;
                    var effect = Instantiate(m_fxHitPrefab);
                    effect.transform.position = dummy.transform.position;
                    effect.transform.rotation = Quaternion.FromToRotation(effect.transform.forward, (unitList[i].transform.position - transform.position).normalized);
                }
            }
        }
        for (int i = 0; i < m_attackArea.Length; i++)
            m_attackArea[i].UnitList.RemoveAll(obj => obj.GetComponent<MonsterController>().IsDie);
    }
    void AnimaEvent_AttackFinished()
    {
        bool IsCombo = false; // 연타했을경우를 위한 변수
        if (m_isPressAttack)
            IsCombo = true;
        if(m_keyBuffer.Count == 1)
        {
            var key = m_keyBuffer.Dequeue();
            if (key == KeyCode.Space)
                IsCombo = true;
        }
        else if(m_keyBuffer.Count > 1)
        {
            ReleaseKeyBuffer();
            IsCombo = false;
        }
        if(IsCombo)
        {
            m_comboIndex++;
            if (m_comboIndex >= m_comboList.Count)
                m_comboIndex = 0;
            m_animCtr.Play(m_comboList[m_comboIndex]);
        }
        else
        {
            m_animCtr.Play(PlayerAnimController.Motion.Idle);
            m_comboIndex = 0;
        }
    }
    #endregion
    //애니메이션 5프레임전에 이벤트 만들어서 어택 1234 자동으로 하게 만들려는 함수들

    AttackType AttackProcess(MonsterController mon, SkillData skillData, out float damage)
    {
        AttackType type = AttackType.Dodge;
        damage = 0f;
        if(CalculationDamage.AttackDecision(MyStatus.hitRate, mon.MyStatus.dodgeRate))
        {
            type = AttackType.Normal;
            damage = CalculationDamage.NormalDamage(MyStatus.attack, skillData.attack, mon.MyStatus.defence);
            if(CalculationDamage.CriticalDecision(MyStatus.criRate))
            {
                type = AttackType.Critical;
                damage = CalculationDamage.CriticalDamage(damage, MyStatus.criAttack);
            }
        }
        return type;
    }

    public void OnPressAttack()
    {
        if (IsAttack)
        {
            if (IsInvoking("ReleaseKeyBuffer")) // 아직 리셋이 안되었단 이야기
                CancelInvoke("ReleaseKeyBuffer");
            float time = m_animCtr.GetComboInputTime(m_comboList[m_comboIndex].ToString());
            Invoke("ReleaseKeyBuffer", time); ;
            m_keyBuffer.Enqueue(KeyCode.Space);
        }// 눌린시점으로부터 일정시간동안 값이 안들어오면 이값을 비울것이란 예약
        if (m_animCtr.GetAnimState() == PlayerAnimController.Motion.Idle || m_animCtr.GetAnimState() == PlayerAnimController.Motion.Run)
        {
            m_animCtr.Play(PlayerAnimController.Motion.Attack1);
        }
        m_isPressAttack = true;
    }

    public void OnReleaseAttack()
    {
        m_isPressAttack = false;
    }

    void ReleaseKeyBuffer()
    {
        m_keyBuffer.Clear();
    }

    // System.DateTime? time;// DateTime 은 연월일분초를 알려줌 ?를 넣으면 null 값가능 레퍼런스로 사용가능
    Vector3? GettouchPoint()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000f, 1 << LayerMask.NameToLayer("Ground")))
        {
            return hit.point;
        }
        else
            return null;
    }

    IEnumerator Coroutine_MoveToTarget()
    {
        int curPos = 0;
        m_touchDir = (m_path.corners[curPos] - transform.position);
        while (true)
        {
            if (Vector3.Distance(transform.position, m_path.corners[curPos]) <= 0.25f)
            {
                curPos++;
                if (curPos >= m_path.corners.Length)
                {
                    m_dir = m_touchDir = m_targetPos = Vector3.zero;
                    yield break;
                }
                m_touchDir = (m_path.corners[curPos] - transform.position);
            }
            yield return null;
        }
    }

    Vector3 GetPadDir()
    {
        var Paddir = GamePad.Instance.GetAxis();
        Vector3 dir = Vector3.zero;
        if(Paddir.x < 0.0f)
        {
            dir += Vector3.left * Mathf.Abs(Paddir.x);
        }
        if(Paddir.x > 0.0f)
        {
            dir += Vector3.right * Paddir.x;
        }
        if(Paddir.y < 0.0f)
        {
            dir += Vector3.back * Mathf.Abs(Paddir.y);
        }
        if(Paddir.y > 0.0f)
        {
            dir += Vector3.forward * Paddir.y;
        }
        return dir;
    }

    void InitSkillData()
    {
        m_skillTable.Add(PlayerAnimController.Motion.Attack1, new SkillData() { AttackArea = 0, knockBack = 0.5f, delayFrame = 0, attack = 0f });
        m_skillTable.Add(PlayerAnimController.Motion.Attack2, new SkillData() { AttackArea = 1, knockBack = 0.8f, delayFrame = 5, attack = 10f});
        m_skillTable.Add(PlayerAnimController.Motion.Attack3, new SkillData() { AttackArea = 2, knockBack = 1.0f, delayFrame = 15, attack = 20f});
        m_skillTable.Add(PlayerAnimController.Motion.Attack4, new SkillData() { AttackArea = 3, knockBack = 1.5f, delayFrame = 30, attack = 30f});
        //DB로 만들자
    }

    void InitStatus()
    {
        m_status = new Status(1000, 60f, 10f, 15f, 100f, 50f, 25f);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_animCtr = GetComponent<PlayerAnimController>();
        m_charCtr = GetComponent<CharacterController>();
        m_navAgent = GetComponent<NavMeshAgent>();
        m_lineRenderer = GetComponent<LineRenderer>();
        m_attackArea = m_attackAreaObj.GetComponentsInChildren<AttackAreaUnitFind>();
        m_fxHitPrefab = Resources.Load("Prefab/Effect/FX_Attack01_01") as GameObject;
        m_path = new NavMeshPath();
        m_navAgent.updateRotation = false; // 회전이 불편하면 펄즈 값 주면 안함
        InitSkillData();
        InitStatus();
    }


    // Update is called once per frame
    void Update()
    {
        var padDir = GetPadDir();
        m_dir = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_hudText.Add(Random.Range(5, 200), Color.red, 0f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_hudText.Add(Random.Range(5, 200), Color.green, 2f);
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (GamePad.Instance.IsPressed) return;
           var point = GettouchPoint();
            if(point != null)
            {
                m_touchDir = (point.Value - transform.position);
                m_touchDir.y = 0f;
                m_targetPos = point.Value;
                //m_navAgent.SetDestination(m_targetPos);//길찾기 시도 에이전트로 말고
                NavMesh.CalculatePath(transform.position, m_targetPos, NavMesh.AllAreas, m_path); // 이걸로도 가능
                StartCoroutine("Coroutine_MoveToTarget");
               
            }
            else
            {
                m_touchDir = Vector3.zero;
                m_targetPos = Vector3.zero;
            }
        }
        m_dir = padDir;
        if (m_touchDir != Vector3.zero)
        {
            m_dir = m_touchDir.normalized;
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            OnPressAttack();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnReleaseAttack();
        }
        if (m_dir != Vector3.zero && !IsAttack)
        {
            // m_animator.SetBool("IsMove", true);
            if (m_animCtr.GetAnimState() == PlayerAnimController.Motion.Idle)
                m_animCtr.Play(PlayerAnimController.Motion.Run);
            if(m_charCtr.enabled)
            transform.forward = m_dir; //캐릭터의 정면을 뜻함
        }
        else
        {
            if (m_animCtr.GetAnimState() == PlayerAnimController.Motion.Run)
                m_animCtr.Play(PlayerAnimController.Motion.Idle);
           /* m_animator.SetBool("IsMove", false);*/
        }

        if (!m_charCtr.isGrounded)
        {
            m_accel += Mathf.Abs(Physics.gravity.y) * Time.deltaTime;
            m_dir += Vector3.down * m_accel;

        }
        else
        {
            m_accel = 0f;
        }
     /*   if(m_touchDir != Vector3.zero)
        {
            var dist = Vector3.Distance(transform.position, m_targetPos);
            if (Mathf.Approximately(dist, 0.25f) || dist < 0.25f)
            {
                m_targetPos = m_touchDir = m_dir = Vector3.zero;
            }
        }*/
      /*  if(m_navAgent.enabled && m_navAgent.desiredVelocity.sqrMagnitude >= Mathf.Pow(0.1f, 2f))
        {
            var dir = m_navAgent.desiredVelocity;
            var angle = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, angle, 8.0f * Time.deltaTime);
        }*/
        if (!IsAttack && m_charCtr.enabled)//패드 말고 마우스클릭으로만 움직이려고해서 캐릭터컨트롤러꺼놈
            m_charCtr.Move(m_dir * m_speed * Time.deltaTime);

        // var moveVal = m_dir * m_speed;
        //m_charCtr.SimpleMove(moveVal);
        //transform.position += m_dir * m_speed * Time.deltaTime;
    }
    private void LateUpdate()// 업데이트 끝난후 마지막에 돌림
    {
        if (m_path != null && m_path.corners != null)
        {
            m_lineRenderer.positionCount = m_path.corners.Length;
            m_lineRenderer.SetPositions(m_path.corners);
        }
        /*if(m_navAgent.hasPath)
         {
             m_lineRenderer.positionCount = m_navAgent.path.corners.Length;
             m_lineRenderer.SetPositions(m_navAgent.path.corners);
         }*/
    }
}
