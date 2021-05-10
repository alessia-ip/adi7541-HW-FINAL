using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreen : MonoBehaviour
{
     public GameManager _gameManager;
     public Camera cam;
     public GameObject startCanv;
     public LogManager firstLog;
     
     private bool start = false;

     public LogScriptableObjects firstLogSO;

     public GameObject recordsCanvas;

     public LogNumberTracker logStart;
     
     public void startthegame() //this is assigned to a button
     {
         start = true; //we set start to true here so the camera can smoothly move
         cam.gameObject.GetComponent<AudioSource>().enabled = true;
     }

 private void Update()
 {
     if (cam.orthographicSize != 25 && start == true) //the camera size gets smaller, and zooms in, when the game starts
     {
         cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, 25, 0.5f);
     }

     if (cam.orthographicSize == 75) //we set the OG canvas to false here to hide the intro UI
     {
         startCanv.SetActive(false);
     }
     
     if (cam.orthographicSize == 26) //one increment before the last size, we open the introductory log of the game
     {
         _gameManager.playAudioClip(_gameManager.openLog);
         logStart.currentLogNumber = 521; //we reset the scriptable object's int here to the original num
         firstLog.openLog(firstLogSO);
         recordsCanvas.SetActive(true);
         _gameManager.GameStart = true; //the game is officially set to start, and the player is granted control
     }
 }
}
