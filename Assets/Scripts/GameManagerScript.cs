//
//  Instantiated on match start
//  One of it's responsibilities is to instantiate and control the UI object
//  


using UnityEngine;
using System.Collections.Generic;

using Pool; // Pool namespace

public class GameManagerScript : MonoBehaviour
{
    public bool GameReady { get; set; }
    public bool IsInitialReposition { get; set; }
    private bool CueBallPotted { get; set; }
    private bool BlackBallPotted{ get; set; }
    public enum GameType { ENGLISH_POOL, AMERICAN_POOL, SNOOKER };
    private bool IsEndGame { get; set; }

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
    private List<GameObject> pottedBalls;                       // List of every potted ball in a given turn. pottedBalls is reset at the beginning of each turn

    [Header("Cue")]
    public GameObject cuePrefab;                        // Prefab of cue
    private GameObject cue;                             // Reference to cue

    [Header("Player")]
    public GameObject playerPrefab;                         // Prefab of player
    public Vector3 playerPos;                               // Initial position of player
    private GameObject player;                              // Reference to current player game object
    public float rotationSpeed;                             // Rotation speed 
    //[Range(0, 2000)]public float forceApplied;              // Force applied to ball on hit
    [HideInInspector] public bool playerHasControl;         // Flag player is in control
    [HideInInspector] public bool playerIsRepositioning;    // Flag player is repositioning
    public int FirstBallHit { get; set; }                   // Number of ball first hit
    public float minPower;                                  // Base power
    public float fullPowerBonus;                            // Amount of extra power on cue strike when charged to full power
    public float timeToFullPower;                           // Amount of time needed to hold the hi button to charged to full power
    public float charge;

    static private int PLAYER_NO;                       // Amount of players
    [HideInInspector] public int currentPlayer;         // Reference to current player
    [HideInInspector] public int winnerPlayer;          // Reference to winner player
    [HideInInspector] Player[] players;                 // References to all players
    private bool IsChangePlayer { get; set; }           // Player needs to change on next turn
    private bool ShouldRepositionCueBall { get; set; }  // Cue ball was potted flag
    private bool IsControlToPlayer { get; set; }        // Player should regain control

    [Header("Cameras")]
    public GameObject camPrefab;                        // Prefab of cam
    private GameObject mainCam;                         // Reference to main camera object
    private GameObject secCam;                          // Reference to second camera object

    private Camera mainCamera;                          // Reference to main camera component
    private Camera secCamera;                           // Reference to second camera component
    public Vector3 secCamPos;                           // Position of second camera
    public Vector3 secCamRot;                           // Rotation of second camera
    public float secCamSize;                            // Size of orthographic camera
    public float camMaxY;
    public float camMinY;

    [Header("Lights")]
    public GameObject lightPrefab;                      // Prefab of light
    public Vector3 lightPos;                            // Initial position of light
    public Vector3 lightRot;                            // Initial rotation of light

    [Header("UI")]
    public GameObject UIPrefab;                         // Prefab of UI object
    private GameObject UI;             // Reference to UI object

