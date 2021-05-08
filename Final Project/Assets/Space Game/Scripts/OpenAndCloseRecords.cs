using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OpenAndCloseRecords : MonoBehaviour
{
   public GameObject records;
   public TextMeshProUGUI recordsText;
   public GameManager _GameManager;
   
   private void Start()
   {
      recordsText.text = "";
   }

   public void OpenRecords()
   {
      records.SetActive(true);
      _GameManager.logOpen = true;
   }

   public void CloseRecords()
   {
      records.SetActive(false);
      _GameManager.logOpen = false;

   }
}
