using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenAndCloseRecords : MonoBehaviour
{
   public GameObject records;
   public TextMeshProUGUI recordsText;
   public GameManager _GameManager;

   public AudioSource audSFX;
   public AudioClip OpenClose;

   public RectTransform scroll;
   
   private void Start()
   {
      recordsText.text = "";
   }

   
   
   public void OpenRecords()
   {
      scroll.transform.position = new Vector2(scroll.transform.position.x, 1);
      records.SetActive(true);
      _GameManager.logOpen = true;
      audSFX.PlayOneShot(OpenClose);
   }

   public void CloseRecords()
   {
      records.SetActive(false);
      _GameManager.logOpen = false;
      audSFX.PlayOneShot(OpenClose);


   }
}
