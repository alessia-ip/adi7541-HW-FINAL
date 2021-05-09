using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Numerics;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

[System.Serializable] 
public class GameManager : MonoBehaviour
{

    [Header("Saving and Loading")]
    private const string FILEPATH_nebulaLayout = "NebulaLayout1";
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

    private GameObject xPosTracker;
    private GameObject yPosTracker;

    public bool GameStart = false;
    
    private bool shipInMotion = false;
    private bool yBetter = false;
    private bool isAngled = false;
    private bool atTracker = false;
    private Vector2 velocity = Vector2.zero;
    private float travelSpeed = 0.3f;

    public bool logOpen = true;
    public LogManager _LogManager;
    public GameObject logCanvas;
    
    public List<LogScriptableObjects> listOfAllLogs;

    private int tripNum = 10;

    public LogScriptableObjects lastLogEntry;

    public TextMeshProUGUI tripTextTracker;
    
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
            for (int y = 0; y < nebulaStrings.Length - 1; y++) // we are getting the y coords from the number of lines in the array
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
        
        for (int y = 0; y < nebulaLayout.GetLength(1) -1 ; y++)
        {
            for (int x = 0; x < nebulaLayout.GetLength(0) -1; x++)
            {
                var newStar = Instantiate<GameObject>(nebula);
                newStar.transform.position = new Vector2(x, y) + gridStartPos + new Vector2(x * xOffset, y * yOffset);
                newStar.transform.parent = level.transform;
                newStar.name = "New Star at " + x + " and " + y;
                
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

                    xPosTracker = new GameObject();
                    xPosTracker.name = "X Position";
                    yPosTracker = new GameObject();
                    yPosTracker.name = "y Position";

                    xPosTracker.transform.position = player.transform.position;
                    yPosTracker.transform.position = player.transform.position;

                    newStar.GetComponent<NebulaLog>().alreadyRead = true;
                    
                }
                else
                {
                    var assignedLogNum = Random.Range(0, listOfAllLogs.Count - 1);
                    newStar.GetComponent<NebulaLog>().logDate = listOfAllLogs[assignedLogNum];
                    listOfAllLogs.RemoveAt(assignedLogNum);
                }
                
            }
        }
    }


    private void Update()
    {
        if (GameStart == true && tripNum != 0)
        {
            if (logOpen == false)
            {
                if (shipInMotion == false)
                {
                    GetNewGridPosition();
                    MoveShipTrigger();
                    
                }
                else
                {
                    MoveShip();
                }
            }
        } else if (logOpen == false && tripNum == 0)
        {
            Debug.Log("end of game");
        }

    }


    void CheckPlanetaryLog(float x, float y)
    {
       
        var nebulaObj = GameObject.Find("New Star at " + x + " and " + y);
        Debug.Log(nebulaObj.transform.position);
        if (nebulaObj.GetComponent<NebulaLog>().alreadyRead == false)
        {
            tripNum--;
            tripTextTracker.text = "Trips remaining: " + tripNum;
            logOpen = true;
            logCanvas.SetActive(true);
            if (tripNum != 0)
            {
                _LogManager.openLog(nebulaObj.GetComponent<NebulaLog>().logDate);
                nebulaObj.GetComponent<NebulaLog>().alreadyRead = true;
            }
            else
            {
                _LogManager.openLog(lastLogEntry);
                nebulaObj.GetComponent<NebulaLog>().alreadyRead = true;
            }
        }
        
    }
    
    //function used to navigate the grid of stars/nebulas
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
        xPosTracker.transform.position = new Vector2(reticle.transform.position.x, xPosTracker.transform.position.y);
        yPosTracker.transform.position = new Vector2(yPosTracker.transform.position.x, reticle.transform.position.y);


        var middlePos = new Vector3(0, 0); 
            
        
        if (Mathf.Abs(reticlePos.x - playerPos.x) > 0 || Mathf.Abs(reticlePos.y - playerPos.y) > 0)
        {
            if (Mathf.Abs(reticlePos.x - playerPos.x) > Mathf.Abs(reticlePos.y - playerPos.y))
            {
                middlePos = xPosTracker.transform.position;
                yBetter = false;
                isAngled = true;
            }
            else
            {
                middlePos = yPosTracker.transform.position;
                yBetter = true;
                isAngled = true;

            }
        }
        else
        {
            middlePos = player.transform.position;
            isAngled = false;
        }

        //this is for making the line renderer
        //this is a very basic pathfinder
        _lineRenderer.SetPosition(0, transformedPosition(playerPos.x, playerPos.y));
        _lineRenderer.SetPosition(1, middlePos);
        _lineRenderer.SetPosition(2, transformedPosition(reticlePos.x, reticlePos.y));
        
    }


    //making this its own function in case I have time to add juice to it. 
    //Really want an animation and SFX
    void MoveShipTrigger()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            atTracker = false;
            shipInMotion = true;

        }   
    }

    void MoveShip()
    {
        if (player.transform.position == reticle.transform.position)
        {
            playerPos = reticlePos;
            xPosTracker.transform.position = player.transform.position;
            yPosTracker.transform.position = player.transform.position;
            atTracker = false;
            shipInMotion = false;
            CheckPlanetaryLog(playerPos.x, playerPos.y);
        }
        
        if (isAngled == false)
        {
            player.transform.position = Vector2.SmoothDamp(player.transform.position, reticle.transform.position, ref velocity ,travelSpeed);
            if (Vector2.Distance(player.transform.position, reticle.transform.position) < 0.3)
            {
                player.transform.position = reticle.transform.position;
                
            }
        }
        else
        {
            if (yBetter == true)
            {
                if (player.transform.position == yPosTracker.transform.position)
                {
                    atTracker = true;
                }

                if (atTracker == false)
                {
                    //Debug.Log(Vector2.Distance(player.transform.position, yPosTracker.transform.position));
                    player.transform.position = Vector2.SmoothDamp(player.transform.position, yPosTracker.transform.position,  ref velocity,  travelSpeed);
                    if (Vector2.Distance(player.transform.position, yPosTracker.transform.position) < 0.3)
                    {
                        player.transform.position = yPosTracker.transform.position;
                        
                    }
                 
                }
                else
                {
                    player.transform.position = Vector2.SmoothDamp(player.transform.position, reticle.transform.position, ref velocity,travelSpeed);
                    
                    if (Vector2.Distance(player.transform.position, reticle.transform.position) < 0.3)
                    {
                        player.transform.position = reticle.transform.position;
                    }
                }
            }
            else
            { 
                if (player.transform.position == xPosTracker.transform.position)
                {
                    atTracker = true;
                }

                if (atTracker == false)
                {
                    player.transform.position = Vector2.SmoothDamp(player.transform.position, xPosTracker.transform.position, ref velocity, travelSpeed);
                    if (Vector2.Distance(player.transform.position, xPosTracker.transform.position) < 0.3)
                    {
                        player.transform.position = xPosTracker.transform.position;
                    }
                }
                else
                {
                    player.transform.position = Vector2.SmoothDamp(player.transform.position, reticle.transform.position,  ref velocity,travelSpeed);
                    if (Vector2.Distance(player.transform.position, reticle.transform.position) < 0.3)
                    {
                        player.transform.position = reticle.transform.position;
                    }
                }
            }
        }


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
