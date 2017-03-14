using UnityEngine;
using System.Collections;

namespace Pool
{
    public enum BallType { NONE, SPOT, STRIPE, BLACK};


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

        // Use this for initialization
        void Awake()
        {
            isMoving = false;
        }

        void FixedUpdate()
        {
            if (!isMoving && GetComponent<Rigidbody>().velocity.magnitude > 0)
            {
                isMoving = true;
            }
        }

        void OnTriggerEnter(Collider other)
        {
             GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>().BallPotted(this.gameObject);
        }
        void OnTriggerExit(Collider other)
        {
            audioSource.clip = pocketDrop;
            audioSource.volume = 1.0f;
            audioSource.Play();
        }

        void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.tag == "Ball" || other.gameObject.tag == "Player")
            {
                audioSource.clip = ballHit;
                audioSource.volume = Mathf.Log10(this.gameObject.GetComponent<Rigidbody>().velocity.magnitude);
                audioSource.Play();
            }
        }
    }
}