    // FOR DEBUG PURPOSES
    private void Start()
    {
        StartGame(GameType.ENGLISH_POOL, 0, 2);
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    /// <param name="type"></param>
    /// <param name="arenaIndex"></param>
    /// <param name="playerAmount"></param>
    public void StartGame(GameType type, int arenaIndex, int playerAmount)
    {
        SetupVariables(type, arenaIndex, playerAmount);
        SetupScene();
        InstantiateUI();
        GameReady = true;
        IsInitialReposition = true;
    }

    /// <summary>
    /// Update is called once per frame
    /// Game loop, if you will
    /// <summary>
    void FixedUpdate()
    {
        // Are we ready to play?
        if (GameReady)
        {
            if (mainCam.GetComponent<CameraScript>().IsReady && IsControlToPlayer)
                GiveControlToPlayer();
            else if (!playerHasControl && !AnyBallMoving() && !playerIsRepositioning && mainCam.GetComponent<CameraScript>().IsReady)
            {
                // At the end of each turn, players will change unless the current players pots one of his balls
                IsChangePlayer = true;

                // Balls have stopped. Let's check what happened and act accordingly
                CheckPottedBalls();

                // Check what ball we hit first
                CheckFirstHitBall();

                //UI.GetComponent<UIScript>().UpdateUI();

                // Decide if the player needs to change
                if (ShouldChangePlayer())
                {
                    ChangePlayer();
                }

                // Then start moving to ball
                //mainCam.GetComponent<CameraScript>().IsReady = false;
                mainCam.GetComponent<CameraScript>().CamToBall();
                IsControlToPlayer = true;
            }

            if (BlackBallPotted)
            {
                EndGame();
            }
        }
        else
        {
            Debug.Log("Show title");

            if (Input.GetKey(KeyCode.Escape))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
        }
    }

    #region SETUP

    /// <summary>
    /// Get variables ready for instantiation
    /// </summary>
    /// <param name="type"></param>
    /// <param name="arenaIndex"></param>
    /// <param name="playerAmount"></param>
    private void SetupVariables(GameType type, int arenaIndex, int playerAmount)
    {
        GameReady = false;
        CueBallPotted = false;
        BlackBallPotted = false;
        IsEndGame = false;
        IsChangePlayer = false;
        ShouldRepositionCueBall = true;
        IsControlToPlayer = false;

        pottedBalls = new List<GameObject>();

        PLAYER_NO = playerAmount;
        players = new Player[PLAYER_NO];

        UI = (GameObject)Instantiate(UIPrefab);

    }

    /// <summary>
    /// Setup scene game objects and variables
    /// </summary>
    private void SetupScene()
    {
        // Create table
        table = Instantiate(tablePrefab, tablePos, Quaternion.Euler(tableRot));

        // Setup player  
        SetupPlayer();

        // Create balls
        SetRack();

        // Get cameras and references
        SetupCameras();

        // Get and instantiate lights
        SetupLights();
    }

    /// <summary>
    /// Instantiate all lights and position them
    /// </summary>
    private void SetupLights()
    {
       Instantiate(lightPrefab, lightPos, Quaternion.Euler(lightRot));
    }

    /// <summary>
    /// Setup all cameras and references
    /// </summary>
    private void SetupCameras()
    {
        // Has been setup in SetupPlayer (appended as a child of player)
        mainCamera = mainCam.GetComponent<Camera>();
        mainCam.AddComponent<CameraScript>();
        mainCam.GetComponent<CameraScript>().IsReady = false;
        mainCam.GetComponent<CameraScript>().player = player;

        secCam = (GameObject)Instantiate(camPrefab, secCamPos, Quaternion.Euler(secCamRot));
        secCam.tag = "SecondCam";

        secCamera = secCam.GetComponent<Camera>();
        secCamera.orthographic = true;
        secCamera.orthographicSize = secCamSize;
        secCamera.GetComponent<AudioListener>().enabled = false;

        mainCamera.enabled = true;
        secCamera.enabled = false;
    }

    /// <summary>
    /// Put all balls in position as in a rack
    /// </summary>
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

            // Balls 1 to 7 are SPOT, 9 to 15 are STRIPES, ball 8 is BLACK
            if (i < 8)          newBall.GetComponent<BallScript>().BallType = BallType.SPOT;
            else if (i == 8)    newBall.GetComponent<BallScript>().BallType = BallType.BLACK;
            else                newBall.GetComponent<BallScript>().BallType = BallType.STRIPE;

            newBall.GetComponent<BallScript>().BallNo = i;

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
        FreezeBalls();
    }

    /// <summary>
    /// Create player with camera
    /// </summary>
    private void SetupPlayer()
    {
        currentPlayer = 0;

        for (int i = 0; i < PLAYER_NO; ++i)
        {
            players[i] = new Player();
            players[i].SetPlayerNo(i);
            players[i].SetPlayerType(BallType.NONE);
        }

        // __CHANGED
        playerHasControl = false;
        playerIsRepositioning = true;
        // Player model
        player = (GameObject)Instantiate(playerPrefab, playerPos, Quaternion.identity);
        balls[0] = player;

        mainCam = (GameObject)Instantiate(camPrefab, new Vector3(0.0f, player.transform.position.y + 1.0f, player.transform.position.z - 3.0f), Quaternion.Euler(10.0f, 0.0f, 0.0f));
        mainCam.transform.SetParent(player.transform);

        // Cue model
        cue = (GameObject)Instantiate(cuePrefab, new Vector3(0.0f, player.transform.position.y, player.transform.position.z - 4.0f), cuePrefab.transform.rotation);
        cue.transform.SetParent(mainCam.transform);

        // Set player variables
        player.GetComponent<PlayerControllerScript>().BallType = BallType.CUE;
        player.GetComponent<PlayerControllerScript>().rotSpeed = rotationSpeed;
        player.GetComponent<PlayerControllerScript>().mainCam = mainCam;
        //player.GetComponent<PlayerControllerScript>().forceApplied = forceApplied;
        player.GetComponent<PlayerControllerScript>().minPower = minPower;
        player.GetComponent<PlayerControllerScript>().fullPowerBonus = fullPowerBonus;
        player.GetComponent<PlayerControllerScript>().timeToFullPower = timeToFullPower;
        player.GetComponent<PlayerControllerScript>().charge = charge;
        player.GetComponent<PlayerControllerScript>().gameManager = this.gameObject;
        player.GetComponent<PlayerControllerScript>().camMaxY = camMaxY;
        player.GetComponent<PlayerControllerScript>().camMinY = camMinY;
        player.GetComponent<PlayerControllerScript>().audioSource = player.GetComponent<AudioSource>();
    }

