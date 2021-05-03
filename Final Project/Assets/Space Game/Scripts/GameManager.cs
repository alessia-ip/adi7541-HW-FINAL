using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

[System.Serializable] 
public class GameManager : MonoBehaviour
{

    [Header("Saving and Loading")]
    private const string FILEPATH_nebulaLayout = "NebulaLayout";
    private TextAsset DATA_nebulaLayout;
    public char[,] nebulaLayout;

    [Header("Init Variables and Objects")] 
    public Vector2 gridStartPos;
    public int xOffset;
    public int yOffset;
    public GameObject ship;
    public GameObject nebula;
    
    void Start()
    {
        
        //this is to first get the grid data - this should only happen ONCE, when you start the game. 
        //we don't need to check if the file exists, because it is necessary for it to exist at the start
        //It will never be first created at runtime
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
                    //Debug.Log("was null");
                }
                for (int x = 0; x < currentLine.Length - 1; x++)
                {
                    nebulaLayout[x, y] = currentLine[x];
                    Debug.Log("X coord: " + x + "\nY coord: " + y + "\nValue: " + currentLine[x]);
                }
            }
            
            MakeGrid();
        
    }

    void MakeGrid() //this is its own function if we ever need to re-draw the grid
    {

        for (int y = 0; y < nebulaLayout.GetLength(1); y++)
        {
            for (int x = 0; x < nebulaLayout.GetLength(0); x++)
            {
                if (nebulaLayout[x, y] == 'o')
                {
                    var playerObj = Instantiate<GameObject>(ship);
                    playerObj.transform.position = new Vector2(x, y) + gridStartPos + new Vector2(x * xOffset, y * yOffset);
                }

                var newStar = Instantiate<GameObject>(nebula);
                newStar.transform.position = new Vector2(x, y) + gridStartPos + new Vector2(x * xOffset, y * yOffset);

            }
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
