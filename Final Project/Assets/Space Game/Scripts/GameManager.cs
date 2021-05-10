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

    public GameObject success;
    
    [Header("Audio Effects")] 
    public AudioSource audPlayer;
    public AudioClip routePlan;
    public AudioClip shipMove;
    public AudioClip openLog;
    
    void Start()
    {
        //just grabbing the line renderer I use later
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
                    //this line is to set the size of the array on the x and y axis
                    //we get this from the # of lines, and then the chars in each line
                    //this only needs to happen once, if the array is null
                    
                    //Debug.Log("was null");
                }
                for (int x = 0; x < currentLine.Length - 1; x++)
                {
                    nebulaLayout[x, y] = currentLine[x]; //with the x and y in mind from these for loops, we can assign each CHAR a position in this 2D array
                    //This 2d array is what I use to then make the grid afterwards/ 
                    
                    //Debug.Log("X coord: " + x + "\nY coord: " + y + "\nValue: " + currentLine[x]);
                }
            }
            
            MakeGrid();
            //then we make the grid
    }

    void MakeGrid() //this is its own function if we ever need to re-draw the grid
    { //I never did need to call it a second time

        height = nebulaLayout.GetLength(1) -1;
        width = nebulaLayout.GetLength(0) -1;
        //we get the height and width easily from these two dimensions. 
        
        //Debug.Log("Width: " + width + "\nHeight: " + height);

        var level = new GameObject();
        level.name = "Level";
        //this level object is used for holding many of the spawned objects, for a cleaner hierarchy 
        
        for (int y = 0; y < nebulaLayout.GetLength(1) -1 ; y++)
        {
            for (int x = 0; x < nebulaLayout.GetLength(0) -1; x++)
            {
                var newStar = Instantiate<GameObject>(nebula); //we make a new nebula
                newStar.transform.position = new Vector2(x, y) + gridStartPos + new Vector2(x * xOffset, y * yOffset); //then put it at the correct position
                newStar.transform.parent = level.transform; //make it a child of 'level'
                newStar.name = "New Star at " + x + " and " + y; //and then name it so it's easy to find after
                
                if (nebulaLayout[x, y] == 'o') //if the CHAR is an 'o', that's what I wanted the player to spawn 
                {//the player spawns ONTOP of a galaxy, not in place of one
                    var playerObj = Instantiate<GameObject>(ship); //we make the player object
                    playerObj.transform.position = new Vector2(x, y) + gridStartPos + new Vector2(x * xOffset, y * yOffset); //set the player position
                    player = playerObj; //then assign it to the player var
                    reticle = Instantiate<GameObject>(ret); //we also instantiate the reticle 
                    reticle.transform.position = player.transform.position; //and assign it to the same position

                    playerPos = new Vector2(x, y); //then we keep track of the player pos
                    reticlePos = playerPos; // and reticle position
                    //these are referenced later, and it is easier to keep track of them as ints and then perform the math on them
                    //instead of keeping track of them just from the transform.position
                    
                    //Debug.Log(reticlePos + " is the ret pos right now");

                    //these two objects make pathfinding easier
                    //one moves only on the y axis
                    //the other moves only on the x axis
                    //meaning, it is easy to track the corner bend of any path 
                    xPosTracker = new GameObject();
                    xPosTracker.name = "X Position";
                    yPosTracker = new GameObject();
                    yPosTracker.name = "y Position";

                    //they start at the player position
                    xPosTracker.transform.position = player.transform.position;
                    yPosTracker.transform.position = player.transform.position;

                    //the star the player starts on is where the original log is read from. 
                    //aka we do not want the player to re-read it
                    //nor are we assigning a scriptable object to it
                    newStar.GetComponent<NebulaLog>().alreadyRead = true;
                    
                }
                else
                {
                    //if it isn't the player, we know the galaxy needs to have a log assigned to it
                    //we've got one large list of all the logs
                    //a random one is assigned, and then removed from the list, so there are no duplicates
                    var assignedLogNum = Random.Range(0, listOfAllLogs.Count - 1);
                    newStar.GetComponent<NebulaLog>().logDate = listOfAllLogs[assignedLogNum];
                    listOfAllLogs.RemoveAt(assignedLogNum);
                }
                
            }
        }
    }


    private void Update() //update is pretty short
    {
        if (GameStart == true && tripNum != 0) //the game starts with GameStart being false. Ergo, none of this really happens until you hit the start button on the main screen
        {
            if (logOpen == false) //when the UI is not open, there's 2 states
            {
                //state 1 is if the ship is not currently moving
                //the player can then plan their route, and decide whether to take it or not
                if (shipInMotion == false)
                {
                    GetNewGridPosition();
                    MoveShipTrigger();
                    
                }
                else //otherwise, the ship is moving
                {
                    MoveShip();
                }
            }
        } else if (logOpen == false && tripNum == 0) //this is triggered at the very end of the game only
        {
            _lineRenderer.enabled = false; //the line renderer needs to be turned off to avoid jank
            Debug.Log("end of game");
        }

    }


    void CheckPlanetaryLog(float x, float y) //this is to check the log
    {
        var nebulaObj = GameObject.Find("New Star at " + x + " and " + y); //now we can get the named gameobject based on the position fed into this function
        //it's a little bit expensive to find it by name, but the game isn't overall that expensive so that's what I've used here
        
        Debug.Log(nebulaObj.transform.position);
        
        if (nebulaObj.GetComponent<NebulaLog>().alreadyRead == false) //we do not want to read the log a second time if it is already read, so we check that it hasn't been
        {//previously read logs are already available as a record, and having it be duplicated would ruin the 10 trip count
            tripNum--; //assuming it hasn't been read, we subtract one from the number of trips left
            tripTextTracker.text = "Trips remaining: " + tripNum; //then update the UI
            logOpen = true; //set this bool to true so the player can't move the ship in the background
            logCanvas.SetActive(true); //then auto-open the canvas
            if (tripNum != 0) //if the # of trips is over 0:
            {
                playAudioClip(openLog); //play the audio clip
                _LogManager.openLog(nebulaObj.GetComponent<NebulaLog>().logDate); //then open that nebula's assigned log 
                nebulaObj.GetComponent<NebulaLog>().alreadyRead = true; //then mark it as read
            }
            else
            {
                playAudioClip(openLog);
                _LogManager.openLog(lastLogEntry); //otherwise, we are overriding it with the final log entry if it is the very last trip
                nebulaObj.GetComponent<NebulaLog>().alreadyRead = true;
            }
        }
        
    }
    
    //function used to navigate the grid of stars/nebulas
    void GetNewGridPosition()
    {
        
        
        //THIS IS PLAYER INPUT USING ARROW KEYS
        
        //this is for moving the reticle
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (reticlePos.x > 0) //we do not want the reticle to move into the negatives
            {
                reticlePos.x--; //otherwise we move it one to the left
                playAudioClip(routePlan); //we also play the audio clip
            }
        } //THIS IS REPEATED FOR THE OTHER ARROW KEYS

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (reticlePos.x < width - 1) //the only difference is that we are making sure it won't exceed the width/height 
            {
                reticlePos.x++;
                playAudioClip(routePlan);
                
            }
        }
        
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (reticlePos.y > 0)
            {

                reticlePos.y--;
                //Debug.Log(reticlePos.y);
                playAudioClip(routePlan);



            }
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (reticlePos.y < height - 1)
            {
                reticlePos.y++;
                //Debug.Log(reticlePos.y);
                playAudioClip(routePlan);

            }
        }

        //END PLAYER ARROW KEY INPUT SECTION
        
   
        //we can use the same math equation as in the instantiation to put the reticle at the new position on the grid
        //reticle.transform.position = reticlePos + gridStartPos + new Vector2(reticlePos.x * xOffset, reticlePos.y * yOffset);
        reticle.transform.position = transformedPosition(reticlePos.x, reticlePos.y);
        
        //the trackers are only moved to match on their respective x / y positions
        xPosTracker.transform.position = new Vector2(reticle.transform.position.x, xPosTracker.transform.position.y);
        yPosTracker.transform.position = new Vector2(yPosTracker.transform.position.x, reticle.transform.position.y);


        var middlePos = new Vector3(0, 0); //this middle pos will determine the center of 3 nodes on the line renderer
            
        //we need the absolute values because the value being - or + has no bearing here
        if (Mathf.Abs(reticlePos.x - playerPos.x) > 0 || Mathf.Abs(reticlePos.y - playerPos.y) > 0) //if the absolute value difference is over 0, we want the middle pos to EITHER be at the y or x bend
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
        else //otherwise that middle position should stay the player position to keep the line smooth
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
        if (Input.GetKeyDown(KeyCode.Space)) //this is to trigger the ship movement
        {
            atTracker = false; //the ship is not at the tracker OR the final position at the start
            shipInMotion = true; //and we don't want the player moving the reticle now either
            playAudioClip(shipMove);

        }   
    }

    void MoveShip() //this is how we move the script across the grid
    {
        if (player.transform.position == reticle.transform.position) //if the player is at the final position, we know it's at it's destination
        {
            //we reset a lot of the positions to the new spot here
            playerPos = reticlePos;
            xPosTracker.transform.position = player.transform.position;
            yPosTracker.transform.position = player.transform.position;
            //tag these bools as false so the player would be able to move things again when the log is closed
            atTracker = false;
            shipInMotion = false;
            //then we call the log open function
            CheckPlanetaryLog(playerPos.x, playerPos.y);
        }
        
        if (isAngled == false) //if there is no bend in the path, we just want to smoothly glide the player directly to the endpoint
        {
            player.transform.position = Vector2.SmoothDamp(player.transform.position, reticle.transform.position, ref velocity ,travelSpeed);
            if (Vector2.Distance(player.transform.position, reticle.transform.position) < 0.3)
            {
                player.transform.position = reticle.transform.position;
                
            }
        }
        else //otherwise we have to account for the corner
        {
            if (yBetter == true) //we check which bend the line renderer is at, so that the ship follows the line
            {
                if (player.transform.position == yPosTracker.transform.position) //and if the player reaches the corner, we need to keep track of that to change direction
                {
                    atTracker = true; 
                }

                if (atTracker == false) //then we glide the ship along the line (this one is towards the tracking pos
                {
                    //Debug.Log(Vector2.Distance(player.transform.position, yPosTracker.transform.position));
                    player.transform.position = Vector2.SmoothDamp(player.transform.position, yPosTracker.transform.position,  ref velocity,  travelSpeed);
                    if (Vector2.Distance(player.transform.position, yPosTracker.transform.position) < 0.3)
                    {
                        player.transform.position = yPosTracker.transform.position;
                        
                    }
                 
                }
                else //this is towards the end pos where the reticle is
                {
                    player.transform.position = Vector2.SmoothDamp(player.transform.position, reticle.transform.position, ref velocity,travelSpeed);
                    
                    if (Vector2.Distance(player.transform.position, reticle.transform.position) < 0.3)
                    {
                        player.transform.position = reticle.transform.position;
                    }
                }
            }
            else //repeat the above, but for the x tracker
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


    public void playAudioClip(AudioClip clip) //I assigned this to a few buttons so it made sense to make a function for it
    {
        audPlayer.PlayOneShot(clip);
    }

    
    //needed this vector 2 since I was doing this bit of math so often. 
    Vector2 transformedPosition(float xPos, float yPos)
    {
        var newVector2 = new Vector2(xPos, yPos) + gridStartPos + new Vector2(xPos * xOffset, yPos * yOffset);
        return newVector2;
    }

    public void checkSuccess()
    {
        if (tripNum == 0)
        {
            success.SetActive(true);
        }
    }
    
    
}
