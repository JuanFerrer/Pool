//
//  Instantiated on match start
//  One of it's responsibilities is to instantiate and control the UI object
//  


using UnityEngine;
using System.Collections.Generic;

using Pool; // Pool namespace

public class GameManagerScript : MonoBehaviour
{
    bool DEBUG_shouldChangePlayer = false;
    bool isGameReady = false;

    public enum GameType { ENGLISH_POOL, AMERICAN_POOL, SNOOKER };

    [Header("Table")]
    public GameObject tablePrefab;                              // Prefab of table
    public Vector3 tablePos;                                    // Initial position of table
    public Vector3 tableRot;                                    // Initial rotation of table
    private GameObject table;                                   // Reference to table

    [Header("Balls")]
    public GameObject ballPrefab;                               // Prefab of ball
    public Vector3 ballPos;                                     // Initial position of balls
    private const int BALLS_NO = 16;                            // Amount of balls
    private GameObject[] balls = new GameObject[BALLS_NO];      // Array of all balls (ball 0 is cue ball)
    public Material[] ballMaterial;                             // Material of all balls 

    [Header("Cue")]
    public GameObject cuePrefab;                        // Prefab of cue
    private GameObject cue;                             // Reference to cue

    [Header("Player")]
    public GameObject playerPrefab;                     // Prefab of player
    public Vector3 playerPos;                           // Initial position of player
    private GameObject player;                          // Reference to current player game object
    public float rotationSpeed;                         // Rotation speed 
    [Range(0, 2000)]public float forceApplied;        // Force applied to ball on hit
    [HideInInspector] public bool playerHasControl;     // Flag player is in control

    static private int PLAYER_NO;                       // Amount of players
    [HideInInspector] public int currentPlayer;         // Reference to current players
    [HideInInspector] Player[] players;                 // References to all players

    [Header("Cameras")]
    public GameObject camPrefab;                        // Prefab of cam
    private GameObject mainCam;                         // Reference to main camera object
    private GameObject secCam;                          // Reference to second camera object

    private Camera mainCamera;                          // Reference to main camera component
    private Camera secCamera;                           // Reference to second camera component
    public Vector3 secCamPos;                           // Position of second camera
    public Vector3 secCamRot;                           // Rotation of second camera
    public float secCamSize;                              // Size of orthographic camera
    public float camMaxY;
    public float camMinY;

    [Header("Lights")]
    public GameObject lightPrefab;                      // Prefab of light
    public Vector3 lightPos;                            // Initial position of light
    private const int LIGHTS_NO = 4;                    // Amount of lights in scene

    // FOR DEBUG PURPOSES
    private void Start()
    {
        StartGame(GameType.ENGLISH_POOL, 0, 2);
    }

    // Use this for initialization
    public void StartGame(GameType type, int arenaIndex, int playerAmount)
    {
        SetupVariables(type, arenaIndex, playerAmount);
        SetupScene();
        InstantiateUI();
        isGameReady = true;
    }

    // Update is called once per frame
    // Game loop, if you will
    void FixedUpdate()
    {
        if (isGameReady)
        {
            if (Input.GetKeyDown(KeyCode.C))
                DEBUG_shouldChangePlayer = true;

            if (!playerHasControl && !AnyBallMoving())
            {
                if (ShouldChangePlayer())
                {
                    ChangePlayer();
                }

                GiveControlToPlayer();
            }

            if (IsWinCondition())
            {
                EndGame();
            }
        }
    }

    /*******************/
    /*      SETUP      */
    /*******************/

    // Get variables ready for instantiation
    private void SetupVariables(GameType type, int arenaIndex, int playerAmount)
    {
        PLAYER_NO = playerAmount;
        players = new Player[PLAYER_NO];
    }

    // Setup scene game objects and variables
    private void SetupScene()
    {
        // Create table
        table = Instantiate(tablePrefab, tablePos, Quaternion.Euler(tableRot));

        // Setup player  
        SetupPlayer();

        // Create balls
        SetRack();

        // Get cameras and referecens
        SetupCameras();

        // Get and instantiate lights
        //SetupLights();
    }

