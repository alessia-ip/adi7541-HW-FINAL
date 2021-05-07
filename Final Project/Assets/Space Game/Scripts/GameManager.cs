using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Unity.Mathematics;
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
    public GameObject ret;

    private LineRenderer _lineRenderer;
    
    private GameObject player;

    private GameObject reticle;

    public Vector2 playerPos;
    public Vector2 reticlePos;

    private int width;
    private int height;

    public List<Vector2> pathForShip;
    
    void Start()
    {

        _lineRenderer = GetComponent<LineRenderer>();

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
                    //Debug.Log("X coord: " + x + "\nY coord: " + y + "\nValue: " + currentLine[x]);
                }
            }
            
            MakeGrid();
        
    }

    void MakeGrid() //this is its own function if we ever need to re-draw the grid
    {

        height = nebulaLayout.GetLength(1);
        width = nebulaLayout.GetLength(0);
        
        //Debug.Log("Width: " + width + "\nHeight: " + height);

        var level = new GameObject();
        level.name = "Level";
        
        for (int y = 0; y < nebulaLayout.GetLength(1); y++)
        {
            for (int x = 0; x < nebulaLayout.GetLength(0); x++)
            {
                if (nebulaLayout[x, y] == 'o')
                {
                    var playerObj = Instantiate<GameObject>(ship);
                    playerObj.transform.position = new Vector2(x, y) + gridStartPos + new Vector2(x * xOffset, y * yOffset);
                    player = playerObj;
                    reticle = Instantiate<GameObject>(ret);
                    reticle.transform.position = player.transform.position;

                    playerPos = new Vector2(x, y);
                    reticlePos = playerPos;
                    //Debug.Log(reticlePos + " is the ret pos right now");
                }

                var newStar = Instantiate<GameObject>(nebula);
                newStar.transform.position = new Vector2(x, y) + gridStartPos + new Vector2(x * xOffset, y * yOffset);
                newStar.transform.parent = level.transform;
            }
        }
    }


    private void Update()
    {
        GetNewGridPosition();
    }

    void GetNewGridPosition()
    {
        
        
        //this is for moving the reticle
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (reticlePos.x > 0)
            {

                reticlePos.x--;


            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (reticlePos.y < width - 1)
            {

                reticlePos.x++;



            }
        }
        
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (reticlePos.y > 0)
            {

                reticlePos.y--;



            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (reticlePos.y < height - 1)
            {
                reticlePos.y++;


            }
        }

   
        //we can use the same math equation as in the instantiation to put the reticle at the new position on the grid
        //reticle.transform.position = reticlePos + gridStartPos + new Vector2(reticlePos.x * xOffset, reticlePos.y * yOffset);
        reticle.transform.position = transformedPosition(reticlePos.x, reticlePos.y);


        if (Mathf.Abs(reticlePos.x - playerPos.x) > 1 || Mathf.Abs(reticlePos.y - playerPos.y) > 1)
        {
            for (int x = 0; x != Mathf.Abs(reticlePos.x); x++)
            {
            }
        }

        //this is for making the line renderer
        //this is a very basic pathfinder
        _lineRenderer.SetPosition(0, transformedPosition(playerPos.x, playerPos.y));
        _lineRenderer.SetPosition(1, transformedPosition(reticlePos.x, reticlePos.y));
        
    }


    void pathNodeAdd()
    {
        if (!pathForShip.Contains(reticlePos))
        {
            pathForShip.Add(reticlePos);
        }
    }

    void pathNodeDelete()
    {
        if (pathForShip.Contains(reticlePos))
        {
            pathForShip.Remove(reticlePos);
        }
    }
    

    //needed this vector 2 since I was doing this bit of math so often. 
    Vector2 transformedPosition(float xPos, float yPos)
    {
        var newVector2 = new Vector2(xPos, yPos) + gridStartPos + new Vector2(xPos * xOffset, yPos * yOffset);
        return newVector2;
    }
    
}
