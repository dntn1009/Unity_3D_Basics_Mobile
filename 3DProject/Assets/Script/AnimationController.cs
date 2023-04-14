using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    //이거 다형성임 부모 자식간
    Animator m_animator;
    Dictionary<string, float> m_dicComboInputTime = new Dictionary<string, float>();
    string m_prevMotion;
    public void CalculateCombonputTime()
    {
       
        var cilps = m_animator.runtimeAnimatorController.animationClips; //애니메이션안에 등록되어있는 클립들 정보
        for (int i = 0; i < cilps.Length; i++)
        {
            if (cilps[i].events.Length >= 2)
            {
                float attackTime = cilps[i].events[0].time;
                float endFrameTime = cilps[i].events[1].time;
                float result = (endFrameTime - attackTime);
                m_dicComboInputTime.Add(cilps[i].name, result);
            }
        }
    }
    public float GetComboInputTime(string animName)
    {
        float time = 0f;
        m_dicComboInputTime.TryGetValue(animName, out time);
        return time;
    }
    public void Play(string animName, bool isBlend = true)
    {
        if(!string.IsNullOrEmpty(m_prevMotion))
        {
            m_animator.ResetTrigger(m_prevMotion);
            m_prevMotion = null;
        }
        if(isBlend)
        {
            m_animator.SetTrigger(animName);
        }
        else
        {
            m_animator.Play(animName, 0, 0f);
        }
        m_prevMotion = animName;
    }

    // Start is called before the first frame update
    void Awake()
    {
        m_animator = GetComponent<Animator>();
        CalculateCombonputTime();
    }

}