    // Instantiate all lights and position them
    private void SetupLights()
    {
        //float[] xOffset = {5, 5, -5, -5 };
        //float[] zOffset = {5, -5, 5, -5 };
        //GameObject[] lights = new GameObject[LIGHTS_NO];


        //for (int i = 0; i < lights.Length; ++i)
        //{
        //    lights[i] = (GameObject)Instantiate(lightPrefab, new Vector3(xOffset[i], 5.0f, zOffset[i]), Quaternion.identity);
        //}

        //Instantiate(lightPrefab, lightPos, Quaternion.identity);
    }

    // Setup all cameras and references
    private void SetupCameras()
    {
        // Has been setup in SetupPlayer (appended as a child of player)
        mainCamera = mainCam.GetComponent<Camera>();

        secCam = (GameObject)Instantiate(camPrefab, secCamPos, Quaternion.Euler(secCamRot));
        secCam.tag = "SecondCam";

        secCamera = secCam.GetComponent<Camera>();
        secCamera.orthographic = true;
        secCamera.orthographicSize = secCamSize;
        secCamera.GetComponent<AudioListener>().enabled = false;

        mainCamera.enabled = true;
        secCamera.enabled = false;
    }

    // Put all balls in position as in a rack
    private void SetRack()
    {
        float[] xOffset = { 0.0f,
                        -0.5f,  0.5f,
                     -1.0f, 0.0f, 1.0f,
                  -1.5f, -0.5f, 0.5f, 1.5f,
               -2.0f, -1.0f, 0.0f, 1.0f, 2.0f};
        float[] zOffset = { 0.0f,
                        0.86f,  0.86f,
                     1.75f, 1.75f, 1.75f,
                  2.62f, 2.62f, 2.62f, 2.62f,
               3.5f, 3.5f, 3.5f, 3.5f, 3.5f};

        GameObject newBall;

        for (int i = 1; i < BALLS_NO; ++i)
        {

            newBall = (GameObject)Instantiate(ballPrefab, new Vector3(ballPos.x + xOffset[i - 1], ballPos.y, ballPos.z + zOffset[i - 1]), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));

            newBall.GetComponent<MeshRenderer>().material = ballMaterial[i];

            newBall.GetComponent<BallScript>().audioSource = newBall.GetComponent<AudioSource>();

            newBall.tag = "Ball";

            balls[i] = newBall;  // Uninitialised?

     
        }
        //player.GetComponent<PlayerControllerScript>().nextBall = balls[1];

        //    Instantiate(ballPrefab, ballPos, Quaternion.identity);
        //    Instantiate(ballPrefab, new Vector3(ballPos.x - 0.5f, ballPos.y, ballPos.z + 1.0f), Quaternion.identity);
        //    Instantiate(ballPrefab, new Vector3(ballPos.x + 0.5f, ballPos.y, ballPos.z + 1.0f), Quaternion.identity);

        //    Instantiate(ballPrefab, new Vector3(ballPos.x - 1.0f, ballPos.y, ballPos.z + 2.0f), Quaternion.identity);
        //    Instantiate(ballPrefab, new Vector3(ballPos.x + 0.0f, ballPos.y, ballPos.z + 2.0f), Quaternion.identity);   // Ball 8
        //    Instantiate(ballPrefab, new Vector3(ballPos.x + 1.0f, ballPos.y, ballPos.z + 2.0f), Quaternion.identity);

        //    Instantiate(ballPrefab, new Vector3(ballPos.x - 1.5f, ballPos.y, ballPos.z + 3.0f), Quaternion.identity);
        //    Instantiate(ballPrefab, new Vector3(ballPos.x - 0.5f, ballPos.y, ballPos.z + 3.0f), Quaternion.identity);
        //    Instantiate(ballPrefab, new Vector3(ballPos.x + 0.5f, ballPos.y, ballPos.z + 3.0f), Quaternion.identity);
        //    Instantiate(ballPrefab, new Vector3(ballPos.x + 1.5f, ballPos.y, ballPos.z + 3.0f), Quaternion.identity);