    /// <summary>
    /// Instantiate UI object
    /// </summary>
    private void InstantiateUI()
    {
        //UI.GetComponent<UIScript>().Power = 15;
    }

    #endregion

    #region PLAYER

    // Returns the current player script
    public Player GetCurrentPlayer()
    {
        return players[currentPlayer];
    }

    /// <summary>
    /// Ask every ball to see if they're still moving
    /// </summary>
    /// <returns></returns>
    private bool AnyBallMoving()
    {
        for (int i = 0; i < BALLS_NO; ++i)
        {
            if (balls[i] != null)
            {
                if (balls[i].GetComponent<BallScript>().isMoving)
                    if (Mathf.Approximately(balls[i].GetComponent<Rigidbody>().velocity.magnitude, 0.0f))   // Stopped moving
                    {
                        balls[i].GetComponent<BallScript>().isMoving = false;
                    }
                    else return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Return control to current player
    /// </summary>
    private void GiveControlToPlayer()
    {
        IsControlToPlayer = false;
        if (CueBallPotted)
        {
            ShouldRepositionCueBall = true;
            //player.GetComponent<Rigidbody>().transform.position = playerPos;
            CueBallPotted = false;
        }
        if (ShouldRepositionCueBall)
        {
            playerIsRepositioning = true;
            ShouldRepositionCueBall = false;
            FreezeBalls();
        }
        else
        {
            playerHasControl = true;
            //player.GetComponent<PlayerControllerScript>().ResetPlayerView();

            //mainCam.transform.SetParent(player.transform);

            // Remove player constraints
            player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;

        }
        // Reset power charge
        ChangePower(0);
        player.GetComponent<PlayerControllerScript>().charge = 0.0f;
        player.GetComponent<PlayerControllerScript>().ResetPlayerView();
    }

    /// <summary>
    /// Stay in selected position and stop repositioning
    /// </summary>
    public void FinishReposition()
    {
        ShouldRepositionCueBall = false;
        IsInitialReposition = false;
        playerIsRepositioning = false;
        playerHasControl = true;
        UnfreezeBalls();
    }

    /// <summary>
    /// Check rules and see if changing the player is needed
    /// Heuristics/game rules
    /// </summary>
    /// <returns></returns>
    private bool ShouldChangePlayer()
    {
        return IsChangePlayer;
    }

    /// <summary>
    /// Change player, select next ball, update text, etc.
    /// </summary>
    private void ChangePlayer()
    {
        currentPlayer = (currentPlayer + 1) % PLAYER_NO;
        Debug.Log("Turn of player " + currentPlayer);

        UI.GetComponent<UIScript>().UpdatePlayer(players[currentPlayer]);

        SelectNextBallForPlayer();
        IsChangePlayer = false;
    }

    /// <summary>
    /// See if player has potted all of his balls
    /// </summary>
    private void SelectNextBallForPlayer()
    {
        // Player has a ball type assigned, and it's not the black ball
        if (players[currentPlayer].GetPlayerType() != BallType.BLACK && players[currentPlayer].GetPlayerType() != BallType.NONE)
        {
            int firstBallPlayer = players[currentPlayer].GetPlayerType() == BallType.SPOT? 1 : 9;   // First ball inclusive
            int lastBallPlayer = players[currentPlayer].GetPlayerType() == BallType.SPOT ? 7 : 15;  // Last ball inclusive 
            for (int i = firstBallPlayer; i <= lastBallPlayer; ++i)
                if (balls[i] != null) return;
            // All balls potted
            players[currentPlayer].SetPlayerType(BallType.BLACK);
        }
        // Then ignore it
    }

    /// <summary>
    /// Change rendering camera
    /// </summary>
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

    /// <summary>
    /// Freeze rotation of cue ball position and rotation of all other balls
    /// </summary>
    public void FreezeBalls()
    {
        foreach (GameObject ball in balls)
        {
            if (ball != null)
            {
                if (ball.GetComponent<BallScript>().BallType == BallType.CUE)
                    ball.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
                else ball.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            }
        }
    }

    /// <summary>
    /// Unfreeze rotation and position of balls
    /// </summary>
    public void UnfreezeBalls()
    {
        foreach (GameObject ball in balls)
        {
            if (ball != null)
                ball.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
        }
    }
    #endregion

    #region LOGIC

    public void ChangePower(float val)
    {
        UI.GetComponent<UIScript>().SetCuePower(val);
    }


    /// <summary>
    /// Reaction to potting a ball
    /// </summary>
    /// <param name="ball"></param>
    public void BallPotted(GameObject ball)
    {
        pottedBalls.Add(ball);
        UI.GetComponent<UIScript>().PutOnRack(ball.GetComponent<BallScript>().BallNo);
    }

    /// <summary>
    /// Check balls potted since last turn
    /// </summary>
    public void CheckPottedBalls()
    {
        foreach (GameObject ball in pottedBalls)
        {
            // Potting black ball
            if (ball.GetComponent<BallScript>().BallType == BallType.BLACK)
            {
                BlackBallPotted = true;
                break;
            }

            // Potting cue ball
            if (ball.GetComponent<BallScript>().BallType == BallType.CUE)
            {
                IsChangePlayer = true;
                CueBallPotted = true;
                player.GetComponent<Rigidbody>().transform.position = playerPos;
                break;
            }

            // Player does not have a ball type. We assign them this type
            if (players[currentPlayer].GetPlayerType() == BallType.NONE)
            {
                IsChangePlayer = false;
                SetPlayersType(ball);
                Debug.Log("Player " + currentPlayer + " plays with " + GetCurrentPlayer().GetPlayerType());
                
            }

            // Player potted one of his balls
            else if (players[currentPlayer].GetPlayerType() == ball.GetComponent<BallScript>().BallType)
            {
                IsChangePlayer = false;              
            }               
        }

        // Remove each ball
        RemoveBalls();

        ResetBallList();
    }

    /// <summary>
    /// Make sure we change player if the first ball hit was not the player's type
    /// </summary>
    public void CheckFirstHitBall()
    {
        if (FirstBallHit == 0 || (players[currentPlayer].GetPlayerType() != BallType.NONE && players[currentPlayer].GetPlayerType() != BallScript.BallTypeFromNo(FirstBallHit)))
        {
            ShouldRepositionCueBall = true;
        }
    }

    /// <summary>
    /// Remove potted balls from the table
    /// </summary>
    private void RemoveBalls()
    {
        foreach (GameObject ball in pottedBalls)
            if (ball.GetComponent<BallScript>().BallType != BallType.CUE)
            Destroy(ball);
    }

    /// <summary>
    /// Set pottedBalls list to empty
    /// </summary>
    private void ResetBallList()
    {
        // We're gonna use this again, so there's no point in creating a new object. It would be slower
        // to allocate new memory and dump it every turn.
        // http://stackoverflow.com/questions/10901020/which-is-faster-clear-collection-or-instantiate-new
        pottedBalls.Clear();
    }

    /// <summary>
    /// One of the players potted the first ball. Assign them their ball type
    /// </summary>
    /// <param name="ball"></param>
    private void SetPlayersType(GameObject ball)
    {
        players[currentPlayer].SetPlayerType(ball.GetComponent<BallScript>().BallType);
        players[(currentPlayer  + 1 ) % PLAYER_NO].SetPlayerType(ball.GetComponent<BallScript>().BallType == BallType.SPOT ? BallType.STRIPE : BallType.SPOT);
    }

    /// <summary>
    /// Check if last turn, player won or lost
    /// Heuristics/game rules
    /// </summary>
    /// <returns></returns>
    private bool CurrentPlayerIsWinner()
    {
        // Was the black ball the next ball for player?
        if (players[currentPlayer].GetPlayerType() == BallType.BLACK)
        {
            // Did she also pot the cue ball?
            if (CueBallPotted)
                return false;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Give control back to MainMenu and let it deal with it? TBD
    /// </summary>
    private void EndGame()
    {
        winnerPlayer = CurrentPlayerIsWinner()? currentPlayer : (currentPlayer + 1) % PLAYER_NO;
        Debug.Log("Player " + winnerPlayer + " wins!");
        GameReady = false;
    }
    #endregion

    #region UI

    /// <summary>
    /// Meant to be used by the UI object. Returns score of current player
    /// </summary>
    /// <returns></returns>
    public int GetPlayerScore()
    {
        return players[currentPlayer].GetScore();
    }



    
    #endregion
}
