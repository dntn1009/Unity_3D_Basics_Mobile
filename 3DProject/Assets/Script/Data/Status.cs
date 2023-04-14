using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Status
{
    public int hp;
    public int hpMax;
    public float hitRate;
    public float dodgeRate;
    public float criRate;
    public float criAttack;
    public float attack;
    public float defence;

    public Status(int hp, float hitRate, float dodgeRate, float criRate, float criAttack, float attack, float defence)
    {
        this.hp = hpMax = hp;
        this.hitRate = hitRate;
        this.dodgeRate = dodgeRate;
        this.criRate = criRate;
        this.criAttack = criAttack;
        this.attack = attack;
        this.defence = defence;
    }
}