        //    Instantiate(ballPrefab, new Vector3(ballPos.x - 2.0f, ballPos.y, ballPos.z + 4.0f), Quaternion.identity);
        //    Instantiate(ballPrefab, new Vector3(ballPos.x - 1.0f, ballPos.y, ballPos.z + 4.0f), Quaternion.identity);
        //    Instantiate(ballPrefab, new Vector3(ballPos.x + 0.0f, ballPos.y, ballPos.z + 4.0f), Quaternion.identity);
        //    Instantiate(ballPrefab, new Vector3(ballPos.x + 1.0f, ballPos.y, ballPos.z + 4.0f), Quaternion.identity);
        //    Instantiate(ballPrefab, new Vector3(ballPos.x + 2.0f, ballPos.y, ballPos.z + 4.0f), Quaternion.identity);
    }

    // Create player with camera
    private void SetupPlayer()
    {
        currentPlayer = 0;

        for (int i = 0; i < PLAYER_NO; ++i)
        {
            players[i] = new Player();
            players[i].SetPlayerNo(i);
        }

        playerHasControl = true;
        // Player model
        player = (GameObject)Instantiate(playerPrefab, playerPos, Quaternion.identity);
        balls[0] = player;

        mainCam = (GameObject)Instantiate(camPrefab, new Vector3(0.0f, player.transform.position.y + 1.0f, player.transform.position.z - 3.0f), Quaternion.Euler(10.0f, 0.0f, 0.0f));
        mainCam.transform.SetParent(player.transform);

        // Cue model
        cue = (GameObject)Instantiate(cuePrefab, new Vector3(0.0f, player.transform.position.y, player.transform.position.z - 4.0f), cuePrefab.transform.rotation);
        cue.transform.SetParent(mainCam.transform);

        // Set player variables
        player.GetComponent<PlayerControllerScript>().rotSpeed = rotationSpeed;
        player.GetComponent<PlayerControllerScript>().mainCam = mainCam;
        player.GetComponent<PlayerControllerScript>().forceApplied = forceApplied;
        player.GetComponent<PlayerControllerScript>().gameManager = this.gameObject;
        player.GetComponent<PlayerControllerScript>().camMaxY = camMaxY;
        player.GetComponent<PlayerControllerScript>().camMinY = camMinY;
        player.GetComponent<PlayerControllerScript>().audioSource = player.GetComponent<AudioSource>();
    }

    // Instantiate UI object
    private void InstantiateUI()
    {

    }

    /*******************/
    /*     PLAYER      */
    /*******************/

    // Ask every ball to see if they're still moving
    private bool AnyBallMoving()
    {
        for (int i = 0; i < BALLS_NO; ++i)
        {
            if (balls[i].GetComponent<BallScript>().isMoving)
                if (Mathf.Approximately(balls[i].GetComponent<Rigidbody>().velocity.magnitude, 0.0f))   // Stopped moving
                {
                    balls[i].GetComponent<BallScript>().isMoving = false;
                }
                else return true;
        }
        return false;
    }

    // Return control to current player
    private void GiveControlToPlayer()
    {
        playerHasControl = true;
        player.GetComponent<PlayerControllerScript>().ResetPlayerView();

        mainCam.transform.SetParent(player.transform);
    }

    // Check rules and see if changing the player is needed
    // Heuristics/game rules
    private bool ShouldChangePlayer()
    {
        bool temp = DEBUG_shouldChangePlayer;
        DEBUG_shouldChangePlayer = false;
        return temp;
    }

    // Change player, select next ball, update text, etc.
    private void ChangePlayer()
    {

        // TODO
        currentPlayer = (currentPlayer + 1) % PLAYER_NO;
        Debug.Log("Turn of player " + currentPlayer);

        SelectNextBallForPlayer();
    }

    // Make nextBall be the appropriate for the player
    private void SelectNextBallForPlayer()
    {
        // TODO
    }

    // Change rendering camera
    public void ToggleCamera()
    {
        if (mainCamera.enabled == true)
        {
            mainCamera.enabled = false;
            secCamera.enabled = true;
        }
        else
        {
            mainCamera.enabled = true;
            secCamera.enabled = false;
        }
    }

    /******************/
    /*     LOGIC      */
    /******************/

    // Check if last turn, player won or lost
    // Heuristics/game rules
    private bool IsWinCondition()
    {
        // TODO
        return false;
    }

    // Give control back to MainMenu? TBD
    private void EndGame()
    {

    }

    /***************/
    /*     UI      */
    /***************/

    // Meant to be used by the UI object. Returns score of current player
    public int GetPlayerScore()
    {
        return players[currentPlayer].GetScore();
    }
}
