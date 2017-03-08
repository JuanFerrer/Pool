using UnityEngine;
using System.Collections;

namespace Pool
{
    public enum BallType { SPOT, STRIPE };


    public class BallScript : MonoBehaviour
    {

        [HideInInspector] public bool isMoving;

        public  int BallNo { get; set; }
        public BallType BallType { get; set; }



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
    }
}
