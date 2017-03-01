using UnityEngine;
using System.Collections.Generic;
using Pool;

public class GameManagerScript : MonoBehaviour
{

    [Header("Table")]
    public GameObject tablePrefab;                              // Prefab of table
    public Vector3 tablePos;                                    // Initial position of table
    public Vector3 tableRot;                                    // Initial rotation of table

    [Header("Ball")]
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
    [Range(400, 4000)]public float forceApplied;        // Force applied to ball on hit
    [HideInInspector] public bool playerHasControl;     // Flag player is in control

    public const int PLAYER_NO = 2;                     // Amount of players
    [HideInInspector] public int currentPlayer;
    [HideInInspector] Player[] players = new Player[PLAYER_NO];

    [Header("Camera")]
    public GameObject camPrefab;                        // Prefab of cam
    private GameObject mainCam;                         // Reference to main camera

    private Camera mainCamera;
    private Camera secCamera;
    public float camMaxY;
    public float camMinY;


    // Use this for initialization
    void Start ()
    {
        SetupScene();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (!playerHasControl && !AnyBallMoving())
        {
            Debug.Log("Return control to player");
            if (ShouldChangePlayer())
            {
                ChangePlayer();
            }

            GiveControlToPlayer();
        }
	}

    /*******************/
    /*      SETUP      */
    /*******************/

    // Other functions
    void SetupScene()
    {
        // Create table
        Instantiate(tablePrefab, tablePos, Quaternion.Euler(tableRot));

        // Setup player  
        SetupPlayer();

        // Create balls
        SetRack();

        // Get cameras and referecens
        SetupCameras();
    }

    // Setup all cameras and references
    void SetupCameras()
    {
        mainCamera = mainCam.GetComponent<Camera>();
        secCamera = GameObject.FindGameObjectWithTag("SecondCam").GetComponent<Camera>();
    }

    // Put all balls in position as in a rack
    void SetRack()
    {
        float[] xOffset = { 0.0f,
                        -0.5f,  0.5f,
                     -1.0f, 0.0f, 1.0f,
                  -1.5f, -0.5f, 0.5f, 1.5f,
               -2.0f, -1.0f, 0.0f, 1.0f, 2.0f};
        float[] zOffset = { 0.0f,
                        1.0f,  1.0f,
                     2.0f, 2.0f, 2.0f,
                  3.0f, 3.0f,3.0f, 3.0f,
               4.0f, 4.0f, 4.0f, 4.0f, 4.0f};
        
        for (int i = 1; i < BALLS_NO; ++i)
        {

            GameObject newBall = (GameObject)Instantiate(ballPrefab, new Vector3(ballPos.x + xOffset[i - 1], ballPos.y, ballPos.z + zOffset[i - 1]), Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)));

            newBall.GetComponent<MeshRenderer>().material = ballMaterial[i];

            balls[i] = newBall;  // Uninitialised?
        }
        player.GetComponent<PlayerControllerScript>().nextBall = balls[1];

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
    void SetupPlayer()
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
    }

    /*******************/
    /*     PLAYER      */
    /*******************/

    // Ask every ball to see if they're still moving
    bool AnyBallMoving()
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
    void GiveControlToPlayer()
    {
        playerHasControl = true;
        player.GetComponent<PlayerControllerScript>().ResetPlayerView();

        mainCam.transform.SetParent(player.transform);
    }

    // Check rules and see if changing the player is needed
    // Heuristics/game rules
    bool ShouldChangePlayer()
    {
        return false;
    }

    // Change player, select next ball, update text, etc.
    void ChangePlayer()
    {
        // TODO
        currentPlayer = (currentPlayer + 1) % PLAYER_NO;

        SelectNextBallForPlayer();
    }

    // Make nextBall be the appropriate for the player
    void SelectNextBallForPlayer()
    {
        // TODO
    }

    // Change rendering camera
    public void ToggleView()
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
}
