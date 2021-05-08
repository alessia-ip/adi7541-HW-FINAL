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
     
     public void startthegame()
     {
         start = true;
     }

 private void Update()
 {
     if (cam.orthographicSize != 25 && start == true)
     {
         cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, 25, 0.5f);
     }

     if (cam.orthographicSize == 75)
     {
         startCanv.SetActive(false);
     }
     
     if (cam.orthographicSize == 26)
     {
         logStart.currentLogNumber = 521;
         firstLog.openLog(firstLogSO);
         recordsCanvas.SetActive(true);
         _gameManager.GameStart = true;
     }
 }
}
