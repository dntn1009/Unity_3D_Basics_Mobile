using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    Normal,
    Critical,
    Dodge,
    Max
}
public class CalculationDamage
{
    public static bool AttackDecision(float AttackerHit, float defenceDodge)
    {
        if (Mathf.Approximately(AttackerHit, 100.0f) || AttackerHit > 100f)
            return true;
        float total = AttackerHit + defenceDodge;// 공격자의 히트와 상대방의 방어율을 더한게 토탈임
        float hitRate = Random.Range(0.0f, total);
        if(hitRate <= AttackerHit)
        {
            return true;
        }
        return false;
    }
    public static float NormalDamage(float attackAtk, float skillAtk, float defenceDef)
    {
        float attack = attackAtk + (attackAtk * skillAtk / 100.0f);
        return attack - defenceDef;
    }

    public static bool CriticalDecision(float criRate)
    {
        var result = Random.Range(0.0f, 100.0f);
        if(result<=criRate)
            return true;
        return false;
    }

    public static float CriticalDamage(float damage, float criAtk)
    {
        return damage + (damage * criAtk / 100.0f);
    }

}

