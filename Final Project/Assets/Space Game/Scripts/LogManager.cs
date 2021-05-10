using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{

    public LogScriptableObjects currentLog;
    public GameObject logCanv;
    public TextMeshProUGUI logTitle;
    public TextMeshProUGUI logBody;

    public GameManager _GameManager;

    public TextMeshProUGUI recordtextupdate;
    
    public void closeLog() //this is for when the player manually closes the log
    {
        _GameManager.logOpen = false; //the game manager keeps this bool to allow /disallow player movement
        logCanv.SetActive(false); //also we want the canvas to stop being visible

    }

    public void openLog(LogScriptableObjects getLog) //we always take a scriptable object here when we call this function
    {
        _GameManager.logOpen = true; //we mark this true so arrow keys don't work
        currentLog = getLog; //then we assigned the log we've passed in as the current log
        
        //since the logs can be acquired in any order, we make the numbering still work chronologically by replacing NUM with the current number
        var titletest = currentLog.logTitle.Replace("NUM", currentLog.logNumber.currentLogNumber.ToString()); 
        //then add it to the canvas element
        logTitle.text = titletest;
        
        //\n doesn't work in a scriptable object editor string, so we replace 'NEWLINE' additions with the newline character here
        var bodytext = currentLog.logText.Replace("NEWLINE ", "\n");
        logBody.text = bodytext;
        
        //set the canvas to true
        logCanv.SetActive(true);
        
        //we also update the record text
        recordtextupdate.text = recordtextupdate.text + "------" + "\n" + "\n" +  logTitle.text + "\n" + "\n" + logBody.text + "\n" + "\n";
        
        //and then increase the log number for the next one
        currentLog.logNumber.currentLogNumber++;
        

    }
    
}
