using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class GameManager : MonoBehaviour
{

    [Header("Saving and Loading")]
    private const string FILEPATH_nebulaLayout = "NebulaLayout";
    private TextAsset DATA_nebulaLayout;
    public char[,] nebulaLayout;
    
    void Start()
    {
            DATA_nebulaLayout = Resources.Load(FILEPATH_nebulaLayout) as TextAsset;
            var tempLayout = DATA_nebulaLayout.text;
            // Debug.Log(tempLayout.ToCharArray().Length); - note, a char array keeps the newline characters
            var nebulaStrings = tempLayout.Split('\n');
            for (int y = 0; y < nebulaStrings.Length; y++) // we are getting the y coords from the number of lines in the array
            {
                var currentLine = nebulaStrings[y].ToCharArray();
                if (nebulaLayout == null)
                {
                    nebulaLayout = new char[currentLine.Length, nebulaStrings.Length];
                    Debug.Log("was null");
                }
                for (int x = 0; x < currentLine.Length; x++)
                {
                    nebulaLayout[x, y] = currentLine[x];
                }
            }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
