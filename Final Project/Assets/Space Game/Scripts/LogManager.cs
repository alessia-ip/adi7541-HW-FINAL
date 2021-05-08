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
    
    public void closeLog()
    {
        _GameManager.logOpen = false;
        logCanv.SetActive(false);

    }

    public void openLog(LogScriptableObjects getLog)
    {
        _GameManager.logOpen = true;
        currentLog = getLog;
        logTitle.text = currentLog.logTitle;
        var bodytext = currentLog.logText.Replace("NEWLINE", "\n");
        logBody.text = bodytext;
        logCanv.SetActive(true);
    }

    public void reviewLog()
    {
        
    }
}
