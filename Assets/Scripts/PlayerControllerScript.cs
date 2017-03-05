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

        // Update is called once per frame
        void FixedUpdate()
        {
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

            // Toggle view
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                gameManager.GetComponent<GameManagerScript>().ToggleCamera();
            }

        }

        // Rotate right or left
        void Rotate(Direction rotDir)
        {
            int dirToRotate = (rotDir == Direction.RIGHT ? 1 : -1);
            transform.Rotate(dirToRotate * Vector3.up * rotSpeed * Time.deltaTime, Space.World);
        }

        // Set player view for next turn
        public void ResetPlayerView()
        {
            mainCam.transform.position = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z - 3.0f);
            mainCam.transform.rotation = Quaternion.Euler(10.0f, 0.0f, 0.0f);
            transform.forward = new Vector3(mainCam.transform.forward.x, 0.0f, mainCam.transform.forward.z);
            // To be fixed
            //transform.forward = new Vector3(nextBall.transform.position.x - transform.position.x, 0.0f, nextBall.transform.position.z - transform.position.z);
        }

        // Tilt forwards or backwards
        void Tilt(Direction tiltDir)
        {
            int dirToTilt = (tiltDir == Direction.UP ? 1 : -1);
            if (mainCam.transform.position.y < camMaxY && tiltDir == Direction.UP)
                transform.Rotate(dirToTilt * Vector3.right * rotSpeed * Time.deltaTime);
            else if (mainCam.transform.position.y > camMinY && tiltDir == Direction.DOWN)
                transform.Rotate(dirToTilt * Vector3.right * rotSpeed * Time.deltaTime);
        }

        // Apply forward force to ball
        void HitBall()
        {
            isMoving = true;
            Vector3 facingDirection = transform.forward;
            GetComponent<Rigidbody>().AddForce(facingDirection * forceApplied, ForceMode.Impulse);
            mainCam.transform.SetParent(null);

            gameManager.GetComponent<GameManagerScript>().playerHasControl = false;
        }

        // Draw visual aid on the floor
        void UpdateVisualAid()
        {
            //TO DO
        }
    }
}
