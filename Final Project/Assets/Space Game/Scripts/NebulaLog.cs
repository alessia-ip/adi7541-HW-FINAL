using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NebulaLog : MonoBehaviour
{
   public LogScriptableObjects logDate;
   public bool alreadyRead = false;

   private void Update()
   {
      if (alreadyRead == true)
      {
         this.gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;
      }
   }
}
