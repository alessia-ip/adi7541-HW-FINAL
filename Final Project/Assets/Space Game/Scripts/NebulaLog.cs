using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NebulaLog : MonoBehaviour
{
   public LogScriptableObjects logDate;
   public bool alreadyRead = false;

   //this is assigned to every nebula
   //each one keeps track of its own assigned scriptable object
   //and whether the player has visited or not
   
   private void Update()
   {
      if (alreadyRead == true)
      {
         this.gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;
         //when the player visits, we have a visual cue to keep track of it
      }
   }
}
