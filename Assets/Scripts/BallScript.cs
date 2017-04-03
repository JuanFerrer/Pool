using UnityEngine;
using System.Collections;

namespace Pool
{
    public enum BallType { NONE, SPOT, STRIPE, BLACK, CUE};


    public class BallScript : MonoBehaviour
    {

        [HideInInspector] public bool isMoving;

        public  int BallNo { get; set; }
        public BallType BallType { get; set; }

        public AudioClip cueHit;
        public AudioClip ballHit;
        public AudioClip cushionHit;
        public AudioClip pocketDrop;
        [HideInInspector] public AudioSource audioSource;

        /// <summary>
        /// Use this for initialization
        /// </summary>
        void Awake()
        {
            isMoving = false;
        }

        void FixedUpdate()
        {       
            // Can be improved. Should only check if gameManager.GameReady    
            if (!isMoving && GetComponent<Rigidbody>().velocity.magnitude > 0)
            {
                isMoving = true;
            }
        }

        /// <summary>
        /// Unity function. Enter a trigger
        /// </summary>
        /// <param name="other"></param>
        void OnTriggerEnter(Collider other)
        {
             GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>().BallPotted(this.gameObject);
            audioSource.clip = pocketDrop;
            audioSource.volume = 1.0f;
            audioSource.Play();
        }

        /// <summary>
        /// Unity function. Collide with another object
        /// </summary>
        /// <param name="other"></param>
        void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.tag == "Ball" || other.gameObject.tag == "Player")
            {
                audioSource.clip = ballHit;
                audioSource.volume = Mathf.Log10(this.gameObject.GetComponent<Rigidbody>().velocity.magnitude);
                audioSource.Play();
            }
        }

        public static BallType BallTypeFromNo(int no)
        {
            switch (no)
            {
                case 0:
                    return BallType.CUE;
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                    return BallType.SPOT;
                case 8:
                    return BallType.BLACK;
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                    return BallType.SPOT;
                default:
                    return BallType.NONE;
            }
        }
    }
}
