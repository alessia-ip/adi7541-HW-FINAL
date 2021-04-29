using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingThings : MonoBehaviour
{

    public string cType;
    public float health;
    public float bAttack;

    public LivingThings(string cType, float health, float bAttack)
    {
        this.cType = cType;
        this.health = health;
        this.bAttack = bAttack;
    }

    public virtual float attackDamage(float attack, float modifier)
    {
        return attack * modifier;
    }

}
