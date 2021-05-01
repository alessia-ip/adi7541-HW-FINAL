using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingPlayer : LivingThings
{

    public KeyCode left;
    public KeyCode right;
    public KeyCode shoot;
    
    public LivingPlayer(float health, float bAttack, int bullets, KeyCode L, KeyCode R, KeyCode S) : base( health, bAttack, bullets)
    {
        cType = "Player";
        left = L;
        right = R;
        shoot = S;
    }
}
