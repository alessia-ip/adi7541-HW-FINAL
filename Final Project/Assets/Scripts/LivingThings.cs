using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingThings
{

    public string cType;
    public float health;
    public float bAttack;
    public float bDefense;
    public int bulletNum;
    
    public LivingThings(float health, float bAttack, int bullets)
    {
        this.health = health;
        this.bAttack = bAttack;
        this.bulletNum = bullets; 
    }

    public virtual float attackDamage(float attack, float modifier)
    {
        return attack * modifier;
    }    
    
 
    public virtual float takeDamage(float defense, float damage)
    {
        return damage / defense;
    }

}
