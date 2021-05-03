using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private LivingPlayer playerStats;
    public int startingHealth;
    public int startingDefense;
    public int startingAttackDMG;
    public int playerBullets;


    void Start()
    {
        playerStats = new LivingPlayer(
            startingHealth,
            startingAttackDMG,
            startingDefense,
            KeyCode.A,
            KeyCode.D,
            KeyCode.S
        );
    }
    
    void Update()
    {
        //movement
        if (Input.GetKey(playerStats.left))
        {
            
        } else if (Input.GetKey(playerStats.right))
        {
            
        }

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        var obj = other.gameObject;
        if (obj.tag == "enemyBullet")
        {  
            var damageTaken = playerStats.takeDamage(
                playerStats.bDefense,
                obj.GetComponent<LivingBoss>().bAttack);
            playerStats.health = playerStats.health - damageTaken; 
        }
    }
}
