using UnityEngine;
using System.Collections;

namespace Pool
{
    public class PlayerControllerScript : BallScript
    {
        private enum Direction { RIGHT, LEFT, UP, DOWN };
        [HideInInspector] public float rotSpeed;
        [HideInInspector] public GameObject mainCam;
        [HideInInspector] public float forceApplied;
        [HideInInspector] public GameObject gameManager;
        [HideInInspector] public GameObject nextBall;       // Reference to next ball game object

        [HideInInspector] public float camMaxY;
        [HideInInspector] public float camMinY;

        private float tableLineZ = -6.0f;

        // Update is called once per frame
        void FixedUpdate()
        {
            // Player aiming ball
            if (gameManager.GetComponent<GameManagerScript>().playerHasControl)
            {
                if (Input.GetKey(KeyCode.D))
                    Rotate(Direction.RIGHT);
                else if (Input.GetKey(KeyCode.A))
                    Rotate(Direction.LEFT);
                if (Input.GetKey(KeyCode.W))
                    Tilt(Direction.UP);
                else if (Input.GetKey(KeyCode.S))
                    Tilt(Direction.DOWN);

                // Hit
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    HitBall();
                }

                UpdateVisualAid();
            }

            // Player repositioning ball
            else if (gameManager.GetComponent<GameManagerScript>().playerIsRepositioning)
            {
                Vector3 dir = new Vector3();

                if (Input.GetKey(KeyCode.D))
                    dir.x = 1;
                else if (Input.GetKey(KeyCode.A))
                    dir.x = -1;
                if (Input.GetKey(KeyCode.W))
                {
                    if (gameManager.GetComponent<GameManagerScript>().IsInitialReposition)
                    {
                        if (GetComponent<Rigidbody>().transform.position.z < tableLineZ)
                            dir.z = 1;
                    }
                    else
                        dir.z = 1;
                }
                else if (Input.GetKey(KeyCode.S))
                    dir.z = -1;

                Move(dir);

                // Stay in this position
                if (Input.GetKeyDown(KeyCode.Space))
                {
                gameManager.GetComponent<GameManagerScript>().FinishReposition();
                    gameManager.GetComponent<GameManagerScript>().UnfreezeBalls();
                }
            }

            // Toggle view
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                gameManager.GetComponent<GameManagerScript>().ToggleCamera();
            }

        }

        /// <summary>
        /// Rotate right or left
        /// </summary>
        /// <param name="rotDir"></param>
        void Rotate(Direction rotDir)
        {
            int dirToRotate = (rotDir == Direction.RIGHT ? 1 : -1);
            transform.Rotate(dirToRotate * Vector3.up * rotSpeed * Time.deltaTime, Space.World);
        }

        /// <summary>
        /// Move ball to reposition it
        /// </summary>
        /// <param name="dir"></param>
        void Move(Vector3 dir)
        {
            transform.Translate(dir * 0.1f, Space.World);
        }

        /// <summary>
        /// Set player view for next turn
        /// </summary>
        public void ResetPlayerView()
        {
            mainCam.transform.position = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z - 3.0f);
            mainCam.transform.rotation = Quaternion.Euler(10.0f, 0.0f, 0.0f);
            transform.forward = new Vector3(mainCam.transform.forward.x, 0.0f, mainCam.transform.forward.z);
            mainCam.transform.SetParent(transform);
            // To be fixed
            //transform.forward = new Vector3(nextBall.transform.position.x - transform.position.x, 0.0f, nextBall.transform.position.z - transform.position.z);
        }

        /// <summary>
        /// Tilt forwards or backwards
        /// </summary>
        /// <param name="tiltDir"></param>
        void Tilt(Direction tiltDir)
        {
            int dirToTilt = (tiltDir == Direction.UP ? 1 : -1);
            if (mainCam.transform.position.y < camMaxY && tiltDir == Direction.UP)
                transform.Rotate(dirToTilt * Vector3.right * rotSpeed * Time.deltaTime);
            else if (mainCam.transform.position.y > camMinY && tiltDir == Direction.DOWN)
                transform.Rotate(dirToTilt * Vector3.right * rotSpeed * Time.deltaTime);
        }

        /// <summary>
        /// Apply forward force to ball
        /// </summary>
        void HitBall()
        {
            isMoving = true;
            Vector3 facingDirection = transform.forward;
            GetComponent<Rigidbody>().AddForce(facingDirection * forceApplied, ForceMode.Impulse);
            mainCam.transform.SetParent(null);

            gameManager.GetComponent<GameManagerScript>().playerHasControl = false;

            audioSource.clip = cueHit;
            audioSource.Play();

            mainCam.GetComponent<CameraScript>().BallToViewpoint();
        }


        /// <summary>
        /// Draw visual aid on the floor
        /// </summary>
        void UpdateVisualAid()
        {
            //TO DO
        }
    }
}
