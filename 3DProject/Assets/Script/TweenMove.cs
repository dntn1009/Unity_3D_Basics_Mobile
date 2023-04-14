using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TweenMove : MonoBehaviour
{
    public Vector3 m_from;
    public Vector3 m_to;
    public float m_duration = 1f;
    [SerializeField]
    AnimationCurve m_curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    NavMeshAgent m_navAgent;
    bool m_isStart;
    float m_time;

    public void Play()
    {
        m_time = 0f;
        m_navAgent.ResetPath();
        m_navAgent.isStopped = true;
        StopAllCoroutines();
        StartCoroutine("Coroutine_PlayTween");
    }

    public void Play(Vector3 from, Vector3 to, float duration)
    {
        m_from = from;
        m_to = to;
        m_duration = duration;
        Play();
    }
    IEnumerator Coroutine_PlayTween()
    {
        while(m_time < 1.0f)
        {
            m_time += Time.deltaTime / m_duration;
            var value = m_curve.Evaluate(m_time);
            var result = m_from * (1f - value) + m_to * value;
            m_navAgent.Move(result - transform.position);
            m_time += Time.deltaTime / m_duration;
            yield return null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_navAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
