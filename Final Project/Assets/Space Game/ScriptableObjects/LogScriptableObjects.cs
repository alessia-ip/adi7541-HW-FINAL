using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Log", menuName = "ScriptableObjects/Logs", order = 1)]
public class LogScriptableObjects : ScriptableObject
{
    public LogNumberTracker logNumber;
    public string logTitle = "Log Date: 1-M NUM";
    public string logText;

}
