using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MonsterAnimController : AnimationController
{
    public enum Motion
    {
        None = -1,
        Idle,
        Run,
        Hit,
        Attack1,
        Die,
        Max
    }

    StringBuilder m_sb = new StringBuilder();
    Motion m_state;
    public Motion GetAnimState()
    {
        return m_state;
    }// 1. 이걸로 Motion값을 얻은후에
    public void Play(Motion motion, bool isBlend = true)
    {
        m_sb.Append(motion);
        m_state = motion;
        Play(m_sb.ToString(), isBlend);
        m_sb.Clear();
    } //2. 모션을 얻은걸 여기에 넣는다.

}
