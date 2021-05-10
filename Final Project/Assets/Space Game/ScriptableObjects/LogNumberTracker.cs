using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LogTracker", menuName = "ScriptableObjects/LogTracker", order = 2)]

public class LogNumberTracker : ScriptableObject
{
    //this keeps track of the log number and is added to every Log Scriptable Object so they are all up to date
    //this gets around the need for a singleton, and makes the value easy to assign / change
    public int currentLogNumber;
}
