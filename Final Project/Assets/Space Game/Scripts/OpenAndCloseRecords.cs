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
      recordsText.text = ""; // clear the dummy text out of the canvas at the start of the game
   }

   
   
   public void OpenRecords()
   {
      scroll.transform.position = new Vector2(scroll.transform.position.x, 1); //we want the most recent entry to be on display when the record is open
      //so we change the rect position
      //the rect is masked, so this shows the bottom entry
      //the player is able to scroll after
      
      records.SetActive(true); //turn on the canv
      _GameManager.logOpen = true; //set this so the player can't use the arrow keys/space
      audSFX.PlayOneShot(OpenClose);
   }

   public void CloseRecords()
   {
      //turns off UI and re-enables player motion
      records.SetActive(false);
      _GameManager.logOpen = false;
      audSFX.PlayOneShot(OpenClose);

   }
}
