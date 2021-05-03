using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingBoss : LivingThings
{
    public LivingBoss(string cType, float health, float bAttack, int bullets) : base(health, bAttack, bullets)
    {
        cType = "Boss";
    }
}
